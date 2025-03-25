using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beamer : MonoBehaviour
{
    private GameObject segmentParent;

    public int reflections = 8;  // 7�� �ݻ�
    public float segmentLength = 34.56f; // �� ���׸�Ʈ ����

    public Vector3[] arrayPosition;
    public int numPosition = 0;

    public GameObject[] mySegment;
    public List<GameObject> totalSegment = new List<GameObject>();

    public GameObject prefabFiber; // Cylinder Prefab
    public int myParentNum;

    private Ray ray;
    private RaycastHit hit;
    private bool rayCastComplete = false;

    [Header("렌더링 최적화")]
    public Material instancedMaterial; // GPU 인스턴싱 지원 머티리얼
    private MaterialPropertyBlock propertyBlock;
    private static readonly int colorProperty = Shader.PropertyToID("_Color");

    private void Awake()
    {
        arrayPosition = new Vector3[reflections + 1]; // 위치들 저장
        mySegment = new GameObject[reflections + 1];
        
        // MaterialPropertyBlock 초기화
        propertyBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        segmentParent = GameObject.Find("FiberObjects");
        ray = new Ray(transform.position, transform.forward);
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

            // ���� �ݻ� ������ ���� ���� ����
            Vector3 randomReflection = GetRandomDirectionWithinAngle(ray.direction, 30f); // 30�� ���� ������ ���� ���� ����

            if (Physics.Raycast(ray.origin, ray.direction, out hit, segmentLength))
            {
                // �浹�� ���
                targetPoint = hit.point;

                if (hit.collider.CompareTag("Edge") || hit.collider.CompareTag("Start") || hit.collider.CompareTag("Final") || hit.collider.CompareTag("Wall"))
                {
                    // Edge�� �ε��� ��� �ݻ� ó��
                    Vector3 reflectedDirection = Vector3.Reflect(ray.direction, hit.normal);
                    ray = new Ray(hit.point, reflectedDirection);
                }
                else
                {
                    // �⺻���� �ݻ� ó��
                    Vector3 reflectedDirection = Vector3.Reflect(ray.direction, hit.normal + randomReflection);
                    ray = new Ray(hit.point, reflectedDirection);
                }

                arrayPosition[numPosition] = hit.point;
                numPosition++;
                remainingLength -= Vector3.Distance(origin, hit.point);
            }
            else
            {
                // �浹���� ���� ���
                ray = new Ray(targetPoint, randomReflection);
                arrayPosition[numPosition] = targetPoint;
                numPosition++;
                remainingLength -= segmentLength;
            }
        }

        // ���׸�Ʈ ���� �� ����
        for (int i = 0; i < numPosition - 1; i++)
        {
            Vector3 start = arrayPosition[i];
            Vector3 end = arrayPosition[i + 1];
            Vector3 segmentCenter = (start + end) / 2;

            float segmentDistance = Vector3.Distance(start, end);
            if (segmentDistance < 0.1f) continue;

            GameObject segment = Instantiate(prefabFiber, segmentCenter, Quaternion.identity);
            segment.transform.LookAt(end);
            segment.transform.Rotate(90, 0, 0);
            segment.transform.localScale = new Vector3(segment.transform.localScale.x, segmentLength / 2, segment.transform.localScale.z);
            segment.transform.localPosition = segmentCenter;

            // GPU 인스턴싱 설정
            ApplyInstancing(segment);

            totalSegment.Add(segment);

            //θ 
            segment.transform.SetParent(segmentParent.transform);

            // EachSegment ��ũ��Ʈ�� �� ����
            EachSegment eachSegment = segment.GetComponent<EachSegment>();
            if (eachSegment != null)
            {
                eachSegment.parentNode = myParentNum.ToString();
                eachSegment.segmentNum = i;
                mySegment[i] = segment;
            }
        }

        rayCastComplete = true;

        SetSegmentToParent();
    }

    public void SetSegmentToParent()
    {
        //θ Ʈ position, rotation, scale�� �ʱ�ȭ
        this.gameObject.transform.position = Vector3.zero;
        this.gameObject.transform.rotation = Quaternion.identity;
        this.gameObject.transform.localScale = Vector3.one;

        for (int i = 0; i < totalSegment.Count; i++)
        {
            // �θ� ���� (���� ��ǥ�� ��� �� ��)
            totalSegment[i].transform.SetParent(this.gameObject.transform, true);
        }
    }

    private Vector3 GetRandomDirectionWithinAngle(Vector3 direction, float angle)
    {
        // ������ ȸ���� ����
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(-angle, angle), // X �� ȸ��
            Random.Range(-angle, angle), // Y �� ȸ��
            Random.Range(-angle, angle)  // Z �� ȸ��
        );

        // ���� ���Ϳ� ȸ�� ����
        return randomRotation * direction;
    }

    // GPU 인스턴싱 적용 메소드
    private void ApplyInstancing(GameObject segment)
    {
        if (instancedMaterial != null)
        {
            Renderer renderer = segment.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 인스턴싱 지원 머티리얼 적용
                renderer.material = instancedMaterial;
                
                // 각 인스턴스에 고유한 속성 설정 (선택 사항)
                // 예: 미세한 색상 변화로 식별 가능하게
                propertyBlock.SetColor(colorProperty, new Color(
                    Random.Range(0.9f, 1.0f), 
                    Random.Range(0.9f, 1.0f), 
                    Random.Range(0.9f, 1.0f)
                ));
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}
