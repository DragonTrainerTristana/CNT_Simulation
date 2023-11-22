using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ArrayControl : MonoBehaviour
{

    string filename = "";

    // Start is called before the first frame update
    private bool timeState;
    private float endTime;
    private float startTime;
    private int reflection;
    private int cntName;
    private string line;

    string[,] exportArray;
    string[,] collisionArray;

    private void Start()
    {
        
        filename = Application.dataPath + "/CollisionCheck.csv";

        timeState = true;
        startTime = 0.0f;
        endTime = 15.0f;

        reflection = 100;
        cntName = GetComponent<CNT_Data10>().numCnt * 6;

        exportArray = new string[cntName, reflection];
        collisionArray = new string[cntName, reflection];

        /*
        Debug.Log("START");
        Debug.Log(GetComponent<CNT_Data10>().cntArray.GetLength(0));
        Debug.Log("Analyze");
        for (int i = 0; i < 600; i++) line = GetComponent<CNT_Data10>().cntArray[i].name;
        Debug.Log("NO PROBLEM");*/
    }

    // Update is called once per frame
    void Update()
    {

        startTime += Time.deltaTime;
        if (timeState == true && startTime >= endTime)
        {
            timeState = false;


            for (int i = 0; i < cntName; i++)
            {

                for (int j = 0; j < reflection + 1; j++)
                {
                    if (j == 0)
                    {

                        if (GetComponent<CNT_Data10>().cntArray[i] != null)
                            exportArray[i, j] = GetComponent<CNT_Data10>().cntArray[i].name;

                   
                    }
                    if (j > 0)
                    {

                        if (GetComponent<CNT_Data10>().cntArray[i].GetComponent<orignal>().prefabCapsuleObjNum[j - 1] == null) break;

                        if (GetComponent<CNT_Data10>().cntArray[i].GetComponent<orignal>().prefabCapsuleObjNum[j - 1].GetComponent<SF>().startStatus == true)
                        {
                            exportArray[i, j] += "-1:0";
                            exportArray[i, j] += ",";
                        }

                        exportArray[i, j] += GetComponent<CNT_Data10>().cntArray[i].GetComponent<orignal>().prefabCapsuleObjNum[j - 1].GetComponent<EachSegment>().collisionFiber;

                        if (GetComponent<CNT_Data10>().cntArray[i].GetComponent<orignal>().prefabCapsuleObjNum[j - 1].GetComponent<SF>().finalStatus == true)
                        {
                            exportArray[i, j] += ",";
                            exportArray[i, j] += "-1:1";
                        }


                        //Debug.Log(exportArray[i, j]);
                    }
                }
            }


            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int i = 0; i < exportArray.GetLength(0); i++)
                {
                    string line = "";
                    for (int j = 0; j < exportArray.GetLength(1); j++)
                    {
                        line += exportArray[i, j] + ",";
                    }
                    line = line.TrimEnd(',');
                    sw.WriteLine(line);
                }
            }
            Debug.Log("EXPORT COMPLETE");
        }
    }
}