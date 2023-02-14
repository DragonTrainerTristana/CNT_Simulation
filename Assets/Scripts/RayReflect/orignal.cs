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
    public Vector3[] comparePosition;
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
    private Vector3 finalObj;
    private Vector3[] alreadyhitObj;
    private int numObj; // arraynumber
    private bool objStatus = false;
    Vector3 pos;
    

    //Collision Prefab of Cubiod shaped obj
    public GameObject[] prefabCapsuleObjNum;
    public GameObject prefabCapsuleObj;
    Vector3 CentralPos;
    private float dirObj;



    //LineRenderer Separation (안쓰는데 왜 만들었지?)
    public GameObject[,,] separationObj;

    // Time Variable
    private float activationMeshRenderer = 2.0f;
    private float originalTime = 0.0f;

    // Generate Variable
    private bool generatePosition = true;
    private bool endSegment = false;

    //Fiber Management Variable (다른 Field Script를 위한)
    public float objID = 0; //(csv File 관리용)


    private void Awake()
    {
        //Declare Array Size of arrayPosition
        arrayPosition = new Vector3[reflections];
        alreadyhitObj = new Vector3[reflections];
        prefabCapsuleObjNum = new GameObject[reflections];

        numObj = 0;
        //LineRenderer Component
        lineRenderer = GetComponent<LineRenderer>();
        //Initialize
        dirObj = 0f;

        //Mesh mesh = new Mesh();
        //lineRenderer.BakeMesh(mesh);

    }

    void FixedUpdate()
    {

        originalTime += Time.deltaTime;
        if (activationMeshRenderer <= originalTime && generatePosition == true)
        {

            generatePosition = false;
            lineRenderer.enabled = false;

            if (endSegment == true)
            {
                arrayPosition[numPosition] = finalObj;
                numPosition++;
            }
                
            for (int i = 0; i < numPosition - 1; i++)
            {

                CentralPos.x = (arrayPosition[i].x + arrayPosition[i + 1].x) / 2;
                CentralPos.y = (arrayPosition[i].y + arrayPosition[i + 1].y) / 2;
                CentralPos.z = (arrayPosition[i].z + arrayPosition[i + 1].z) / 2;

                dirObj = Vector3.Distance(arrayPosition[i], arrayPosition[i + 1]);

                prefabCapsuleObj.transform.localScale = new Vector3(0.05f, 0.05f, dirObj);

                //여기서 생성해야하고 FiberCollision부분에 ID 할당해야함
                prefabCapsuleObjNum[i] = Instantiate(prefabCapsuleObj, CentralPos, Quaternion.identity);
                                
                //각자 번호 할당
                prefabCapsuleObjNum[i].GetComponent<EachSegment>().segmentNum = i;
                prefabCapsuleObjNum[i].GetComponent<EachSegment>().parentNode = this.gameObject.name;
                prefabCapsuleObjNum[i].GetComponent<EachSegment>().initialPos = arrayPosition[i];
                prefabCapsuleObjNum[i].GetComponent<EachSegment>().currentLength = 0;
                /*
                if (i == 0) { }
                else if (i != 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        prefabCapsuleObjNum[i].GetComponent<EachSegment>().currentLength += prefabCapsuleObjNum[j - 1].GetComponent<EachSegment>().currentLength;

                    }
                    prefabCapsuleObjNum[i].GetComponent<EachSegment>().currentLength += dirObj;
                }
                
                */
                prefabCapsuleObjNum[i].transform.LookAt(arrayPosition[i + 1]);

            }

        }

        ray = new Ray(transform.position, transform.forward);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = maxLength;


        //angle
        for (int i = 0; i < reflections; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength))
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                remainingLength -= Vector3.Distance(ray.origin, hit.point);

                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));

                if (hit.collider.tag == "Silica" && endSegment == false)
                {                
                    arbitaryObj = hit.point;
                
                    if (numObj == 0)
                    {
                        //Debug.Log("Silica 충돌");
                        alreadyhitObj[numObj] = arbitaryObj;
                        numObj++;

                        arrayPosition[numPosition] = this.gameObject.transform.position;
                        numPosition++;

                        arrayPosition[numPosition] = hit.point;
                        numPosition++;
                    }

                    else if (numObj > 0)
                    { // Compare last Collision obj for store of hit.point

                        
                        for (int j = 0; j < numObj; j++)
                        {
                            if (j == 0) objStatus = true;
                            if (alreadyhitObj[j] == arbitaryObj) objStatus = false;
                        }

                        if (objStatus == true)
                        {
                            //Debug.Log("Silica 충돌 num>0");
                            objStatus = false;
                            alreadyhitObj[numObj] = arbitaryObj;
                            numObj++;

                            arrayPosition[numPosition] = hit.point;

                            numPosition++;

                        }

                    }
                }
                else if (hit.collider.tag == "Edge" && endSegment == false) {

                    if (numObj == 0) {
                        //Debug.Log("Edge 충돌");
                        //Debug.Log(this.gameObject.name);
                        arrayPosition[numPosition] = this.gameObject.transform.position;
                        numPosition++;
                    }
                    //else Debug.Log("Edge 충돌");

                    endSegment = true;

                    finalObj = hit.point;
                    //Debug.Log(arrayPosition[numPosition]);
                    //Debug.Log("마지막 충돌 부분");
                    remainingLength = 0.0f;
                    break;
                }
                else if (hit.collider.tag != "Silica")break;
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
            }
        }
    }
}
