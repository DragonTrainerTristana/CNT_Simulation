using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class csvExport : MonoBehaviour
{
    public GameObject totalFiber;

    // CSV 파일 경로
    string collisionMatrixFile;
    string collisionCheckFile;

    // Fiber 수
    private int fiberCount;

    // 충돌 매트릭스
    int[,] collisionMatrix;

    // 충돌 배열 (기존 기능 유지)
    string[,] collisionCheckArray;
    private int reflection;

    void Start()
    {
        collisionMatrixFile = Application.dataPath + "/CollisionMatrix.csv";
        collisionCheckFile = Application.dataPath + "/CollisionCheck.csv";

        // Fiber 수 초기화
        fiberCount = GetComponent<TestArrayBeammer>().numCnt;
        reflection = 9; // Segment 개수

        // 매트릭스 및 배열 초기화
        collisionMatrix = new int[fiberCount, fiberCount];
        collisionCheckArray = new string[fiberCount, reflection];
    }

    public void exportMatrix()
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
            }
        }

        // CSV 파일로 저장
        using (StreamWriter sw = new StreamWriter(collisionMatrixFile))
        {
            for (int i = 0; i < fiberCount; i++)
            {
                string line = "";
                for (int j = 0; j < fiberCount; j++)
                {
                    line += collisionMatrix[i, j] + ",";
                }
                line = line.TrimEnd(',');
                sw.WriteLine(line);
            }
        }

        Debug.Log("Collision Matrix Export Complete");
    }

    public void exportCSV()
    {
        // CollisionCheck
        for (int i = 0; i < fiberCount; i++) // Fiber
        {
            for (int j = 0; j < reflection; j++) // Segment
            {
                if (j == 0) // 첫 번째 Segment 처리
                {
                    if (GetComponent<TestArrayBeammer>().cntArray[i] != null) // Fiber가 존재하는 경우
                    {
                        collisionCheckArray[i, j] = GetComponent<TestArrayBeammer>().cntArray[i].name;
                    }
                }
                if (j > 0) // 나머지 Segment 처리
                {
                    if (GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1] == null) break;

                    EachSegment segment = GetComponent<TestArrayBeammer>().cntArray[i].GetComponent<Beamer>().mySegment[j - 1].GetComponent<EachSegment>();

                    if (segment.startStatus)
                    {
                        collisionCheckArray[i, j] += "1V";
                        if (segment.yeah)
                        {
                            collisionCheckArray[i, j] += "|";
                        }
                    }
                    collisionCheckArray[i, j] += segment.collisionFiber;
                    if (segment.finalStatus)
                    {
                        if (segment.yeah)
                        {
                            collisionCheckArray[i, j] += "|";
                        }
                        collisionCheckArray[i, j] += "0V";
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

        Debug.Log("Collision Check Export Complete");
    }
}
