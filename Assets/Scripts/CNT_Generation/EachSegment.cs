using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachSegment : MonoBehaviour
{

    // �Ҵ��ϱ� orignal.cs(generateSegment)
    public string parentNode; // ����
    public int segmentNum; // ����


    // �浹 ���� ��� ����
    public float currentLength; // �ؾ���
    public float collisionLength; // �浹 ��� ����

    public Vector3 initialPos; // ����
    public Vector3 collisionPos; // �ؾ��� (����)
    public float distance; //�����

    public string collisionFiber; // �ڱ� ���� �浹 �� Fiber
    public string finalFiber;
    private string arbiContact;
    private string arbiSegment;

    public bool collState = true;
       
   

    // �浹 ����
    // tag -> Fiber
    private bool collisionState = false;

    // �浹 ������Ʈ



    private float startTime = 0.0f;
    private float endTime = 5.0f;
    private bool stateTime = true;
    void Update()
    {

        startTime += Time.deltaTime;
        if(stateTime == true && startTime >= endTime)
        {
            /*
            if(collisionPos != null)
            {
                if(segmentNum == 0)distance = Vector3.Distance(initialPos, collisionPos);
                else if (segmentNum != 0)
                {
                    currentLength += Vector3.Distance(initialPos, collisionPos);
                    distance = currentLength;
                }
            }
            */


            stateTime = false;
            if (collisionFiber.Length <= 0) collisionFiber = "NONE";
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