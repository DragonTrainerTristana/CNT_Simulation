#include <iostream>
#include <vector>
#include <sstream>
#include <fstream>
#include <Eigen/Sparse>
#include <Eigen/Dense>

using namespace std;
using namespace Eigen;

// 루프문을 도는 변수인 i,j을 제외한 나머지는 전부 전역변수

const string COL_FILE = "Collision_18000.csv";
const string DIS_FILE = "Distance_18000.csv";
const int MAX_NODE_SIZE = 30000;
const int MAX_COLLISION = 14;
int node_array[MAX_NODE_SIZE][MAX_COLLISION][2] = { {{0}} };
int storage[MAX_NODE_SIZE][MAX_COLLISION] = { {0} };
int storage_idx[MAX_NODE_SIZE] = { 0 };
bool start_candidate[MAX_NODE_SIZE] = { false };
bool path_status[MAX_NODE_SIZE] = { false };
double Node_distance[MAX_NODE_SIZE][MAX_COLLISION] = { {{0}} };

int num_Node = 0;
int num_non_dup_Node = 0;
int Node_list[MAX_NODE_SIZE][8] = { {0, 0, 0, 0, 0, 0, 0, 0} }; //name_0, name_1, value_0, value_1, parent, left, middle, right
int Node_list_idx = 0;
int Dis_list[MAX_NODE_SIZE][8] = { {0, 0, 0, 0, 0, 0, 0, 0} };

struct Node {
    int name_0;
    int name_1;
    int value_0;
    int value_1;
    char status; // S D F T L -N
    char direction; // + -

    int node_num;
    Node* parent, * left, * middle, * right;

    Node(Node* parent_node, int data1_0, int data1_1, int data2_0, int data2_1, char stat, char dir, int num) {
        this->name_0 = data1_0;
        this->name_1 = data1_1;
        this->value_0 = data2_0;
        this->value_1 = data2_1;
        this->status = stat;
        this->direction = dir;
        this->node_num = num;
        parent = parent_node;
        left = nullptr;
        middle = nullptr;
        right = nullptr;
    }
};

// ---- Funcion Part ----

// Visualization
void printNode(Node* node);
void visualizeNode_Array();

// Decision or Check of Node state
char find_status(int name_0, int name_1, int value_0, int value_1);
bool findPathAliveRecursive(Node* node);
int find_node_num(int name_0, int name_1, int value_0, int value_1, char status, int node_num_given);

// Finding child node or parent node by recursive strategy
void addNodeRecursive(Node* node);
void TreetoListRecursive_first(Node* node);
void TreetoListRecursive_second(Node* node);

// Preprocessing part
vector<int> split(const string& s, char delim);
void preprocessingNode_Array(ifstream& filecol ,ifstream& filedis);
void preprocessingStorage_Array();


