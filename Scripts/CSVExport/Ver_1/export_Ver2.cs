using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CNT_Calculation : MonoBehaviour
{
    public GameObject totalFiber;

    string collisionMatrixFile;
    private int fiberCount;
    int[,] collisionMatrix;

    // Start/Final 상태 배열
    string[] startStatusArray;
    string[] finalStatusArray;
    string fileName;


    void Start()
    {

        fiberCount = GetComponent<TestArrayBeammer>().numCnt;
        fileName = "/CollisionCheck" + "_" + fiberCount.ToString() + ".csv";
        collisionMatrixFile = Application.dataPath + fileName;

        collisionMatrix = new int[fiberCount, fiberCount];
        startStatusArray = new string[fiberCount];
        finalStatusArray = new string[fiberCount];
    }

    public void exportMatrixWithSF()
    {
        // 모든 Fiber 순회
        for (int i = 0; i < fiberCount; i++)
        {
            // 현재 Fiber가 존재하는지 확인
            if (GetComponent<TestArrayBeammer>().cntArray[i] == null) continue;

            GameObject currentFiber = GetComponent<TestArrayBeammer>().cntArray[i];

            // 현재 Fiber의 모든 Segment 확인
            Beamer beamer = currentFiber.GetComponent<Beamer>();
            if (beamer == null) continue;

            bool hasStart = false;
            bool hasFinal = false;

            for (int j = 0; j < beamer.mySegment.Length; j++)
            {
                EachSegment segment = beamer.mySegment[j]?.GetComponent<EachSegment>();
                if (segment == null) continue;

                // 충돌된 Fiber 가져오기
                string collisionFiber = segment.collisionFiber;

                if (!string.IsNullOrEmpty(collisionFiber))
                {
                    // 충돌된 Fiber의 번호 파싱 (예: "832X1" -> "832" 추출)
                    string[] split = collisionFiber.Split('X');
                    if (split.Length > 0 && int.TryParse(split[0], out int collisionIndex))
                    {
                        // 충돌 매트릭스 업데이트
                        if (collisionIndex >= 0 && collisionIndex < fiberCount)
                        {
                            collisionMatrix[i, collisionIndex] = 1;
                        }
                    }
                }

                // Start 또는 Final 상태 확인
                if (segment.startStatus) hasStart = true;
                if (segment.finalStatus) hasFinal = true;
            }

            // Start/Final 상태 기록
            startStatusArray[i] = hasStart ? "S" : "None";
            finalStatusArray[i] = hasFinal ? "F" : "None";

            // 자기 자신과 연결
            collisionMatrix[i, i] = 1;
        }

        // CSV 파일로 저장
        using (StreamWriter sw = new StreamWriter(collisionMatrixFile))
        {
            for (int i = 0; i < fiberCount; i++)
            {
                string line = "";

                // Start 상태 추가
                line += startStatusArray[i] + ",";

                // 충돌 매트릭스 데이터 추가
                for (int j = 0; j < fiberCount; j++)
                {
                    line += collisionMatrix[i, j] + ",";
                }

                // Final 상태 추가
                line += finalStatusArray[i];

                sw.WriteLine(line);
            }
        }

        Debug.Log("Collision Matrix with Start/Final Export Complete");
    }
}
