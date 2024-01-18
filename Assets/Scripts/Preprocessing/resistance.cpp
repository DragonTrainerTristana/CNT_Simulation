#include <iostream>
#include <vector>
#include <sstream>
#include <fstream>
#include <Eigen/Sparse>
#include <Eigen/Dense>

using namespace std;
using namespace Eigen;



    
int main(){

    MatrixXd matrix_A(3, 3);
    matrix_A << 0, 0, 1,
        0, 0, 0,
        1, 0, 0;

    // 이게 이제, fiber 충돌 길이로 바뀌고
    MatrixXd matrix_B(3, 3);
    matrix_B << 0, 0, 0.078,
        0, 0, 0,
        0.12, 0, 0;
    // 저항 계산을 위한 비저항 값을 설정
    double rho = 1.0; // 예시로 1 ohm·m를 사용하기

    // 저항 행렬로 바꿔주기
    MatrixXd resistance_matrix = rho * matrix_B.array();


    // 저항구하는 matrix 계산: -matrix_A + diag(sum(matrix_A, 2))
    MatrixXd conmat = -resistance_matrix;
    conmat += MatrixXd(resistance_matrix.rowwise().sum().asDiagonal());

    // bb 벡터 계산: -conmat3의 첫 번째 열
    VectorXd bb = -conmat.col(0);

    // AA 행렬 계산: conmat3의 나머지 부분 + 마지막 열에 추가 조건 적용
    MatrixXd AA = conmat.block(0, 1, conmat.rows(), conmat.cols() - 2);
    AA.conservativeResize(AA.rows(), AA.cols() + 1);
    AA.col(AA.cols() - 1) = VectorXd::Zero(AA.rows());
    AA(0, AA.cols() - 1) = -1;
    AA(AA.rows() - 1, AA.cols() - 1) = 1;

    // 선형 방정식 풀기: AA * xx = bb
    VectorXd xx = AA.fullPivLu().solve(bb);

    // 전체 네트워크의 저항 계산: rent = 1 / xx의 마지막 값
    double rent = 1.0 / xx(xx.size() - 1);

    std::cout << "Calculated Resistance: " << rent << std::endl;

    return 0;}
