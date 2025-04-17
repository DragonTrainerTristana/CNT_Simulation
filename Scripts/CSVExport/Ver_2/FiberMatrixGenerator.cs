using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI; // 버튼 이벤트용

public class FiberMatrixGenerator : MonoBehaviour
{
    // TestArrayBeammer 참조 (Inspector에서 할당)
    public TestArrayBeammer arrayBeamer;
    public ConductivityCalculator conductivityCalculator;

    // 파일의 출력 경로 설정
    public string outputFilePath = "Assets/FiberMatrix.csv";

    // 진행 표시용 텍스트 (상태 표시)
    public Text progressText;

    // 충돌 데이터가 생성될 충분 대기 시간 (초)
    public float delayBeforeGeneration = 4.0f;

    // 이미 생성 작업중 확인 플래그
    private bool isGenerationScheduled = false;

    // 생성 시작 메서드 (UI 버튼에서 호출)
    public void GenerateConnectionMatrix()
    {
        if (isGenerationScheduled)
        {
            Debug.Log("이미 생성 작업이 예약되어 있습니다.");
            if (progressText != null) progressText.text = "이미 작업중... 잠시 기다려주세요";
            return;
        }

        if (arrayBeamer == null)
        {
            Debug.LogError("TestArrayBeammer 참조가 설정되지 않았습니다. Inspector에서 할당해주세요.");
            if (progressText != null) progressText.text = "오류: TestArrayBeammer 참조 없음";
            return;
        }

        // 작업 플래그 설정
        isGenerationScheduled = true;

        // 지연 시간 후 실행 
        StartCoroutine(GenerateMatrixAfterDelay());
    }

    private IEnumerator GenerateMatrixAfterDelay()
    {
        if (progressText != null) progressText.text = $"충돌 데이터가 생성 되길 대기 중... ({delayBeforeGeneration}초)";
        Debug.Log($"충돌 데이터가 생성될 때까지 {delayBeforeGeneration}초 대기 중...");

        // 지연 시간 대기
        yield return new WaitForSeconds(delayBeforeGeneration);

        // 실제 생성 작업 시작
        yield return StartCoroutine(GenerateMatrixCoroutine());

        // 작업 완료 후 플래그 초기화
        isGenerationScheduled = false;
    }

    private IEnumerator GenerateMatrixCoroutine()
    {
        Debug.Log("파이버 연결 행렬 생성 시작...");
        if (progressText != null) progressText.text = "행렬 생성 시작...";

        // 1. 파이버 목록 수집하기
        List<string> fiberNames = new List<string>();
        Dictionary<string, int> fiberIndices = new Dictionary<string, int>();
        List<int> startIndices = new List<int>();
        List<int> finalIndices = new List<int>();

        // arrayBeamer의 CNT 배열에서 파이버 목록 수집
        GameObject[] cntArray = arrayBeamer.cntArray;

        if (cntArray == null || cntArray.Length == 0)
        {
            Debug.LogError("CNT 배열이 비어있습니다. 먼저 파이버를 생성해주세요.");
            if (progressText != null) progressText.text = "오류: 파이버가 없음";
            yield break;
        }

        // 파이버 목록 처리
        for (int i = 0; i < cntArray.Length; i++)
        {
            if (cntArray[i] != null)
            {
                string fiberName = i.ToString();
                fiberNames.Add(fiberName);
                fiberIndices[fiberName] = i;

                // Check for start/final status
                Beamer beamer = cntArray[i].GetComponent<Beamer>();
                if (beamer != null)
                {
                    foreach (GameObject segment in beamer.totalSegment)
                    {
                        EachSegment segmentScript = segment.GetComponent<EachSegment>();
                        if (segmentScript != null)
                        {
                            if (segmentScript.startStatus) startIndices.Add(i);
                            if (segmentScript.finalStatus) finalIndices.Add(i);
                        }
                    }
                }
            }

            // 처리 진행 표시
            if (i % 500 == 0)
            {
                if (progressText != null) progressText.text = $"파이버 목록 수집중... {i}/{cntArray.Length}";
                yield return null;
            }
        }

        int fiberCount = fiberNames.Count;
        Debug.Log($"총 {fiberCount}개의 파이버 식별됨. {fiberCount}x{fiberCount} 행렬 생성");
        if (progressText != null) progressText.text = $"{fiberCount}개 파이버 식별됨. 행렬 생성중...";

        // 2. 행렬 초기화
        int[,] connectionMatrix = new int[fiberCount, fiberCount];

        // 대각선 요소(자기 자신과의 연결) 1로 설정
        for (int i = 0; i < fiberCount; i++)
        {
            connectionMatrix[i, i] = 1;
        }

        // 3. 연결 정보 수집
        yield return StartCoroutine(CollectConnectionData(cntArray, connectionMatrix, fiberIndices));

        // 4. 결과를 파일로 저장
        SaveMatrixToFile(connectionMatrix, fiberNames);

        // 5. Conductivity 계산
        if (conductivityCalculator != null)
        {
            conductivityCalculator.CalculateConductivity(connectionMatrix, startIndices, finalIndices);
        }
        else
        {
            Debug.LogWarning("ConductivityCalculator가 설정되지 않았습니다. 전도도 계산을 건너뜁니다.");
        }

        Debug.Log("파이버 연결 행렬 생성 완료!");
        if (progressText != null) progressText.text = "행렬 생성 완료!";
    }

