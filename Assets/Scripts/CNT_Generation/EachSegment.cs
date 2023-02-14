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
    public Vector3 collisionPos; // �ؾ���

    public string collisionFiber; // �ڱ� ���� �浹 �� Fiber
    public string finalFiber;
    private string arbiContact;
    private string arbiSegment;
   

    // �浹 ����
    // tag -> Fiber
    private bool collisionState = false;

    // �浹 ������Ʈ
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Fiber")
        {
            arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
            arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
            if (arbiContact != parentNode)
            {

                collisionFiber = arbiContact + " " + arbiSegment;
                //Debug.Log(collisionFiber);
            }

        }
        else if (other.gameObject.tag == "Edge" && other.gameObject.tag == "Fiber")
        {
            arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
            arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
            if (arbiContact != parentNode)
            {

                collisionFiber = arbiContact + " " + arbiSegment + "final";
                //Debug.Log(collisionFiber);
            }
        }
        else if (other.gameObject.tag == "Edge")
        {
            collisionFiber = "final";
        }
        else collisionFiber = "NONE";
    }


}