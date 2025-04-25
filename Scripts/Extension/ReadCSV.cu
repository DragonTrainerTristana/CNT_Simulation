// 필요한 header 파일 불러오기
#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>

// CUDA를 사용하기 위한 header 파일 불러오기
// #include <cuda.h>
// #include <cuda_runtime.h>



// CUDA 없이 CPU에서만 코드를 돌릴 때에도 에러 없이 돌리기 위한 부분
// CUDACC가 정의되지 않았으면 = CUDA가 사용 불가능하면
// __host__와 __device__를 빈 macro로 지정
// 코드에서 삭제되어서 에러 발생 X
#ifndef __CUDACC__
#define __host__
#define __device__
#endif


// 3차원 vector struct 정의
struct Vector3 {

    // 3차원에서 점을 나타내는 좌표 저장장
    float x, y, z;

    // 기본 생성자
    __host__ __device__ Vector3() : x(0), y(0), z(0) {}

    // 사용자 정의 생성자
    __host__ __device__ Vector3(float X, float Y, float Z) : x(X), y(Y), z(Z) {}

};


// CNT는 1개의 선으로 표현
// 선은 시작점과 끝점으로 구성됨
// .csv 파일의 구조를 고려해서 좌측에 끝점
// 우측에 시작점이 오도록 struct를 정의
struct CNT {

    Vector3 end;
    Vector3 start;

};



// 
__global__ void processFibers(CNT* fibers, int size) {

    int idx = threadIdx.x + blockIdx.x * blockDim.x;

    if (idx < size) {
        // CUDA에서 points[idx]에 접근 가능
        Vector3 End = fibers[idx].end;
        Vector3 Start = fibers[idx].start;

        printf("Start : (%f, %f, %f), End : (%f, %f, %f)\n",
                Start.x, Start.y, Start.z, End.x, End.y, End.z);
    }
}



// filename이라는 변수와 일치하는 파일 명을 가지는 .csv 파일을 line별로 (row별로) 읽어옴
// 현재 읽고 있는 row에 저장되어 있는 정보를 CNT 구조체의 형태로 저장
// 이후 해당 CNT 구조체를 std::vector라는 container 안에 저장
// 파일 안의 모든 데이터를 읽을 때까지 이를 계속 반복하는 함수
std::vector<CNT> readCSV (const std::string& filename) 
{

    // .csv 파일의 필요한 모든 정보를 담기 위한 대상 CNT 정의
    std::vector<CNT> fibers;

    // .csv 파일 parsing을 위한 도구 불러오기
    std::ifstream file(filename);


    // 파일이 열리지 않으면
    if (!file.is_open()) {

        // 에러 출력 
        std::cerr << "Error : Unable to Open File" << filename << std::endl;

        // 함수의 return 자료형을 맞춰주기 위한 return 값 설정
        // 실제로는 이 경우 file을 읽어오지 못한 경우이므로 points는 비어 있을 것임
        return fibers;

    }


    // 파일의 정보를 한 줄씩 읽을 때 이를 저장할 대상인 line 지정
    std::string line;

    if(std::getline(file, line)) {
        // 첫 번째 줄을 읽지만 아무 것도 하지 않음으로써 무시
    }


    // 앞선 if문을 통해서
    // getline이 첫 번째 줄에 대해서는 이미 실행되었으므로
    // while 문의 getline은 두 번째 줄부터 읽기 시작함
    while(std::getline(file, line)) {

        // 각 줄(row)에 대해서 아래 내용 반복

        // .csv 파일 parsing을 위한 도구 불러오기
        // line 안에 저장된 데이터를 stream으로 불러옴
        // 이제 ss라는 대상 안에는 데이터가 담긴 line stream이 저장됨
        std::stringstream ss(line);
        // line stream에서 임의의 구분자로 구분된 데이터를 불러올 때
        // 구분자로 구분되는 최소의 단위 데이터를 저장하기 위한 대상 token 정의
        std::string token;

        // 한 줄(row)의 데이터를 저장하기 위한 대상 values 정의
        std::vector<float> values;

        // ss에 저장된 line stream에서부터 
        // 쉼표를 구분자로 하여 데이터를 불러오는 함수 std::getline(ss, token, ',')

        // 읽고 있는 줄의 첫 번째 column에는 segment 번호가 들어 있으므로 무시
        std::getline(ss, token, ',');

        // 읽고 있는 줄의 나머지 여섯 개 column을 읽은 뒤
        // 이를 values에 저장함
        while (std::getline(ss, token, ',')) {
            values.push_back(std::stof(token));
        }

        // values에 저장된 대상을 CNT 자료형을 가진 대상인 fiber로 옮기고
        // 이를 다시 .csv 파일의 전체 정보를 담는 fibers로 옮김
        if (values.size() == 6) {
            CNT fiber;
            fiber.end = Vector3(values[0], values[1], values[2]);
            fiber.start = Vector3(values[3], values[4], values[5]);
            fibers.push_back(fiber);
        }

    }


    // 파일 닫아주고
    file.close();

    // .csv 파일 안의 필요한 정보 return
    return fibers;

}



int main () 
{

    std::string filename = "FiberSegments6000-1.csv";
    std::vector<CNT> CNTs = readCSV(filename);

    #ifdef __CUDACC__

        CNT* d_CNTs;
        size_t data_size = CNTs.size() * sizeof(CNT);

        cudaMalloc(&d_CNTs, data_size);
        cudaMemcpy(d_CNTs, CNTs.data(), data_size, cudaMemcpyHostToDevice);

        // CUDA 커널 호출 (여기서 각 점을 처리)
        int blockSize = 256;
        int numBlocks = (CNTs.size() + blockSize - 1) / blockSize;

        // 
        processFibers<<<numBlocks, blockSize>>>(d_CNTs, CNTs.size());

        // 커널 실행 후 결과 확인
        cudaDeviceSynchronize();

        // GPU 메모리 해제
        cudaFree(d_CNTs);

    #else

        for (const auto& fiber : CNTs) {
            std::cerr << "Number of CNTs: " << CNTs.size() << std::endl;
            std::cout << " Start : (" << fiber.start.x << ", " << fiber.start.y << ", " << fiber.start.z << ")"
                    << " End : (" << fiber.end.x << ", " << fiber.end.y << ", " << fiber.end.z << ")" << std::endl;
        }

    #endif
    
    return 0;

}
