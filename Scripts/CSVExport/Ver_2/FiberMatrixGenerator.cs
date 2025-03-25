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
    
    // 데이터 저장 경로 설정
    public string outputFilePath = "Assets/FiberMatrix.csv";
    
    // 진행 표시용 텍스트 (선택 사항)
    public Text progressText;
    
    // 행렬 생성 메서드 (UI 버튼에서 호출)
    public void GenerateConnectionMatrix()
    {
        if (arrayBeamer == null)
        {
            Debug.LogError("TestArrayBeammer 참조가 연결되지 않았습니다. Inspector에서 할당해주세요.");
            if (progressText != null) progressText.text = "오류: TestArrayBeammer 참조 없음";
            return;
        }
        
        StartCoroutine(GenerateMatrixCoroutine());
    }
    
    private IEnumerator GenerateMatrixCoroutine()
    {
        Debug.Log("파이버 연결 행렬 생성 시작...");
        if (progressText != null) progressText.text = "행렬 생성 시작...";
        
        // 1. 파이버 목록 가져오기
        List<string> fiberNames = new List<string>();
        Dictionary<string, int> fiberIndices = new Dictionary<string, int>();
        
        // arrayBeamer의 CNT 배열에서 파이버 목록 추출
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
            }
            
            // 처리 상태 표시
            if (i % 500 == 0)
            {
                if (progressText != null) progressText.text = $"파이버 목록 생성중... {i}/{cntArray.Length}";
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
        
        // 4. 행렬을 파일로 저장
        SaveMatrixToFile(connectionMatrix, fiberNames);
        
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
            
            // 이 파이버의 모든 세그먼트 검사
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
                    
                    // 충돌 파이버가 인덱스에 존재하는지 확인
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
                if (progressText != null) progressText.text = $"데이터 수집 중... {percent:F1}% ({connectionCount} 연결)";
                yield return null;
            }
        }
    }
    
    private void SaveMatrixToFile(int[,] matrix, List<string> fiberNames)
    {
        try
        {
            // 디렉토리 확인 및 생성
            string directory = Path.GetDirectoryName(outputFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                // 헤더 행 작성 (파이버 이름)
                writer.Write("Fiber");
                foreach (string fiberName in fiberNames)
                {
                    writer.Write($",{fiberName}");
                }
                writer.WriteLine();
                
                // 데이터 행 작성
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
            
            Debug.Log($"행렬이 '{outputFilePath}'에 저장되었습니다.");
            
            // 연결 요약 정보도 생성 (선택 사항)
            string summaryPath = Path.Combine(Path.GetDirectoryName(outputFilePath), "FiberSummary.csv");
            SaveConnectionSummary(matrix, fiberNames, summaryPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 저장 오류: {e.Message}");
            if (progressText != null) progressText.text = $"저장 오류: {e.Message}";
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
            
            Debug.Log($"연결 요약이 '{summaryPath}'에 저장되었습니다.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"요약 저장 오류: {e.Message}");
        }
    }
    
    // 편의를 위한 메서드 - Inspector에서 TestArrayBeammer 자동 찾기
    public void FindArrayBeammer()
    {
        if (arrayBeamer == null)
        {
            arrayBeamer = FindObjectOfType<TestArrayBeammer>();
            if (arrayBeamer != null)
            {
                Debug.Log("TestArrayBeammer를 자동으로 찾았습니다.");
            }
            else
            {
                Debug.LogWarning("씬에서 TestArrayBeammer를 찾을 수 없습니다.");
            }
        }
    }
    
    // Unity Editor에서 사용되는 메서드
    private void Reset()
    {
        FindArrayBeammer();
    }
}