int main() {
    // pause("system");
    cout << "Let's make this" << endl;
    ifstream file_col(COL_FILE); // Declare and extract data from ...
    ifstream file_dis(DIS_FILE);
    cout << "Load process is success" << endl;

    // Split and Visualize
    preprocessingNode_Array(file_col, file_dis);
    //visualizeNode_Array();

    // 똑같이 여기에 Distance 부분 넣어주기 (나중에)


    // Storage 저장
    // storage[max_i][max_j] = -1로 초기화 // activate storage candidate[i] = true
    preprocessingStorage_Array();

    for (int i = 0; i < MAX_NODE_SIZE; i++) {

        if (start_candidate[i]) {
            /*
            1) root이니, parent는 null point
            2) 현재 자기 index인, i (name_0)
            3) 1은 start position을 의미하는 것 같음 (name_1)
            4,5) node_array[i][1][0], node_array[i][1][1]은 처음으로 자기에게 붙어있는 CNT Segment info, 각각 value_0,1임
            6) T가 뭐였을까? 연결 상태를 의미하는 걸까?
            7) +는 Direction을 뜻함
            8) -1은 node num을 의미함
            */

            // Make new root of state 'S'
            Node* root = new Node(nullptr, i, 1, node_array[i][1][0], node_array[i][1][1], 'T', '+', -1);
            num_Node = 0;
            num_non_dup_Node = 0;
            addNodeRecursive(root);

            if (findPathAliveRecursive(root)) {
                path_status[i] = true;
                cout << "This path is alive : " << i + 1 << endl;
                cout << "Total amount of segments : " << num_non_dup_Node << endl;
                system("pause");

                // 살아남은 tree 구조를 뜯어봐야함 ㅇㅇ

                for (int i = 0; i < num_Node; i++) for (int j = 0; j < 8; j++) Node_list[i][j] = -4;
                Node_list_idx = 0;
                int root_parent = -1;
                TreetoListRecursive_first(root);
                TreetoListRecursive_second(root);
                for (int i = 0; i < num_non_dup_Node; i++) {
                    cout
                        << Node_list[i][0] << " " // name_0
                        << Node_list[i][1] << " " // name_1
                        << Node_list[i][2] << " " // value_0
                        << Node_list[i][3] << " " // value_1

                        << Node_list[i][4] << " " // parent idx
                        << Node_list[i][5] << " " // middle idx
                        << Node_list[i][6] << " " // left idx
                        << Node_list[i][7] << " " // right idx
                        << endl;
                }

                system("pause");


                // 생존한 아이들
                int matrix_size = 2 * num_non_dup_Node + 2;

                SparseMatrix<double> Node_matrix(matrix_size, matrix_size);


                cout <<"-------- 행렬 작업 --------" <<endl<<endl;

                for (int i = 0; i < num_non_dup_Node; i++) {
                    int node_list_top = Node_list[i][4]; // parent
                    int node_list_bot = Node_list[i][5]; // middle
                    int node_list_lft = Node_list[i][6]; // left
                    int node_list_rgt = Node_list[i][7]; // right
                    cout << Node_list[i][0] << " " << Node_list[i][1] << " " << Node_list[i][2] << " " << Node_list[i][3] << " ";
                    cout << node_list_top << " " << node_list_bot << " " << node_list_lft << " " << node_list_rgt << endl;
                    

                    int mat_row_idx_top = 2 * i;
                    int mat_col_idx_top;
                    mat_col_idx_top = (node_list_top < 0) ? matrix_size + node_list_top : 2 * node_list_top;
                    Node_matrix.insert(mat_row_idx_top, mat_col_idx_top) = 1;
                    if (node_list_top >= 0) Node_matrix.insert(mat_row_idx_top, mat_col_idx_top + 1) = 1;
                    

                    int mat_row_idx_bot = 2 * i;
                    int mat_col_idx_bot;
                    if (-3 < node_list_bot) {
                        mat_col_idx_bot = (node_list_bot < 0) ? matrix_size + node_list_bot : 2 * node_list_bot;
                        Node_matrix.insert(mat_row_idx_bot, mat_col_idx_bot) = 1;
                        if (node_list_bot >= 0) Node_matrix.insert(mat_row_idx_bot, mat_col_idx_bot + 1) = 1;
                    }

                    int mat_row_idx_twin = 2 * i;
                    int mat_col_idx_twin = 2 * i + 1;
                    Node_matrix.insert(mat_row_idx_twin, mat_col_idx_twin) = 1;
                    Node_matrix.insert(mat_col_idx_twin, mat_row_idx_twin) = 1;


                    int mat_row_idx_lft = 2 * i + 1;
                    int mat_col_idx_lft;
                    if (-3 < node_list_lft) {
                        mat_col_idx_lft = (node_list_lft < 0) ? matrix_size + node_list_lft : 2 * node_list_lft;
                        Node_matrix.insert(mat_row_idx_lft, mat_col_idx_lft) = 1;
                        if (node_list_lft >= 0) Node_matrix.insert(mat_row_idx_lft, mat_col_idx_lft + 1) = 1;
                    }


                    int mat_row_idx_rgt = 2 * i + 1;
                    int mat_col_idx_rgt;
                    if (-3 < node_list_rgt) {
                        mat_col_idx_rgt = (node_list_rgt < 0) ? matrix_size + node_list_rgt : 2 * node_list_rgt;
                        Node_matrix.insert(mat_row_idx_rgt, mat_col_idx_rgt) = 1;
                        if (node_list_rgt >= 0) Node_matrix.insert(mat_row_idx_rgt, mat_col_idx_rgt + 1) = 1;
                    }
                }


                for (int i = 0; i < matrix_size; i++) Node_matrix.insert(matrix_size - 1, i) = Node_matrix.coeff(i, matrix_size - 1);
                for (int i = 0; i < matrix_size; i++) Node_matrix.insert(matrix_size - 2, i) = Node_matrix.coeff(i, matrix_size - 2);
                

                for (int i = 0; i < matrix_size; i++) {
                    for (int j = 0; j < matrix_size; j++) {
                        if (Node_matrix.coeff(i, j) != Node_matrix.coeff(j, i) != 0) Node_matrix.coeffRef(i, j) = 0;
                    }
                }


                SparseMatrix<double> Node_matrix_A(matrix_size - 2, matrix_size - 2);
                
                
                for (int i = 0; i < matrix_size - 2; i++) {
                    system("pause");
                    cout << "step : " << i << endl;


                    // i가 두번째 돌때 갑자기 에러가남 왜일까요 왜일까요 왜일까요 왜일까요 왜일까요 왜일까요 왜일까요 
                    for (int j = 0; j < matrix_size - 2; j++) {
                        
                        cout << "-------- 1 --------" << endl;
                        
                        //Node_matrix_A.insert(i, j) = Node_matrix.coeff(i, j);
                        Node_matrix_A.coeffRef(i, j) = Node_matrix.coeff(i, j);
                        
                        if (Node_matrix.coeff(i, j) > 0) {
                            int node_num = -1;
                            int node_idx = -1;
                            int i_0 = Node_list[i][0];
                            int i_1 = Node_list[i][1];
                            int i_2 = Node_list[i][2];
                            int i_3 = Node_list[i][3];
                            int j_0 = Node_list[j][0];
                            int j_1 = Node_list[j][1];
                            int j_2 = Node_list[j][2];
                            int j_3 = Node_list[j][3];
                            if (i_0 == j_0) {
                                node_num = i_0;
                                node_idx = i_1 < j_1 ? i_1 : j_1;
                            }
                            if (Node_list[i][0] == Node_list[j][2]) {
                                node_num = i_0;
                                node_idx = i_1 < j_3 ? i_1 : j_3;
                            }
                            if (Node_list[i][2] == Node_list[j][0]) {
                                node_num = i_2;
                                node_idx = i_3 < j_1 ? i_3 : j_1;
                            }
                            if (Node_list[i][2] == Node_list[j][2]) {
                                node_num = i_2;
                                node_idx = i_3 < j_3 ? i_3 : j_3;
                            }
                            cout << "-------- 2 --------" << endl;
                            
                            Node_matrix_A.insert(i, j) = Node_distance[node_num][node_idx];
                        }
                    }
                }

                VectorXd Node_vector_b(matrix_size - 2);

                Node_matrix_A.makeCompressed();
                cout << "done" << endl;
                system("pause");
            }
        }
    }
}

