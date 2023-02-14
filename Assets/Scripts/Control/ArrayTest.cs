using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ArrayTest : MonoBehaviour
{
    public string[,] data;
    public string filename;

    void Start()
    {
        filename = Application.dataPath + "/yeah.csv";

        data = new string[10, 10];
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++) {
                if (i == 0) data[i, j] = "title";
                else data[i, j] = "yes";
            }
        }

        using (StreamWriter sw = new StreamWriter(filename)) {
            for (int i = 0; i < data.GetLength(0); i++) {
                string line = "";
                for (int j = 0; j < data.GetLength(1); j++) {
                    line += data[i, j] + ",";
                }
                line = line.TrimEnd(',');
                sw.WriteLine(line);
            }
        }
        Debug.Log("CSV WRITE END");

    }


}
