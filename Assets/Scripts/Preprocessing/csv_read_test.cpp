#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <sstream>

using namespace std;

// // Function to split a string by a specific character
// vector<int> split(const string &s, char delim) {
//     vector<int> elems;
//     stringstream ss(s);
//     string item;
//     while (getline(ss, item, delim)) {
//         elems.push_back(stoi(item)); // Convert string to int
//     }
//     return elems;
// }

int main() {
    // vector<vector<vector<int>>> data;
    // ifstream file("data.csv");

    // string line;
    // while (getline(file, line)) {
    //     vector<vector<int>> row;
    //     stringstream ss(line);

    //     string cell;
    //     while (getline(ss, cell, ',')) {
    //         row.push_back(split(cell, ':')); // Split by ':', convert to int and store
    //     }

    //     data.push_back(row);
    // }
    // cout << "hi" << endl;
    // // Print data
    // for (const auto &rows : data) {
    //     for (const auto &cell : rows) {
    //         cout << "(" << cell[0] << ", " << cell[1] << "), ";
    //     }
    //     cout << endl;
    // }

    bool my_array[6] = {false};
    my_array[0] = true;
    my_array[2] = true;
    my_array[5] = true;

    for (int i = 0; i < 6; i++){
        if(my_array[i]){
            if (i == 2) {
                my_array[5] = false;
            }
            cout << i << " is ture" << endl;
        }
    }
    return 0;
}
