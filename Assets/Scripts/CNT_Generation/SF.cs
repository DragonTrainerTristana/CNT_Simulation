using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SF: MonoBehaviour
{
    //private HashSet<string> uniqueCollisions = new HashSet<string>();
    // �Ҵ��ϱ� orignal.cs(generateSegment)
    // �浹 ���� ��� ����
    public float currentLength; // �ؾ���
    public float collisionLength; // �浹 ��� ����

    public float sLength;
    public float fLength;

    public Vector3 initialPos; // ����

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
            startPos = other.ClosestPointOnBounds(transform.position); // ���� �κ��� ������
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
