using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachSegment : MonoBehaviour
{
    private HashSet<string> sameFiberSegments = new HashSet<string>();
    private HashSet<string> uniqueCollisions = new HashSet<string>();

    // 할당하기 orignal.cs(generateSegment)
    public string parentNode; // 받음
    public int segmentNum; // 받음

    public Vector3 initialPos; // 받음
    public Vector3 collisionPos; // 해야함 (했음)
    public string collisionFiber; // 자기 제외 충돌 된 Fiber
    public string finalFiber;
    public string sFiber;
    public string fFiber;
    private string arbiContact;
    private string arbiSegment;

    public bool collState = false;
    public bool startStatus = false;
    public bool finalStatus = false;

    public bool parentAlive = false;
    public bool childAlive = false;
    public bool deleteStatus = false;

    public int uniqueCollCount;
    public bool yeah = false;

    public float deleteTime = 0.0f;
    public float triggerTime = 0.0f;
    public GameObject environment;

    public void Start()
    {
        collState = false;
        Application.targetFrameRate = -1;
    }

    public void Update()
    {
        deleteTime += Time.deltaTime;
        triggerTime += Time.deltaTime;

        if (triggerTime >= 1.6f) triggerTime = 0.0f;
        if (deleteTime >= 3.0f)
        {
            deleteTime = 0.0f;
            ByeByeBye();
            // Initialize
            this.enabled = false; // 스크립트 비활성화
            this.enabled = true;  // 스크립트 재활성화 (OnEnable, Start 호출됨)
        }
    }
    private void ByeByeBye()
    {

        if (childAlive == true && parentAlive == true)
        {
            return;
        }
        if (childAlive != true)
        {
            if (collState == true)
            {
                if (uniqueCollCount >= 2) { }
                if (uniqueCollCount == 1)
                {
                    if (startStatus == true || finalStatus == true) { }
                    else if (parentAlive != true) Destroy(this.gameObject);
                }
            }
            else if ((startStatus == true || finalStatus == true) && parentAlive == true) { }//무시
            else
            {
                Destroy(this.gameObject);
            }
        }

        if (parentAlive != true)
        {
            if (collState == true)
            {
                if (uniqueCollCount >= 2) { }
                if (uniqueCollCount == 1)
                {
                    if (startStatus == true || finalStatus == true) { }//무시
                    else if (childAlive != true) Destroy(this.gameObject);
                }
            }
            else if ((startStatus == true || finalStatus == true) && childAlive == true) { } // 무시
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (triggerTime >= 1.5f)
        {
            if (other.gameObject.CompareTag("Start")) startStatus = true;
            if (other.gameObject.CompareTag("Final")) finalStatus = true;
            if (other.CompareTag("Fiber"))
            {
                arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
                arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
                string _collisionFiber = arbiContact + "X" + arbiSegment;
                // 같은 fiber
                if (arbiContact == parentNode)
                {
                    if (!sameFiberSegments.Contains(arbiSegment))
                    {
                        sameFiberSegments.Add(arbiSegment);

                        if (segmentNum > int.Parse(arbiSegment))
                        {
                            parentAlive = true;
                        }
                        else if (segmentNum < int.Parse(arbiSegment))
                        {
                            childAlive = true;
                        }
                    }
                }
                // 다른 fiber
                if (arbiContact != parentNode)
                {
                    collState = true;
                    if (!uniqueCollisions.Contains(_collisionFiber)) uniqueCollisions.Add(_collisionFiber);
                    uniqueCollCount = uniqueCollisions.Count;
                }
            }
        }
    }
}

