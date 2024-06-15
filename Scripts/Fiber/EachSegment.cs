using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachSegment : MonoBehaviour
{

    private HashSet<string> uniqueCollisions = new HashSet<string>();
    // 할당하기 orignal.cs(generateSegment)
    public string parentNode; // 받음
    public int segmentNum; // 받음

    // 충돌 지점 계산 변수
    public float currentLength; // 해야함
    public float collisionLength; // 충돌 계산 내용
    public float legendLength;
    public float sLength;
    public float fLength;

    public Vector3 initialPos; // 받음
    public Vector3 collisionPos; // 해야함 (했음)
    public Vector3 sfPos;
    public Vector3 startPos;
    public Vector3 finalPos;
    public float distance; //계산함
    public float startDis;
    public float finalDis;

    public string collisionFiber; // 자기 제외 충돌 된 Fiber
    public string finalFiber;
    public string sFiber;
    public string fFiber;
    private string arbiContact;
    private string arbiSegment;

    public bool collState = true;

    //안 쓰는 변수임
    public bool startStatus = false;
    public bool finalStatus = false;

    public bool SS = false;
    public bool FF = false;

    public bool parentAlive = false;
    public bool childAlive = false;

    private bool deleteStatus = false;


    // 충돌 변수
    // tag -> Fiber
    private bool collisionState = false;

    // 충돌 오브젝트

    private float startTime = 0.0f;
    private float endTime = 5.0f;
    private bool stateTime = true;

    public GameObject environment;
    //private bool possibleTime = false;


    void Update()
    {
        //if(environment.GetComponent<Environment>().fieldCollisionState == true)
        //{
        //    possibleTime = true;
        //}  

        startTime += Time.deltaTime;
        if (endTime <= startTime)
        {
            deleteStatus = true;
        }

    }

    private void OnTriggerStay(Collider other) // Enter -> Stay
    {

            // 여기에서 그냥 다 처리해보면 안되나
            if (other.gameObject.CompareTag("Start"))
            {

                startStatus = true;
                //startPos = other.ClosestPoint(transform.position);
                startPos = other.ClosestPointOnBounds(transform.position); // 여기 부분이 문제임
                startDis = Vector3.Distance(initialPos, startPos);
                sLength = currentLength + startDis;

                //sFiber = "[" + sLength.ToString() + " S]";
                sFiber = sLength.ToString();
                //this.gameObject.GetComponent<EachSegment>().SS = true;
                SS = true;

            }
            if (other.gameObject.CompareTag("Final"))
            {
                finalStatus = true;
                //finalPos = other.contacts[0];
                //finalPos = other.ClosestPoint(transform.position);
                finalPos = other.ClosestPointOnBounds(transform.position);
                finalDis = Vector3.Distance(initialPos, finalPos);
                fLength = currentLength + finalDis;
                //fFiber = "[" + fLength.ToString() + " F]";
                fFiber = fLength.ToString();
                //this.gameObject.GetComponent<EachSegment>().FF = true;
                FF = true;
            }


            if (other.CompareTag("Fiber"))
            {
                
            // 그지 같은거 왜 안되는건데 
            arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
                
            arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
            string _collisionFiber = arbiContact + " " + arbiSegment;


                // 다른 FIber와 충돌했을 때
                if (arbiContact != parentNode)
                {
                    //Debug.Log("???");
                    collState = false;

                    if (!uniqueCollisions.Contains(_collisionFiber))
                    {
                        // Add the collision information to the HashSet 
                        uniqueCollisions.Add(_collisionFiber);

                        // Add a comma to separate collision data entries
                        if (collisionFiber.Length > 0)
                        {
                            collisionFiber += ",";
                            finalFiber += ",";
                        }

                        // Append the collision information to the string
                        collisionPos = other.ClosestPoint(transform.position);
                        distance = Vector3.Distance(initialPos, collisionPos);
                        collisionLength = currentLength + distance;

                        finalFiber += collisionLength.ToString();
                        collisionFiber += arbiContact.ToString();
                    }

                    if (deleteStatus == true)
                    {
                        if (childAlive == false || parentAlive == false)
                        {
                            Destroy(this);
                        }
                    }
                }

                // Parent와 같은 Fiber와 충돌했을 때
                // S와 F의 Status를 주면 됨 (전파)
                else if (arbiContact == parentNode)
                {
                    if (SS == true)
                    {
                        other.gameObject.GetComponent<EachSegment>().SS = true;
                        Debug.Log("SS " + parentNode + " " + segmentNum);
                    }

                    if (FF == true)
                    {
                        other.gameObject.GetComponent<EachSegment>().FF = false;
                        Debug.Log("FF " + parentNode + " " + segmentNum);
                    }

                }

                if (deleteStatus == true)
                {

                    // 충돌도하고, 부모나 자식노드도 있다.
                    if (collisionState == false && arbiContact == parentNode)
                    {
                    }

                    // 충돌노드는 없고 부모나 자식 노드는 있다.
                    // 아무것도 충돌하지 않았을 경우에는, -1X-1 (NONE 값 부여하면 됩니다.)
                    // 만약에 parentNode가 2개면 삭제하면 안되고 당연히, 부모나 자식노드 하나만 있으면 삭제해야함
                    // 이유는, 혼자 삐죽 나온 가지라서, 가지치기 해야함
                    else if (arbiContact == parentNode && collState == true)
                    {
                        if (SS == true || FF == true || SS == false || FF == false)
                        {
                            if (segmentNum == int.Parse(arbiSegment) + 1)
                            {
                                childAlive = true;
                            }
                            else
                            {
                                childAlive = false;
                            }

                            if (segmentNum == int.Parse(arbiSegment) - 1)
                            {
                                parentAlive = true;
                            }
                            else
                            {
                                parentAlive = false;
                            }


                            //하나만 살아도
                            if ((childAlive == true && parentAlive == false) || (childAlive == false && parentAlive == true))
                            {
                                // 아예 스크립트 자체를 없애버리자.
                                Destroy(this);


                            }





                        }
                        else if (SS == false && FF == false)
                        {

                        }

                        collisionFiber = "-1X-1";
                    }
                }
            }
            // 아무것도 충돌하지 않았을 경우 (delete를 하는게 맞을까? 그냥 -1X-1하는게 좋을듯?)
            // 아니 delete는 무조건 해야됨, 그래야 col 감지를 안하지
            // 그리고 distance도 null 값으로 바꾸면 될듯
            else
            {
                Destroy(this);
                //collisionFiber = "-1X-1";
            }

        
    }
}

