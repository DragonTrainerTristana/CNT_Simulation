using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SF: MonoBehaviour
{
    //private HashSet<string> uniqueCollisions = new HashSet<string>();
    // 할당하기 orignal.cs(generateSegment)
    // 충돌 지점 계산 변수
    public float currentLength; // 해야함
    public float collisionLength; // 충돌 계산 내용

    public float sLength;
    public float fLength;

    public Vector3 initialPos; // 받음

    public Vector3 startPos;
    public Vector3 finalPos;

    public float startDis;
    public float finalDis;


    public string sFiber;
    public string fFiber;

    public bool startStatus = false;
    public bool finalStatus = false;

    public void FixedUpdate()
    {

    }
    // Start is called before the first frame update

    private void OnTriggerStay(Collider other) // Enter -> Stay
    {

        if (other.gameObject.CompareTag("Start"))
        {

            startStatus = true;
            //startPos = other.ClosestPoint(transform.position);
            startPos = other.ClosestPointOnBounds(transform.position); // 여기 부분이 문제임
            startDis = Vector3.Distance(initialPos, startPos);
            sLength = currentLength + startDis;

            //sFiber = "[" + sLength.ToString() + " S]";
            sFiber = sLength.ToString();
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
        }
    }
}
