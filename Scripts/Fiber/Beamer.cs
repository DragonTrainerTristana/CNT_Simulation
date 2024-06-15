using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Beamer : MonoBehaviour
{
    public int reflections = 7;  // 7�� �ݻ�
    public float segmentLength = 10; // �� ���׸�Ʈ ���� 10

    public Vector3[] arrayPosition;
    public int numPosition = 0;

    public GameObject prefabFiber; // Cylinder Prefab
    public int myParentNum;

    private LineRenderer lineRenderer;
    private Ray ray;
    private RaycastHit hit;
    private bool rayCastComplete = false;

    private void Awake()
    {
        arrayPosition = new Vector3[reflections + 1]; // ������ ����
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        ray = new Ray(transform.position, transform.forward);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        numPosition = 0; // Start���� �ʱ�ȭ

        arrayPosition[numPosition] = transform.position; // ������ ����
        numPosition++;

        float remainingLength = segmentLength * reflections; // ��ü ���� ���� ����

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

                Vector3 reflectedDirection = Vector3.Reflect(ray.direction, hit.normal);
                ray = new Ray(hit.point, reflectedDirection);

                arrayPosition[numPosition] = hit.point;
                numPosition++;
                remainingLength -= Vector3.Distance(origin, hit.point);
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, targetPoint);
                ray = new Ray(targetPoint, ray.direction);  // keep the same direction if no hit
                arrayPosition[numPosition] = targetPoint;
                numPosition++;
                remainingLength -= segmentLength;
            }
        }

        lineRenderer.positionCount = numPosition;
        for (int i = 0; i < numPosition; i++)
        {
            lineRenderer.SetPosition(i, arrayPosition[i]);
        }

        // �� ���׸�Ʈ�� ���� 'prefabFiber'�� ����
        for (int i = 0; i < numPosition - 1; i++)
        {
            Vector3 start = arrayPosition[i];
            Vector3 end = arrayPosition[i + 1];
            Vector3 segmentCenter = (start + end) / 2;

            // �� ���� ������ �Ÿ� ���
            float segmentDistance = Vector3.Distance(start, end);

            // Segment�� �ʹ� ª�� ���, �ǳʶٱ�
            if (segmentDistance < 0.1f) continue;

            // Cylinder ���� �� ����
            GameObject segment = Instantiate(prefabFiber, segmentCenter, Quaternion.identity);
            segment.transform.LookAt(end);
            segment.transform.Rotate(90, 0, 0); // Y���� �ƴ� Z���� �������� ȸ���ϱ� ���� 90�� ȸ��
            segment.transform.localScale = new Vector3(segment.transform.localScale.x, segmentDistance / 2, segment.transform.localScale.z); // Y�� ���� ����

            // EachSegment ��ũ��Ʈ�� �� ����
            EachSegment eachSegment = segment.GetComponent<EachSegment>();
            if (eachSegment != null)
            {
                eachSegment.parentNode = myParentNum.ToString();
                eachSegment.segmentNum = i;
            }
        }

        rayCastComplete = true; // Raycast�� �Ϸ�Ǿ����� ǥ��
    }

    void Update()
    {
        if (rayCastComplete)
        {
            // �̹� ���� �ݻ� ������ ����
            for (int i = 0; i < numPosition; i++)
            {
                lineRenderer.SetPosition(i, arrayPosition[i]);
            }
        }
    }
}
