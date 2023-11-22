#include <iostream>
#include <Eigen/Sparse>
#include <Eigen/Dense>

using namespace std;
using namespace Eigen;
int main() 
{
    SparseMatrix<double> A(4,4);
    A.insert(0,2) = 9.0;
    A.insert(1,1) = 1.0;
    A.insert(2,0) = 5.0;
    A.insert(2,2) = 9.0;
    A.insert(3,0) = 8.0;
    A.insert(3,3) = 6.0;
    A.makeCompressed();

    // cout << A.(0,2) << endl;

    VectorXd b(4);
    b(0) = 1.0;
    b(1) = 1.0;
    b(2) = 1.0;
    b(3) = 1.0;

    VectorXd x(4);

    // SimplicialLLT <SparseMatrix<double>, Upper> solver;
    SparseLU <SparseMatrix<double> > solver;
    solver.compute(A);
    if(solver.info() != Eigen::Success) {
        cout << "Eigen decomposition fail" << endl;
    }
    x = solver.solve(b);

    cout << x << endl;

    return 0;
}