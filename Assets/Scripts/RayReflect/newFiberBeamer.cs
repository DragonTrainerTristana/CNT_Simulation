using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Beamer : MonoBehaviour
{
    public int reflections = 7;  // 7번 반사
    public float segmentLength = 10; // 각 세그먼트 길이 10

    public Vector3[] arrayPosition;
    public int numPosition = 0;

    public GameObject prefabFiber; // Cylinder Prefab

    private LineRenderer lineRenderer;
    private Ray ray;
    private RaycastHit hit;
    private bool rayCastComplete = false;

    private void Awake()
    {
        arrayPosition = new Vector3[reflections + 1]; // 시작점 포함
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        ray = new Ray(transform.position, transform.forward);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        numPosition = 0; // Start마다 초기화

        arrayPosition[numPosition] = transform.position; // 시작점 저장
        numPosition++;

        float remainingLength = segmentLength * reflections; // 전체 남은 길이 설정

        for (int i = 0; i < reflections; i++)
        {
            if (remainingLength <= 0)
                break;

            Vector3 origin = ray.origin;
            Vector3 direction = ray.direction;

            Vector3 targetPoint = origin + direction * segmentLength;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, segmentLength))
            {
                targetPoint = hit.point;
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                if (hit.collider.CompareTag("Edge"))
                {
                    remainingLength = 0;
                    break;
                }

                Vector3 reflectedDirection = RandomReflection(ray.direction, hit.normal);
                ray = new Ray(hit.point, reflectedDirection);

                arrayPosition[numPosition] = hit.point;
                numPosition++;
                remainingLength -= Vector3.Distance(origin, hit.point);
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, targetPoint);
                ray = new Ray(targetPoint, RandomReflection(ray.direction, Vector3.up)); // 유닛 벡터로 반사
                arrayPosition[numPosition] = targetPoint;
                numPosition++;
                remainingLength -= segmentLength;
            }
        }

        // 배열에 저장된 반사 지점을 따라 LineRenderer가 지점을 설정
        lineRenderer.positionCount = numPosition;
        for (int i = 0; i < numPosition; i++)
        {
            lineRenderer.SetPosition(i, arrayPosition[i]);
        }

        // 각 세그먼트에 대해 'prefabFiber'를 생성
        for (int i = 0; i < numPosition - 1; i++)
        {
            Vector3 start = arrayPosition[i];
            Vector3 end = arrayPosition[i + 1];
            Vector3 segmentCenter = (start + end) / 2;

            // 두 지점 사이의 거리 계산
            float segmentDistance = Vector3.Distance(start, end);

            // Cylinder 생성 및 설정
            GameObject segment = Instantiate(prefabFiber, segmentCenter, Quaternion.identity);
            segment.transform.LookAt(end);
            segment.transform.Rotate(90, 0, 0); // Y축이 아닌 Z축을 기준으로 회전하기 위해 90도 회전
            segment.transform.localScale = new Vector3(segment.transform.localScale.x, segmentDistance / 2, segment.transform.localScale.z); // Y축 길이 설정
        }

        rayCastComplete = true; // Raycast가 완료되었음을 표시
    }

    void Update()
    {
        if (rayCastComplete)
        {
            // 이미 계산된 반사 지점을 유지
            for (int i = 0; i < numPosition; i++)
            {
                lineRenderer.SetPosition(i, arrayPosition[i]);
            }
        }
    }

    private Vector3 RandomReflection(Vector3 direction, Vector3 normal)
    {
        // 임의의 반사 각도를 생성
        normal = new Vector3(Random.value, Random.value, Random.value).normalized;
        return Vector3.Reflect(direction, normal);
    }
}
