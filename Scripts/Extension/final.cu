#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <string>
#include <cmath>

#ifdef __CUDACC__
#include <cuda.h>
#include <cuda_runtime.h>
#endif

#ifndef __CUDACC__
#define __host__
#define __device__
#endif

struct Vector3 {
    float x, y, z;
    __host__ __device__ Vector3() : x(0), y(0), z(0) {}
    __host__ __device__ Vector3(float X, float Y, float Z) : x(X), y(Y), z(Z) {}
    __host__ __device__ float norm() const { return sqrtf(x * x + y * y + z * z); }
    __host__ __device__ Vector3 operator+(const Vector3& other) const { return Vector3(x + other.x, y + other.y, z + other.z); }
    __host__ __device__ Vector3 operator-(const Vector3& other) const { return Vector3(x - other.x, y - other.y, z - other.z); }
    template <typename T> __host__ __device__ Vector3 operator*(const T scalar) const { return Vector3(x * scalar, y * scalar, z * scalar); }
    template <typename T> __host__ __device__ Vector3 operator/(const T scalar) const { return Vector3(x / scalar, y / scalar, z / scalar); }
    __host__ __device__ Vector3 operator+=(const Vector3& other) { x += other.x; y += other.y; z += other.z; return *this; }
    __host__ __device__ Vector3 operator-=(const Vector3& other) { x -= other.x; y -= other.y; z -= other.z; return *this; }
    template <typename T> __host__ __device__ Vector3 operator*=(const T scalar) { x *= scalar; y *= scalar; z *= scalar; return *this; }
    template <typename T> __host__ __device__ Vector3 operator/=(const T scalar) { x /= scalar; y /= scalar; z /= scalar; return *this; }
    __host__ __device__ void clamp(float minVal) {z = fmaxf(z, minVal);}
};

struct CNT {
    Vector3 start;
    Vector3 end;
};

using matrix = std::vector<std::vector<char>>;

inline char toBit(const std::string& token) { return token == "S" ? 1 : 0; }

std::vector<CNT> readCSV1(const std::string& filename) {
    std::vector<CNT> fibers;
    std::ifstream file(filename);
    if (!file.is_open()) return fibers;
    std::string line;
    std::getline(file, line);
    std::vector<CNT> temp;
    while (std::getline(file, line)) {
        if (line.empty()) continue;
        std::stringstream ss(line);
        std::string token;
        std::vector<float> values;
        std::getline(ss, token, ',');
        while (std::getline(ss, token, ',')) {
            try { values.push_back(std::stof(token)); } catch (...) { values.clear(); break; }
        }
        if (values.size() != 6) continue;
        temp.push_back({Vector3(values[0], values[1], values[2]), Vector3(values[3], values[4], values[5])});
    }
    file.close();
    for (size_t i = 0; i + 7 < temp.size(); i += 8) fibers.push_back({temp[i].start, temp[i + 7].end});
    return fibers;
}

matrix readCSV2(const std::string& filename) {
    matrix mat;
    std::ifstream file(filename);
    if (!file.is_open()) return mat;
    std::string line;
    while (std::getline(file, line)) {
        std::stringstream ss(line);
        std::string token;
        std::vector<char> row;
        while (std::getline(ss, token, ',')) row.push_back((token == "S") ? 1 : token[0] - '0');
        mat.push_back(std::move(row));
    }
    return mat;
}

std::vector<char> flatten(const matrix& mat) {
    std::vector<char> flattened;
    for (const auto& row : mat) flattened.insert(flattened.end(), row.begin(), row.end());
    return flattened;
}

#ifdef __CUDACC__
__global__ void Force(CNT* d_Force, CNT* d_CNTs, char* d_flattened, int size) {
    int i = blockIdx.x * blockDim.x + threadIdx.x;
    if (i >= size) return;
    d_Force[i] = CNT();
    if (d_flattened[i * (size + 2)] == 1) d_Force[i].start.x += -1.0f;
    if (d_flattened[i * (size + 2) + size + 1] == 1) d_Force[i].end.x += 1.0f;
    Vector3 diff = d_CNTs[i].start - d_CNTs[i].end;
    const auto L = 200;
    float difference = (diff.norm() - L) / L;
    Vector3 springforce = diff * difference;
    d_Force[i].start += springforce * -1.0f;
    d_Force[i].end += springforce;
}

__global__ void TakeStep(float dt, CNT* d_Force, CNT* d_CNTs, int size) {
    int i = blockIdx.x * blockDim.x + threadIdx.x;
    if (i >= size) return;
    float half_dt2 = 0.5f * dt * dt;
    d_CNTs[i].start += d_Force[i].start * half_dt2;
    d_CNTs[i].end += d_Force[i].end * half_dt2;
}

__global__ void correct (CNT& d_CNTs, char* d_flattened, int size)
{
    auto i = blockIdx.x * blockDim.x + threadIdx.x;
    
    if (i >= size) return;

    d_CNTs.start.clamp(0.0f);
    d_CNTs.end.clamp(0.0f);

    // 수정 필요...
    for (int j = 1 + i * (size + 2); j < (i + 1) * (size + 2) - 1; ++j) {
        if (d_flattened[j] == 1){
            int idx = j - i * (size + 2) - 1;
            if (idx == i) {}
            else {
                Vector3 diff1 = d_CNTs[i].start - d_CNTs[idx].end;
                Vector3 diff2 = d_CNTs[i].end - d_CNTs[idx].start;
                if (diff1.norm() < diff2.norm()) {
                    diff1 /= 2.0f;
                    d_CNTs[i].start += diff1;
                    d_CNTs[idx].end -= diff1;
                }
                else if (diff2.norm() < diff1.norm()) {
                    diff2 /= 2.0f;
                    d_CNTs[i].start += diff2;
                    d_CNTs[idx].end -= diff2;
                }
            }
        }
    }
}
#endif


