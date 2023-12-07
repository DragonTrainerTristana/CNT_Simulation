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

        }
    }



}

void addNodeRecursive(Node* node) {

    // If Node info is empty, then print nothing
    if (node == nullptr) { 
       cout << "This Node is empty" << endl;
        return; }

    bool duplicateFlag = false;
   

    cout << " Candidate_Row : " << node->name_0 + 1 << endl;
    cout << storage_idx[node->name_0] << endl;
    cout << storage[node->name_0][0] <<  " " <<  node->name_1 <<endl;

    system("pause");

    
   

    // node index (row 값)과, stroage에 저장된 node가 같을 경우
    for (int i = 0; i < storage_idx[node->name_0]; ++i) {
        if (storage[node->name_0][i] == node->name_1) {
            duplicateFlag = true;
        }
    }
    
    // node에 충돌된 다른 CNT Index 값과, stroage에 저장된 값이 같을 경우
    for (int i = 0; i < storage_idx[node->value_0]; ++i) {
        if (storage[node->value_0][i] == node->value_0) {
            duplicateFlag = true;
        }
    }

    // 맞는 친구들을 찾았을 경우
    if (duplicateFlag)node->status = 'L';
    // 그렇지 않은 경우ㅏ
    else {
        // S D F T 4가지중 하나 찾기
        // 참고로 자기 자신이 발견되면 어떡하나? 
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