void addNodeRecursive(Node* node) {

    // If Node info is empty, then print nothing
    if (node == nullptr) {
        cout << "This Node is empty" << endl;
        return;
    }

    bool duplicateFlag = false;

    //cout << "storage_idx[node->name_0] : "  << storage_idx[node->name_0] << endl;
    //cout << "node->name_0 : " << node->name_0 <<  " node->name_1 : " << node->name_1 << endl;
    //cout << "node->value_0 : "<< node->value_0 << " node->value_1 : " << node->value_1 << endl;;

    // node index (row 값)과, stroage에 저장된 node가 같을 경우 // node->name_0은 row, node->name_1은 column
    // storage_idx[node->name_0]은 storage의 column
    for (int i = 0; i < storage_idx[node->name_0]; ++i) {
        if (storage[node->name_0][i] == node->name_1) {
            duplicateFlag = true;
        }
    }

    // Storage에 쌓여있던 값이랑 같아버리면 안되니까 중복처리합니다.
    for (int i = 0; i < storage_idx[node->value_0]; ++i) {
        if ((node->value_0 != -1) && (storage[node->value_0][i] == node->value_1)) duplicateFlag = true;
    }


    // 중복되는 맞는 친구들을 찾았을 경우
    if (duplicateFlag)node->status = 'L';
    // 그렇지 않은 경우ㅏ
    else {
        // S D F T 4가지중 하나 찾기
        // 참고로 자기 자신이 발견되면 어떡하나? (이전에 쌓여있던 스택이랑 겹치면 duplicateFlag = true로 바뀜)
        node->status = find_status(node->name_0, node->name_1, node->value_0, node->value_1);

        storage[node->name_0][storage_idx[node->name_0]] = node->name_1;
        storage_idx[node->name_0] += 1;
        storage[node->value_0][storage_idx[node->value_0]] = node->value_1;
        storage_idx[node->value_0] += 1;
    }

    if (node->status != 'D' && node->status != 'S' && node->status != 'F') {
        num_Node++;
        if (node->status != 'L') {
            node->node_num = num_non_dup_Node;
            num_non_dup_Node++;
        }
        else {
            node->node_num = -2; // L
        }
    }

    // 연결된 게 없으면 종료해야함
    if (node->status != 'T') {
        return;
    }

    int left_name_0 = node->value_0;
    int left_name_1 = node->value_1 - 1;
    int left_value_0 = node_array[left_name_0][left_name_1][0];
    int left_value_1 = node_array[left_name_0][left_name_1][1];
    char left_status = 'N'; // not allocated yet
    char left_direction = '-';
    int left_num = -1; // not allocated yet

    int middle_name_0 = node->name_0;
    int middle_name_1 = node->name_1;
    if (node->direction == '+') middle_name_1 += 1;
    if (node->direction == '-') middle_name_1 -= 1;
    int middle_value_0 = node_array[middle_name_0][middle_name_1][0];
    int middle_value_1 = node_array[middle_name_0][middle_name_1][1];
    char middle_status = 'N'; // not allocate yet
    char middle_direction = node->direction;
    int middle_num = -1; // not allocated yet

    int right_name_0 = node->value_0;
    int right_name_1 = node->value_1 + 1;
    int right_value_0 = node_array[right_name_0][right_name_1][0];
    int right_value_1 = node_array[right_name_0][right_name_1][1];
    char right_status = 'N'; // not allocate yet
    char right_direction = '+';
    int right_num = -1; // not allocated yet

    node->left = new Node(node, left_name_0, left_name_1, left_value_0, left_value_1, left_status, left_direction, left_num);
    node->middle = new Node(node, middle_name_0, middle_name_1, middle_value_0, middle_value_1, middle_status, middle_direction, middle_num);
    node->right = new Node(node, right_name_0, right_name_1, right_value_0, right_value_1, right_status, right_direction, right_num);

    addNodeRecursive(node->left);
    addNodeRecursive(node->middle);
    addNodeRecursive(node->right);

}

