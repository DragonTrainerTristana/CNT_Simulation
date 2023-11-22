#include <iostream>
#include <vector>
#include <sstream>
#include <fstream>
#include <Eigen/Sparse>
#include <Eigen/Dense>

using namespace std;
using namespace Eigen;

const string COL_FILE = "Collision_18000.csv";
const string DIS_FILE = "Distance_18000.csv";
const int MAX_NODE_SIZE = 30000;
const int MAX_COLLISION = 14;
int node_array[MAX_NODE_SIZE][MAX_COLLISION][2] = {{{0}}};
int storage[MAX_NODE_SIZE][MAX_COLLISION] = {{0}};
int storage_idx[MAX_NODE_SIZE] = {0};
bool start_candidate[MAX_NODE_SIZE] = {false};
bool path_status[MAX_NODE_SIZE] = {false};
double Node_distance[MAX_NODE_SIZE][MAX_COLLISION] = {{{0}}};

int num_Node = 0;
int num_non_dup_Node = 0;
int Node_list[MAX_NODE_SIZE][8] = {{0, 0, 0, 0, 0, 0, 0, 0}}; //name_0, name_1, value_0, value_1, parent, left, middle, right
int Node_list_idx = 0;

struct Node {
    int name_0;
    int name_1;
    int value_0;
    int value_1;
    char status; // S D F T L -N
    char direction; // + -
    int node_num;
    Node *parent, *left, *middle, *right;

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

void printNode(Node* node){
    // if (node == nullptr) {
    //     cout << "hi im nullptr" << endl;
    // }
    cout << "Name: " << node->name_0 << ", " << node->name_1 << endl;
    cout << "Value: " << node->value_0 << ", " << node->value_1 << endl;
    cout << "Status: " << node->status << endl;
    cout << "Direction: " << node->direction << endl;
    cout << "Num: " << node->node_num << endl << endl;
}

// void addNode(Node* node, char place, int data1_0, int data1_1, int data2_0, int data2_1, char stat, char dir) {
//     if (place == 'L'){
//         node->left = new Node(data1_0, data1_1, data2_0, data2_1, stat, dir);
//     }
//     if (place == 'M'){
//         node->middle = new Node(data1_0, data1_1, data2_0, data2_1, stat, dir);
//     }
//     if (place == 'R'){
//         node->right = new Node(data1_0, data1_1, data2_0, data2_1, stat, dir);
//     }
// }

char find_status(int name_0, int name_1, int value_0, int value_1){
    char stat = 'T';
    if (value_0 == -1 && value_1 == -1) stat = 'D';
    if (value_0 == -1 && value_1 ==  1) stat = 'F';
    if (value_0 == -1 && value_1 ==  0) {
        stat = 'S';
        start_candidate[name_0] = false;
    }
    return stat;
}

void addNodeRecursive(Node* node) {
    if (node == nullptr) return;

    bool duplicate_flag = false;
    for (int i = 0; i < storage_idx[node->name_0]; ++i){
        if (storage[node->name_0][i] == node->name_1) duplicate_flag = true;
    }
    for (int i = 0; i < storage_idx[node->value_0]; ++i){
        if ( (node->value_0 != -1) && (storage[node->value_0][i] == node->value_1) ) duplicate_flag = true;
    }
    if (duplicate_flag){
        node->status = 'L';
    }
    else{
        node->status = find_status(node->name_0, node->name_1, node->value_0, node->value_1);
        storage[node->name_0][storage_idx[node->name_0]] = node->name_1;
        storage_idx[node->name_0] += 1;
        storage[node->value_0][storage_idx[node->value_0]] = node->value_1;
        storage_idx[node->value_0] += 1;
    }

    if (node->status != 'D' && node->status != 'S' && node->status != 'F') {
        num_Node++;
        if (node->status != 'L'){
            node->node_num = num_non_dup_Node;
            num_non_dup_Node++;
        }else{
            node->node_num = -2; // L
        }
    }
    // printNode(node); 
    // system("pause");

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

bool findPathAliveRecursive(Node* node) {
    if (node == nullptr)        return false;
    if (node->status == 'F')    return true;
    if (findPathAliveRecursive(node->left) || findPathAliveRecursive(node->middle) || findPathAliveRecursive(node->right)) return true;
    return false;
}

int find_node_num(int name_0, int name_1, int value_0, int value_1, char status, int node_num_given){
    int node_num;
    if (status == 'S') node_num = -1;
    if (status == 'F') node_num = -2;
    if (status == 'D') node_num = -3;
    if (status == 'T') node_num = node_num_given;
    if (status == 'L') {
        for (int i = 0; i < num_non_dup_Node; i++){
            if (Node_list[i][0] == name_0 && Node_list[i][1] == name_1) node_num = i;
            if (Node_list[i][0] == value_0 && Node_list[i][1] == value_1) node_num = i;
        }
    }
    return node_num;
}

void TreetoListRecursive_first(Node* node){
    if (node == nullptr) {
        return;
    }

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

void TreetoListRecursive_second(Node* node){
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

    Node_list[node_num_tmp][4] = node_parent_idx;
    Node_list[node_num_tmp][5] = node_middle_idx;
    Node_list[node_num_tmp][6] = node_left_idx;
    Node_list[node_num_tmp][7] = node_right_idx;

    TreetoListRecursive_second(node->left);
    TreetoListRecursive_second(node->middle);
    TreetoListRecursive_second(node->right);
}

vector<int> split(const string &s, char delim) {
    vector<int> elems;
    stringstream ss(s);
    string item;
    while (getline(ss, item, delim)) {
        elems.push_back(stoi(item)); // Convert string to int
    }
    return elems;
}

int main() {
    cout << "Start Calculation" << endl;

    ifstream file_col(COL_FILE); // 충돌 계산 csv
    ifstream file_dis(DIS_FILE); // 거리 계산 csv

    cout << "Loading file done" << endl;

    string line_col;
    int i = 0;
    while (getline(file_col, line_col)) {
        stringstream ss_col(line_col);
        string cell_col;
        int j = 0;
        while (getline(ss_col, cell_col, ',')) {
            vector<int> cell_data_col = split(cell_col, 'X');
            if (cell_data_col.size() == 2 && i < MAX_NODE_SIZE && j < MAX_COLLISION) {
                node_array[i][j][0] = cell_data_col[0];
                node_array[i][j][1] = cell_data_col[1];
            } else {
                cerr << "Data format error or index out of bounds." << endl;
                cout << "i : " << i << ", j : " << j << endl;
                return -1;
            }
            j++;
        }
        i++;
    }
    for (int i = 0;i < MAX_NODE_SIZE; i++){
        for (int j = 0;j < MAX_COLLISION; j++){
            cout << node_array[i][j][0] << "," << node_array[i][j][1] << " ";
        }
        cout << endl;
    }
    cout<< "Collision csv to Node_array done" << endl;

    vector<vector<double>> data_dis;
    string line_dis;
    while (getline(file_dis, line_dis)) {
        stringstream ss_dis(line_dis);
        vector<double> row_dis;
        string value_dis;
        while (getline(ss_dis, value_dis, ',')) {
            if (value_dis == "None") {
                row_dis.push_back(0.0);
            } else {
                row_dis.push_back(stod(value_dis));
            }
        }
        data_dis.push_back(row_dis);
    }
    for (const auto &row_dis : data_dis) {
        for (double value_dis : row_dis) {
            std::cout << value_dis << " ";
        }
        std::cout << std::endl;
    }
    system("pause");

    for(int i = 0; i < MAX_NODE_SIZE; ++i) {
        for (int j =0; j <MAX_COLLISION; ++j){
            storage[i][j] -= 1;
        }
    }
    for (int i = 0; i < MAX_NODE_SIZE; i++) {
        if ( (node_array[i][0][0] == -1) && (node_array[i][0][1] == 0) ){
            start_candidate[i] = true;
        }
    }
    cout << "Initialize variable done" << endl;

    for (int i = 0; i < MAX_NODE_SIZE; i++) {
        if (start_candidate[i]) {
            // cout << i << " th node is candidate" << endl;
            // system("pause");
            Node* root = new Node(nullptr, i, 1, node_array[i][1][0], node_array[i][1][1], 'T', '+', -1);
            num_Node = 0;
            num_non_dup_Node = 0;

            addNodeRecursive(root);
            // cout << num_Node << " Node is exist" << endl;
            // system("pause");
            if (findPathAliveRecursive(root)) {
                path_status[i] = true;
                cout << i << " is alive path" << endl;
                cout << num_non_dup_Node << endl;
                system("pause");

                for (int i = 0; i < num_Node; i++) for (int j = 0; j < 8; j++) Node_list[i][j] = -4; // not allocated yet
                Node_list_idx = 0;
                int root_parent = -1; // S
                TreetoListRecursive_first(root);
                TreetoListRecursive_second(root);
                for (int i = 0; i < num_non_dup_Node; i++){
                    cout
                    << Node_list[i][0] << " "
                    << Node_list[i][1] << " "
                    << Node_list[i][2] << " "
                    << Node_list[i][3] << "  "
                    << Node_list[i][4] << " "
                    << Node_list[i][5] << " "
                    << Node_list[i][6] << " "
                    << Node_list[i][7] << endl;
                }

                int matrix_size = 2 * num_non_dup_Node + 2;
                // int Node_matrix[matrix_size][matrix_size];
                // int Node_matrix[15000][15000];
                // vector<vector<int>> Node_matrix(matrix_size, vector<int>(matrix_size));

                // for (int i = 0; i < matrix_size; i++) for(int j =0; j < matrix_size; j++) Node_matrix[i][j] = 0;
                SparseMatrix<double> Node_matrix(matrix_size,matrix_size);

                for (int i = 0; i < num_non_dup_Node; i++){
                    int node_list_top = Node_list[i][4];
                    int node_list_bot = Node_list[i][5];
                    int node_list_lft = Node_list[i][6];
                    int node_list_rgt = Node_list[i][7];
                    cout << Node_list[i][0] << " " << Node_list[i][1] << " " << Node_list[i][2] << " " << Node_list[i][3] << " ";
                    cout << node_list_top << " " << node_list_bot << " " << node_list_lft << " " << node_list_rgt << endl;

                    int mat_row_idx_top = 2 * i;
                    int mat_col_idx_top;
                    mat_col_idx_top = (node_list_top < 0) ? matrix_size + node_list_top : 2 * node_list_top;
                    Node_matrix.insert(mat_row_idx_top, mat_col_idx_top) = 1;
                    if (node_list_top >= 0) Node_matrix.insert(mat_row_idx_top, mat_col_idx_top+1) = 1;

                    int mat_row_idx_bot = 2 * i;
                    int mat_col_idx_bot;
                    if (-3 < node_list_bot) {
                        mat_col_idx_bot = (node_list_bot < 0) ? matrix_size + node_list_bot : 2 * node_list_bot;
                        Node_matrix.insert(mat_row_idx_bot, mat_col_idx_bot) = 1;
                        if (node_list_bot >= 0) Node_matrix.insert(mat_row_idx_bot, mat_col_idx_bot+1) = 1;
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
                        if (node_list_lft >= 0) Node_matrix.insert(mat_row_idx_lft, mat_col_idx_lft+1) = 1;
                    }

                    int mat_row_idx_rgt = 2 * i + 1;
                    int mat_col_idx_rgt;
                    if (-3 < node_list_rgt) {
                        mat_col_idx_rgt = (node_list_rgt < 0) ? matrix_size + node_list_rgt : 2 * node_list_rgt;
                        Node_matrix.insert(mat_row_idx_rgt, mat_col_idx_rgt) = 1;
                        if (node_list_rgt >= 0) Node_matrix.insert(mat_row_idx_rgt, mat_col_idx_rgt+1) = 1;
                    }
                }
                for (int i = 0; i < matrix_size; i ++) Node_matrix.insert(matrix_size-1, i) = Node_matrix.coeff(i, matrix_size -1);
                for (int i = 0; i < matrix_size; i ++) Node_matrix.insert(matrix_size-2, i) = Node_matrix.coeff(i, matrix_size -2);

                for (int i = 0; i < matrix_size; i++) {
                    for(int j =0; j < matrix_size; j++) {
                        if ( Node_matrix.coeff(i, j) != Node_matrix.coeff(j, i) != 0 ) Node_matrix.coeffRef(i, j) = 0;
                    }
                }

                SparseMatrix<double> Node_matrix_A(matrix_size-2,matrix_size-2);
                for (int i = 0; i < matrix_size-2; i++) {
                    for (int j =0; j < matrix_size-2; j++) {
                        Node_matrix_A.insert(i, j) = Node_matrix.coeff(i, j);
                        if (Node_matrix.coeff(i, j) > 0){
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
                            if (i_0 == j_0){
                                node_num = i_0;
                                node_idx = i_1 < j_1 ? i_1 : j_1;
                            }
                            if (Node_list[i][0] == Node_list[j][2]){
                                node_num = i_0;
                                node_idx = i_1 < j_3 ? i_1 : j_3;
                            }
                            if (Node_list[i][2] == Node_list[j][0]){
                                node_num = i_2;
                                node_idx = i_3 < j_1 ? i_3 : j_1;
                            }
                            if (Node_list[i][2] == Node_list[j][2]){
                                node_num = i_2;
                                node_idx = i_3 < j_3 ? i_3 : j_3;
                            }
                            Node_matrix_A.insert(i, j) = Node_distance[node_num][node_idx];
                        }
                    }
                }

                VectorXd Node_vector_b(matrix_size-2);

                Node_matrix_A.makeCompressed();
                cout << "done" << endl;
                system("pause");
            }
        }
    }  

    return 0;
}