void ForceCPU(std::vector<CNT>& Forces, const std::vector<CNT>& CNTs, const std::vector<char>& flattened, int size) {
    const int cols = size + 2;
    for (int i = 0; i < size; ++i) {
        Forces[i] = CNT();
        if (i * cols >= flattened.size() || (i * cols + size + 1) >= flattened.size()) continue;
        if (flattened[i * cols] == 1) Forces[i].start.x += -1.0f;
        if (flattened[i * cols + size + 1] == 1) Forces[i].end.x += 1.0f;
        Vector3 diff = CNTs[i].start - CNTs[i].end;
        const auto L = 200;
        float difference = (diff.norm() - L) / L;

        Vector3 springforce = diff * difference;
        Forces[i].start += springforce * -1.0f;
        Forces[i].end += springforce;
    }
}

void TakeStepCPU(float dt, std::vector<CNT>& Forces, std::vector<CNT>& CNTs, int size) {
    const float mass = 1.0f;
    const float half_dt2 = 0.5f * dt * dt;
    for (int i = 0; i < size; ++i) {
        CNTs[i].start += Forces[i].start * (half_dt2 / mass);
        CNTs[i].end += Forces[i].end * (half_dt2 / mass);
    }
}

void correctCPU (std::vector<CNT>& CNTs, const std::vector<char>& flattened, int size) 
{
    for (int i = 0; i < size; ++i) {
        CNTs[i].start.clamp(0.0f);
        CNTs[i].end.clamp(0.0f);

        for (int j = 1 + i * (size + 2); j < (i + 1) * (size + 2) - 1; ++j) {
            if (flattened[j] == 1) {
                int idx = j - i * (size + 2) - 1;
                if (idx == i) {}
                else {
                    Vector3 diff1 = CNTs[i].start - CNTs[idx].end;
                    Vector3 diff2 = CNTs[i].end - CNTs[idx].start;
                    if (diff1.norm() < diff2.norm()) {
                        diff1 /= 10.0f;
                        CNTs[i].start += diff1;
                        CNTs[idx].end -= diff1;
                    }
                    else if (diff2.norm() < diff1.norm()) {
                        diff2 /= 10.0f;
                        CNTs[i].start += diff2;
                        CNTs[idx].end -= diff2;
                    }
                }
            }
        }
    }
}



void MakeCSV(const CNT* d_CNTs, int size, const std::string& filename) {
    std::vector<CNT> h_CNTs(size);
#ifdef __CUDACC__
    cudaMemcpy(h_CNTs.data(), d_CNTs, sizeof(CNT) * size, cudaMemcpyDeviceToHost);
#else
    for (int i = 0; i < size; ++i) h_CNTs[i] = d_CNTs[i];
#endif
    std::ofstream file(filename);
    if (!file.is_open()) return;
    file << "Fiber Segment Index, Start X, Start Y, Start Z, End X, End Y, End Z\n";
    for (int i = 0; i < size; ++i) {
        const auto& cnt = h_CNTs[i];
        file << i << "," << cnt.start.x << "," << cnt.start.y << "," << cnt.start.z << ","
             << cnt.end.x << "," << cnt.end.y << "," << cnt.end.z << "\n";
    }
    file.close();
}

int main() {
    std::string filename1 = "FiberSegments6000.csv";
    std::string filename2 = "CollisionCheck6000.csv";
    std::string output = "Result.csv";

    std::vector<CNT> CNTs = readCSV1(filename1);
    matrix mat = readCSV2(filename2);
    std::vector<char> flattened = flatten(mat);
    int size = CNTs.size();
    if (size == 0) return -1;

#ifdef __CUDACC__
    CNT* d_CNTs;
    CNT* d_Forces;
    char* d_flattened;
    cudaMalloc(&d_CNTs, sizeof(CNT) * size);
    cudaMalloc(&d_Forces, sizeof(CNT) * size);
    cudaMalloc(&d_flattened, sizeof(char) * flattened.size());
    cudaMemcpy(d_CNTs, CNTs.data(), sizeof(CNT) * size, cudaMemcpyHostToDevice);
    cudaMemcpy(d_flattened, flattened.data(), sizeof(char) * flattened.size(), cudaMemcpyHostToDevice);
    const int threadsPerBlock = 256;
    const int blocksPerGrid = (size + threadsPerBlock - 1) / threadsPerBlock;
    const float dt = 0.01f;
    const int num_steps = 10;
    for (int step = 0; step < num_steps; ++step) {
        Force<<<blocksPerGrid, threadsPerBlock>>>(*d_Forces, *d_CNTs, *d_flattened, size);
        cudaDeviceSynchronize();
        TakeStep<<<blocksPerGrid, threadsPerBlock>>>(dt, *d_Forces, *d_CNTs, size);
        cudaDeviceSynchronize();
        correct<<<blocksPerGrid, threadsPerBlock>>>(*d_CNTs, d_flattened, size);
        cudaDeviceSynchronize();
    }
    MakeCSV(d_CNTs, size, output);
    cudaFree(d_CNTs);
    cudaFree(d_Forces);
    cudaFree(d_flattened);
#else
    std::vector<CNT> Forces(size);
    const float dt = 0.01f;
    const int num_steps = 10;
    for (int step = 0; step < num_steps; ++step) {
        ForceCPU(Forces, CNTs, flattened, size);
        TakeStepCPU(dt, Forces, CNTs, size);
        //correctCPU(CNTs, flattened, size);
    }
    MakeCSV(CNTs.data(), size, output);
#endif
    return 0;
}