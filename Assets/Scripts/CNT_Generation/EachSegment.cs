using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachSegment : MonoBehaviour
{

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
       
   

    // 충돌 변수
    // tag -> Fiber
    private bool collisionState = false;

    // 충돌 오브젝트



    private float startTime = 0.0f;
    private float endTime = 5.0f;
    private bool stateTime = true;
    void Update()
    {

        startTime += Time.deltaTime;
        if(stateTime == true && startTime >= endTime)
        {
            stateTime = false;
            if (collisionFiber.Length <= 0) collisionFiber = "NONE";
            if (finalFiber.Length <= 0) finalFiber = "NONE";
            
        }

        



    }

    private void OnTriggerEnter(Collider other)
    {
        
        
        if (other.gameObject.tag == "Fiber")
        {
            collState = false;
            arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
            arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
            if (arbiContact != parentNode)
            {
                collisionPos = other.ClosestPoint(transform.position);
                collisionFiber = arbiContact + " " + arbiSegment;
                distance = Vector3.Distance(initialPos, collisionPos);
                collisionLength = currentLength + distance;
                finalFiber = collisionLength.ToString();
                //Debug.Log(collisionFiber);


            }
            else { collisionFiber = "NONE"; }

        }
        else if (other.gameObject.tag == "Edge" && other.gameObject.tag == "Fiber")
        {
            arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
            arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
            if (arbiContact != parentNode)
            {
                collState = false;
                collisionFiber = arbiContact + " " + arbiSegment + "final";
                //Debug.Log(collisionFiber);
            }
        }
        else if (other.gameObject.tag == "Edge")
        {
            collState = false;
            collisionFiber = "final";
        }
    }


}