void TreetoListRecursive_first(Node* node) {

    if (node == nullptr)return;

    int node_num_tmp = node->node_num;

    if (node_num_tmp >= 0) {

        Node_list[node_num_tmp][0] = node->name_0;
        Node_list[node_num_tmp][1] = node->name_1;
        Node_list[node_num_tmp][2] = node->value_0;
        Node_list[node_num_tmp][3] = node->value_1;
    }

    TreetoListRecursive_first(node->left);
    TreetoListRecursive_first(node->middle);
    TreetoListRecursive_first(node->right);

}

void TreetoListRecursive_second(Node* node) {


    if (node == nullptr || node->status != 'T') {
        return;
    }

    int node_num_tmp = node->node_num;
    int node_parent_idx = -1; // S
    int node_left_idx = -4; // not allocated yet
    int node_middle_idx = -4; // not allocated yet
    int node_right_idx = -4; // not allocated yet

    if (node->parent != nullptr) node_parent_idx = node->parent->node_num;

    int middle_name_0 = node->middle->name_0;
    int middle_name_1 = node->middle->name_1;
    int middle_value_0 = node->middle->value_0;
    int middle_value_1 = node->middle->value_1;
    int middle_status = node->middle->status;
    int middle_node_num = node->middle->node_num;

    int left_name_0 = node->left->name_0;
    int left_name_1 = node->left->name_1;
    int left_value_0 = node->left->value_0;
    int left_value_1 = node->left->value_1;
    int left_status = node->left->status;
    int left_node_num = node->left->node_num;

    int right_name_0 = node->right->name_0;
    int right_name_1 = node->right->name_1;
    int right_value_0 = node->right->value_0;
    int right_value_1 = node->right->value_1;
    int right_status = node->right->status;
    int right_node_num = node->right->node_num;

    node_left_idx = find_node_num(left_name_0, left_name_1, left_value_0, left_value_1, left_status, left_node_num);
    node_middle_idx = find_node_num(middle_name_0, middle_name_1, middle_value_0, middle_value_1, middle_status, middle_node_num);
    node_right_idx = find_node_num(right_name_0, right_name_1, right_value_0, right_value_1, right_status, right_node_num);

    Node_list[node_num_tmp][4] = node_parent_idx; // 
    Node_list[node_num_tmp][5] = node_middle_idx; // 
    Node_list[node_num_tmp][6] = node_left_idx; // 
    Node_list[node_num_tmp][7] = node_right_idx; // -2이길 바래야지

    TreetoListRecursive_second(node->left);
    TreetoListRecursive_second(node->middle);
    TreetoListRecursive_second(node->right);

}

