using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class orignal : MonoBehaviour
{

  
    //Hit Range
    public int reflections;
    public float maxLength;

    // Hit Array Management Variable
    public Vector3[] arrayPosition;
    private bool storePosition = false;
    public int numPosition = 0; // arraynumber
    private Vector3 arbitaryPosition_yes;


    // LineRenderer & Ray Variable 
    private LineRenderer lineRenderer;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 direction;

    //Collision Object from RayCast
    private Vector3 arbitaryObj;
    private Vector3[] alreadyhitObj;
    private int numObj = 0; // arraynumber
    private bool objStatus = false;
    Vector3 pos;

    //Collision Prefab of Cubiod shaped obj
    public GameObject[] prefabCapsuleObjNum;
    public GameObject prefabCapsuleObj;
    Vector3 CentralPos;
    private float dirObj;



    //LineRenderer Separation (�Ⱦ��µ� �� �������?)
    public GameObject[,,] separationObj;

    // Time Variable
    private float activationMeshRenderer = 2.0f;
    private float originalTime = 0.0f;

    // Generate Variable
    private bool generatePosition = true;
    private bool endSegment = false;

    //Fiber Management Variable (�ٸ� Field Script�� ����)
    public float objID = 0; //(csv File ������)


    private void Awake()
    {
        //Declare Array Size of arrayPosition
        arrayPosition = new Vector3[reflections];
        alreadyhitObj = new Vector3[reflections];
        prefabCapsuleObjNum = new GameObject[reflections];

        //LineRenderer Component
        lineRenderer = GetComponent<LineRenderer>();
        //Initialize
        dirObj = 0f;

        //Mesh mesh = new Mesh();
        //lineRenderer.BakeMesh(mesh);

    }

    void Update()
    {

        originalTime += Time.deltaTime;
        if (activationMeshRenderer <= originalTime && generatePosition == true)
        {

            generatePosition = false;
            lineRenderer.enabled = false;


            for (int i = 0; i < numPosition - 1; i++)
            {

                CentralPos.x = (arrayPosition[i].x + arrayPosition[i + 1].x) / 2;
                CentralPos.y = (arrayPosition[i].y + arrayPosition[i + 1].y) / 2;
                CentralPos.z = (arrayPosition[i].z + arrayPosition[i + 1].z) / 2;

                dirObj = Vector3.Distance(arrayPosition[i], arrayPosition[i + 1]);

   
                prefabCapsuleObj.transform.localScale = new Vector3(0.05f, 0.05f, dirObj);

                //���⼭ �����ؾ��ϰ� FiberCollision�κп� ID �Ҵ��ؾ���
                prefabCapsuleObjNum[i] = Instantiate(prefabCapsuleObj, CentralPos, Quaternion.identity);


                prefabCapsuleObjNum[i].transform.LookAt(arrayPosition[i + 1]);



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


                if (hit.collider.tag == "Silica" && endSegment == true)
                {

                    arbitaryObj = hit.point;
                
                    if (numObj == 0)
                    {
                        alreadyhitObj[numObj] = arbitaryObj;
                        numObj++;

                        arrayPosition[numPosition] = this.gameObject.transform.position;
                        numPosition++;


                        arrayPosition[numPosition] = hit.point;
                        numPosition++;
                    }

                    if (numObj > 0)
                    { // Compare last Collision obj for store of hit.point

                        for (int j = 0; j < numObj; j++)
                        {
                            if (j == 0) objStatus = true;
                            if (alreadyhitObj[j] == arbitaryObj) objStatus = false;
                        }

                        if (objStatus == true)
                        {
                            objStatus = false;
                            alreadyhitObj[numObj] = arbitaryObj;
                            numObj++;

                            arrayPosition[numPosition] = hit.point;
                            //Debug.Log(arrayPosition[numPosition]);
                            numPosition++;

                        }

                    }


                    //Debug.Log(arbitaryObj.gameObject.transform.position.x);
                    //Debug.Log(arbitaryObj.gameObject.transform.position.y);
                    //Debug.Log(arbitaryObj.gameObject.transform.position.z);
                }
                if (hit.collider.tag == "Edge" && endSegment == false) {
                    endSegment = true;
                    Debug.Log("�ߴٱ���!");
                    
                    arrayPosition[numPosition] = hit.point;
                    numPosition++;
                    remainingLength = 0.0f;
                }
                if (hit.collider.tag == "Beam")
                {
                    //Debug.Log("I HITS BEAM!!!");
                }

                if (hit.collider.tag != "Silica")
                    break;
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
            }
        }
    }
}