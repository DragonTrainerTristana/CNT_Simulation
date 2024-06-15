using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachSegment : MonoBehaviour
{

    private HashSet<string> uniqueCollisions = new HashSet<string>();
    // �Ҵ��ϱ� orignal.cs(generateSegment)
    public string parentNode; // ����
    public int segmentNum; // ����

    // �浹 ���� ��� ����
    public float currentLength; // �ؾ���
    public float collisionLength; // �浹 ��� ����
    public float legendLength;
    public float sLength;
    public float fLength;

    public Vector3 initialPos; // ����
    public Vector3 collisionPos; // �ؾ��� (����)
    public Vector3 sfPos;
    public Vector3 startPos;
    public Vector3 finalPos;
    public float distance; //�����
    public float startDis;
    public float finalDis;

    public string collisionFiber; // �ڱ� ���� �浹 �� Fiber
    public string finalFiber;
    public string sFiber;
    public string fFiber;
    private string arbiContact;
    private string arbiSegment;

    public bool collState = true;

    //�� ���� ������
    public bool startStatus = false;
    public bool finalStatus = false;

    public bool SS = false;
    public bool FF = false;

    public bool parentAlive = false;
    public bool childAlive = false;

    private bool deleteStatus = false;


    // �浹 ����
    // tag -> Fiber
    private bool collisionState = false;

    // �浹 ������Ʈ

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

            // ���⿡�� �׳� �� ó���غ��� �ȵǳ�
            if (other.gameObject.CompareTag("Start"))
            {

                startStatus = true;
                //startPos = other.ClosestPoint(transform.position);
                startPos = other.ClosestPointOnBounds(transform.position); // ���� �κ��� ������
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
                
            // ���� ������ �� �ȵǴ°ǵ� 
            arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
                
            arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
            string _collisionFiber = arbiContact + " " + arbiSegment;


                // �ٸ� FIber�� �浹���� ��
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

                // Parent�� ���� Fiber�� �浹���� ��
                // S�� F�� Status�� �ָ� �� (����)
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

                    // �浹���ϰ�, �θ� �ڽĳ�嵵 �ִ�.
                    if (collisionState == false && arbiContact == parentNode)
                    {
                    }

                    // �浹���� ���� �θ� �ڽ� ���� �ִ�.
                    // �ƹ��͵� �浹���� �ʾ��� ��쿡��, -1X-1 (NONE �� �ο��ϸ� �˴ϴ�.)
                    // ���࿡ parentNode�� 2���� �����ϸ� �ȵǰ� �翬��, �θ� �ڽĳ�� �ϳ��� ������ �����ؾ���
                    // ������, ȥ�� ���� ���� ������, ����ġ�� �ؾ���
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


                            //�ϳ��� ��Ƶ�
                            if ((childAlive == true && parentAlive == false) || (childAlive == false && parentAlive == true))
                            {
                                // �ƿ� ��ũ��Ʈ ��ü�� ���ֹ�����.
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
            // �ƹ��͵� �浹���� �ʾ��� ��� (delete�� �ϴ°� ������? �׳� -1X-1�ϴ°� ������?)
            // �ƴ� delete�� ������ �ؾߵ�, �׷��� col ������ ������
            // �׸��� distance�� null ������ �ٲٸ� �ɵ�
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

    private bool startStatus = false;
    private bool finalStatus = false;

    // �浹 ����
    // tag -> Fiber
    private bool collisionState = false;

    // �浹 ������Ʈ

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
            else if (collisionFiber.Length <= 0) collisionFiber = "NONE"; // �ϴ� ���⼭ ���� �߻��ϱ� ��

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


                collisionFiber = arbiContact + " " + arbiSegment; // ���Ⱑ �� ȣ���� �� �ȵɱ�? // �浹 �����ϱ�?
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