bool findPathAliveRecursive(Node* node) {
    if (node == nullptr)        return false;
    if (node->status == 'F')    return true;
    if (findPathAliveRecursive(node->left) || findPathAliveRecursive(node->middle) || findPathAliveRecursive(node->right)) return true;
    return false;
}

char find_status(int name_0, int name_1, int value_0, int value_1) {

    char stat = 'T';

    if (value_0 == -1 && value_1 == -1) stat = 'D';
    if (value_0 == -1 && value_1 == 1) stat = 'F';
    if (value_0 == -1 && value_1 == 0) {
        stat = 'S'; start_candidate[name_0] = false;
    }
    return stat;

}

int find_node_num(int name_0, int name_1, int value_0, int value_1, char status, int node_num_given) {
    int node_num;
    if (status == 'S') node_num = -1;
    if (status == 'F') node_num = -2;
    if (status == 'D') node_num = -3;
    if (status == 'T') node_num = node_num_given;
    if (status == 'L') {
        for (int i = 0; i < num_non_dup_Node; i++) {
            if (Node_list[i][0] == name_0 && Node_list[i][1] == name_1) node_num = i;
            if (Node_list[i][0] == value_0 && Node_list[i][1] == value_1) node_num = i;
        }
    }
    return node_num;
}

vector<int> split(const string& s, char delim) {
    vector<int> elems;
    stringstream ss(s);
    string item;
    while (getline(ss, item, delim)) {
        elems.push_back(stoi(item)); // Convert string to int
    }
    return elems;
}


void preprocessingNode_Array(ifstream& file_col, ifstream& file_dis) {
    string line_col;
    int i = 0;

    while (getline(file_col, line_col)) {
        stringstream ss_col(line_col);
        string cell_col;
        int j = 0;
        while (getline(ss_col, cell_col, ',')) {
            vector<int> cell_data_col = split(cell_col, 'X');
            if (cell_data_col.size() == 2 && i < MAX_NODE_SIZE && j < MAX_COLLISION) {
                node_array[i][j][0] = cell_data_col[0]; // 이게 전자로
                node_array[i][j][1] = cell_data_col[1]; // 이게 후자로
            }
            else {
                cerr << "Data format error or index out of bounds." << endl;
                cout << "i : " << i << ", j : " << j << endl;
                exit(-1); // 오류가 발생하면 프로그램 종료
            }
            j++;
        }

        // 제대로 추출되는지 하나씩 확인해보기
        //for (int k = 0; k < j; k++) {
        //    cout << node_array[i][k][0] << endl;
        //    cout << node_array[i][k][1] << endl;

        //    cout << "number of column is : " << j << endl;
        //}
        //cout << "number of row is : " << i << endl;
        //system("pause");
        i++;
    }
}

void visualizeNode_Array() {

    for (int i = 0; i < MAX_NODE_SIZE; i++) {
        for (int j = 0; j < MAX_COLLISION; j++) {
            cout << node_array[i][j][0] << "," << node_array[i][j][1] << " ";
        }
        cout << endl;
    }
    cout << "Collision csv to Node_array done" << endl;


}

void preprocessingStorage_Array() {

    for (int i = 0; i < MAX_NODE_SIZE; ++i) {
        for (int j = 0; j < MAX_COLLISION; ++j) {
            storage[i][j] -= 1;
        }
    }
    for (int i = 0; i < MAX_NODE_SIZE; i++) {
        if ((node_array[i][0][0] == -1) && (node_array[i][0][1] == 0)) {
            start_candidate[i] = true;
        }
    }
}
