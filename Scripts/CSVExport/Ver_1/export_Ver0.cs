using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Xml.Schema;
using UnityEngine;


public class csvExport : MonoBehaviour
{

    // 여기에 CSV 파일 추출하는거 만들기 
    // 테스트 빌드에 대한 정보를 얻어야합니다.
    // csv 파일에 대한 각각의 EachSegment 스크립트에서, Collision 정보와, Distance 정보를 가져와야 합니다
    // Static이든 Extension이든 이미 모델은 있으니 csv파일만 추출하면 되는 상황입니다
    // 문제가 뭐냐면, finalFiber로 충돌된 애들은, segment 정보가 하나 당 여러개라, 어떻게 계산할 지 잘 모르겠음

    string distanceCheckFile;
    string collisionCheckFile;

    public GameObject totalFiber;


    // CSV 파일 검사 시작 시간
    private float startTime;
    private bool timeState;

    private bool[] start;
    private bool[] final;
    private float[] distance;

    // 배열 변수
    private int cntNum;
    public int reflection;

    // 최종 분석해야할 CSV 파일
    string[,] collisionCheckArray;
    string[,] distanceCheckArray;

    void Start()
    {
        distanceCheckFile = Application.dataPath + "/CollisionDistance.csv";
        collisionCheckFile = Application.dataPath + "/CollisionCheck.csv";

        cntNum = GetComponent<TestArrayBeammer>().numCnt;
        reflection = 9;

        startTime = 0.0f;
        timeState = false;

        // CSV 파일 초기화
        collisionCheckArray = new string[cntNum, reflection];
        distanceCheckArray = new string[cntNum, reflection];
    }
    public void exportCSV()
    {
            // CollisionCheck
            for (int i = 0; i < cntNum; i++) // Fiber
            {
                for (int j = 0; j < reflection; j++) // Segment
                {
                    if (j == 0) // 첫빠따는 이 친구가 있는지 없는지를 체크해야함 (가끔 Fiber가 없는 경우가 있음)
                    {
                        if (GetComponent<TestArrayBeammer>().cntArray[i] != null) // 있으면, 고유번호 불러오기
                        {
                            collisionCheckArray[i, j] = GetComponent<TestArrayBeammer>().cntArray[i].name;
                        }
                    }

                if (j > 0)
                        {
                         //Debug.Log("i, j : "+ i + " , "+ j);
                        if (GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1] == null) break;

                    if (GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1].GetComponent<EachSegment>().startStatus == true)
                    {
                        //collisionCheckArray[i, j] += "-1X0";
                        collisionCheckArray[i, j] += "1V";
                        if (GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1].GetComponent<EachSegment>().yeah == true)
                        {
                            collisionCheckArray[i, j] += "|";
                        }
                        }
                        collisionCheckArray[i, j] += GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1].GetComponent<EachSegment>().collisionFiber;
                    if (GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1].GetComponent<EachSegment>().finalStatus == true)
                    {
                        if (GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1].GetComponent<EachSegment>().yeah == true)
                        {
                            collisionCheckArray[i, j] += "|";
                        }
                        collisionCheckArray[i, j] += "0V";
                            //collisionCheckArray[i, j] += "-1X1";
                        }
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(collisionCheckFile))
            {
                for (int i = 0; i < collisionCheckArray.GetLength(0); i++)
                {
                    string line = "";
                    for (int j = 0; j < collisionCheckArray.GetLength(1); j++)
                    {
                        line += collisionCheckArray[i, j] + ",";
                    }
                    line = line.TrimEnd(',');
                    sw.WriteLine(line);
                }
            }
            Debug.Log("Collsion EXPORT COMPLETE");

        

    }
}
