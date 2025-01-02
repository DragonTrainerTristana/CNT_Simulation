public void exportCSV()
{
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
                // 쉼표로 구분된 충돌 Fiber ID를 파싱
                string[] collisionIDs = collisionFiber.Split(',');
                foreach (string id in collisionIDs)
                {
                    if (int.TryParse(id, out int collisionIndex) && collisionIndex >= 0 && collisionIndex < fiberCount)
                    {
                        // 충돌 매트릭스 업데이트
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