/*
   public class ori_EachSegment : MonoBehaviour
{
    private HashSet<string> uniqueCollisions = new HashSet<string>();
    // 할당하기 orignal.cs(generateSegment)
    public string parentNode; // 받음
    public int segmentNum; // 받음

    // 충돌 지점 계산 변수
    public float currentLength; // 해야함
    public float collisionLength; // 충돌 계산 내용

    public Vector3 initialPos; // 받음
    public Vector3 collisionPos; // 해야함 (했음)
    public float distance; //계산함

    public string collisionFiber; // 자기 제외 충돌 된 Fiber
    public string finalFiber;
    private string arbiContact;
    private string arbiSegment;

    public bool collState = true;

    private bool startStatus = false;
    private bool finalStatus = false;

    // 충돌 변수
    // tag -> Fiber
    private bool collisionState = false;

    // 충돌 오브젝트

    private float startTime = 0.0f;
    private float endTime = 5.0f;
    private bool stateTime = true;
    void FixedUpdate()
    {

        if (collisionFiber.Length <= 0) { collisionFiber = "NONE"; }
        /*
        startTime += Time.deltaTime;
        if(stateTime == true && startTime >= endTime)
        {
            stateTime = false;
            if (stateTime == true) { collisionFiber = "NONE"; }
            else if (collisionFiber.Length <= 0) collisionFiber = "NONE"; // 일단 여기서 문제 발생하긴 함

            if (finalFiber.Length <= 0) finalFiber = "NONE";
            
        }
        
    }

    private void OnTriggerStay(Collider other) // Enter -> Stay
{


    if (other.gameObject.CompareTag("Start"))
    {
        startStatus = true;
    }
    if (other.gameObject.CompareTag("Final"))
    {
        finalStatus = true;
    }


    if (other.gameObject.tag == "Fiber")
    {


        arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
        arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();

        if (arbiContact != parentNode)
        {
            collState = false;

            if (other.gameObject.tag == "Edge")
            {

                collisionFiber = arbiContact + " " + arbiSegment + " " + "Final";
                if (collisionFiber.Length <= 0) { collisionFiber = "Opt_1"; }
                collisionPos = other.ClosestPoint(transform.position);
                distance = Vector3.Distance(initialPos, collisionPos);
                collisionLength = currentLength + distance;
                finalFiber = collisionLength.ToString();
            }
            else
            {


                collisionFiber = arbiContact + " " + arbiSegment; // 여기가 왜 호출이 잘 안될까? // 충돌 문제일까?
                collisionPos = other.ClosestPoint(transform.position);
                if (collisionFiber.Length <= 0) { collisionFiber = "Opt_2"; }

                distance = Vector3.Distance(initialPos, collisionPos);
                collisionLength = currentLength + distance;
                finalFiber = collisionLength.ToString();

            }
        }
        else if (arbiContact == parentNode && collState == true) { collisionFiber = "NONE"; }
    }
}


}
*/