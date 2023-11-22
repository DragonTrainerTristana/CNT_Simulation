using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

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
        */




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
            string _collisionFiber = arbiContact + " " + arbiSegment; 

            if (arbiContact != parentNode)
            {
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
                    collisionFiber += _collisionFiber;
                }


            }
            else if (arbiContact == parentNode && collState == true) { collisionFiber = "NONE"; }

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