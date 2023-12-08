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
void preprocessingNode_Array(ifstream& filecol);
void preprocessingStorage_Array();


int main() {    
    // pause("system");
    cout << "Let's make this" << endl;
    ifstream file_col(COL_FILE); // Declare and extract data from ...
    cout << "Load process is success" << endl;

    // Split and Visualize
    preprocessingNode_Array(file_col);
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
                int root_parent =  - 1;
                TreetoListRecursive_first(root);

                for (int i = 0; i < num_non_dup_Node; i++) {
                    cout
                        << Node_list[i][0] << " " // name_0
                        << Node_list[i][1] << " " // name_1
                        << Node_list[i][2] << " " // value_0
                        << Node_list[i][3] << " " // value_1
                        << endl;
                }

                system("pause");


            }


        }
    }



}

void addNodeRecursive(Node* node) {

    // If Node info is empty, then print nothing
    if (node == nullptr) { 
       cout << "This Node is empty" << endl;
        return; }

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
    if (value_0 == -1 && value_1 ==  1) stat = 'F';
    if (value_0 == -1 && value_1 ==  0) {
        stat = 'S'; start_candidate[name_0] = false;
    }
    return stat;

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


void preprocessingNode_Array(ifstream& file_col) {
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
