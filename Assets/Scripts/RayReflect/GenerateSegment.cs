using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GenerateSegment : MonoBehaviour
{
    //CNT ID
    private string cntID;
    //CNT Segment
    private int segmentNum;

    //RayCast ����
    public int reflections; //�ִ� �ݻ� ���� ����
    public float maxLength; //�ִ� ���� ����

    //Ray ���� ����
    private Ray ray;
    private RaycastHit hit;
    private Vector3 direction;

    //�ð� ���� ����
    private float originalTime;
    private float activationTime; // public���� �ص� ��
    private bool timeStatus;

    //�浹 �κ� ���
    
    public GameObject prefabSegment;
    private GameObject[] segmentManage;
    
    private string[] prefabSegmentObjNum;
    private Vector3[] arrayPosition;
    private Vector3[] arrayTemp;
    private Vector3 tempPos;
    private Vector3 centralPos;

    private float dirObj;
    private int numObj;
    private int numPos;
    private bool objStatus;
    private bool edgecollison;

    LineRenderer lineRenderer;

    void Start()
    {

        segmentManage = new GameObject[reflections];
        prefabSegmentObjNum = new string[reflections + 1];
        arrayPosition = new Vector3[reflections];
        arrayTemp = new Vector3[reflections];

        lineRenderer = GetComponent<LineRenderer>();


        // �ð� ���� �ʱ�ȭ
        originalTime = 0.0f;
        activationTime = 2.0f;
        timeStatus = true;

        // cnt���� �̸� ����
        cntID = this.name; // ex : cnt_ONE62 <- CNT_<ONE>�� 63��° CNT Num
        segmentNum = 1; // segment����

        //array�� ����
        numObj = 0;
        numPos = 0;
        objStatus = false;
        edgecollison = true;

    }


    void Update()
    {

        //Debug.Log(numObj);

        if (originalTime <= activationTime) originalTime += Time.deltaTime;
        else if (originalTime > activationTime && timeStatus == true ) {
            timeStatus = false;
            lineRenderer.enabled = false;

            for (int i = 0; i < numPos -1; i++) {

                centralPos.x = (arrayPosition[i].x + arrayPosition[i + 1].x) / 2;
                centralPos.y = (arrayPosition[i].y + arrayPosition[i + 1].y) / 2;
                centralPos.z = (arrayPosition[i].z + arrayPosition[i + 1].z) / 2;

                dirObj = Vector3.Distance(arrayPosition[i], arrayPosition[i + 1]);
                prefabSegment.transform.localScale = new Vector3(0.05f, 0.05f, dirObj);
                segmentManage[i] = Instantiate(prefabSegment, centralPos, Quaternion.identity);

                //child script ����, �ѹ� �Ҵ�

                segmentManage[i].transform.LookAt(arrayPosition[i + 1]);
            }

        }

        ray = new Ray(transform.position, transform.forward);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = maxLength;

        for (int i = 0; i < reflections; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength))
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                remainingLength -= Vector3.Distance(ray.origin, hit.point);
                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));

                if (hit.collider.tag == "Silica")
                {
                    tempPos = hit.point;
                    
                    if (numObj == 0) {
                        
                        arrayPosition[numPos] = this.gameObject.transform.position; // ���� ��ġ
                        arrayTemp[numObj] = tempPos;
                        Debug.Log("ó�� ���� �Ǵ� �κ�.");

                        numPos++;
                        numObj++;

                        arrayPosition[numPos] = tempPos;
                        numPos++;
                    }

                    if (numObj > 0) {
                        for (int k = 0; k < numObj; k++)
                        {
                            if (k == 0) { objStatus = true; }
                            if (arrayTemp[k] == tempPos) { objStatus = false; }
                        }
                        if(objStatus == true){
                            objStatus = false;
                            arrayTemp[numObj] = tempPos;
                            numObj++;

                            arrayPosition[numPos] = tempPos;
                            Debug.Log(tempPos);
                            numPos++;                      
                        }
                        
                    }
                }
                if (hit.collider.tag == "Edge" || edgecollison == true) {
                    edgecollison = false;
                    tempPos = hit.point;
                    arrayPosition[numPos] = tempPos;
                    numPos++;
                    remainingLength = 0.0f;
                }
                if (hit.collider.tag != "Silica") {
                    break;
                }
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
            }
        }///

    }
}