    private IEnumerator CollectConnectionData(GameObject[] cntArray, int[,] matrix, Dictionary<string, int> fiberIndices)
    {
        int processedCount = 0;
        int connectionCount = 0;

        foreach (GameObject cnt in cntArray)
        {
            if (cnt == null) continue;

            processedCount++;
            string cntId = cnt.name;
            if (!fiberIndices.ContainsKey(cntId)) continue;

            int sourceIndex = fiberIndices[cntId];

            // 해당 CNT의 Beamer 컴포넌트 가져오기
            Beamer beamer = cnt.GetComponent<Beamer>();
            if (beamer == null) continue;

            // 각 파이버의 모든 세그먼트 검사
            foreach (GameObject segment in beamer.totalSegment)
            {
                if (segment == null) continue;

                // 세그먼트의 충돌 정보 가져오기
                EachSegment segmentScript = segment.GetComponent<EachSegment>();
                if (segmentScript == null || string.IsNullOrEmpty(segmentScript.collisionFiber)) continue;

                // 충돌 파이버 정보 파싱
                string[] collisions = segmentScript.collisionFiber.Split(',');
                foreach (string collision in collisions)
                {
                    if (string.IsNullOrEmpty(collision)) continue;

                    // 충돌 파이버의 인덱스가 존재하는지 확인
                    if (fiberIndices.ContainsKey(collision))
                    {
                        int targetIndex = fiberIndices[collision];

                        // 행렬 업데이트 (양방향 연결)
                        matrix[sourceIndex, targetIndex] = 1;
                        matrix[targetIndex, sourceIndex] = 1;
                        connectionCount++;
                    }
                }
            }

            // 진행 상태 업데이트
            if (processedCount % 50 == 0)
            {
                float percent = (float)processedCount / cntArray.Length * 100;
                if (progressText != null) progressText.text = $"연결정보 수집 중... {percent:F1}% ({connectionCount} 연결)";
                yield return null;
            }
        }
    }

    private void SaveMatrixToFile(int[,] matrix, List<string> fiberNames)
    {
        try
        {
            // 파일 경로를 Application.dataPath 디렉토리로 설정
            string fileName = "FiberMatrix.csv";
            string filePath = Path.Combine(Application.dataPath, fileName);
            outputFilePath = filePath; // 출력 경로 업데이트

            Debug.Log($"파일 경로: {filePath}");

            // 디렉토리 확인 및 생성
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // 헤더 추가 (피보딩 이름)
                writer.Write("Fiber");
                foreach (string fiberName in fiberNames)
                {
                    writer.Write($",{fiberName}");
                }
                writer.WriteLine();

                // 행별 추가
                for (int i = 0; i < fiberNames.Count; i++)
                {
                    writer.Write(fiberNames[i]);
                    for (int j = 0; j < fiberNames.Count; j++)
                    {
                        writer.Write($",{matrix[i, j]}");
                    }
                    writer.WriteLine();
                }
            }

            Debug.Log($"파일 '{filePath}' 저장됨.");

            // 연결 요약 정보도 저장
            string summaryPath = Path.Combine(directory, "FiberSummary.csv");
            SaveConnectionSummary(matrix, fiberNames, summaryPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 저장 오류: {e.Message}");
            if (progressText != null) progressText.text = $"파일 저장 오류: {e.Message}";
        }
    }

    private void SaveConnectionSummary(int[,] matrix, List<string> fiberNames, string summaryPath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(summaryPath))
            {
                writer.WriteLine("Fiber,ConnectionCount,ConnectedFibers");

                for (int i = 0; i < fiberNames.Count; i++)
                {
                    int connectionCount = 0;
                    List<string> connectedFibers = new List<string>();

                    for (int j = 0; j < fiberNames.Count; j++)
                    {
                        if (i != j && matrix[i, j] == 1)
                        {
                            connectionCount++;
                            connectedFibers.Add(fiberNames[j]);
                        }
                    }

                    writer.WriteLine($"{fiberNames[i]},{connectionCount},{string.Join("|", connectedFibers)}");
                }
            }

            Debug.Log($"연결 요약 정보 '{summaryPath}' 저장됨.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"연결 요약 저장 오류: {e.Message}");
        }
    }

    // 파일 경로 찾기 - Inspector에서 TestArrayBeammer 설정
    public void FindArrayBeammer()
    {
        if (arrayBeamer == null)
        {
            arrayBeamer = FindObjectOfType<TestArrayBeammer>();
            if (arrayBeamer != null)
            {
                Debug.Log("TestArrayBeammer 설정됨.");
            }
            else
            {
                Debug.LogWarning("TestArrayBeammer 설정 실패. 자동으로 설정해주세요.");
            }
        }
    }

    // Unity Editor에서 리셋 호출 메서드
    private void Reset()
    {
        FindArrayBeammer();
    }
}
