using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachSegment : MonoBehaviour
{
    private HashSet<string> sameFiberSegments = new HashSet<string>();
    private HashSet<string> uniqueCollisions = new HashSet<string>();

    // GameObject
    public GameObject UIManagement;

    // 할당하기 orignal.cs(generateSegment)
    public string parentNode; // 상속
    public int segmentNum; // 상속

    public Vector3 initialPos; // 상속
    public Vector3 collisionPos; // 해야함 (상속)
    public string collisionFiber; // 자기 외의 충돌 한 Fiber
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
    
    // 물리 컴포넌트 캐싱 (제거 전 참조 유지를 위해)
    private Rigidbody myRigidbody;
    private Collider myCollider;
    
    // 충돌 데이터 저장 완료 플래그
    private bool collisionDataCollected = false;

    public void Start()
    {
        collState = false;
        Application.targetFrameRate = -1;
        
        // 물리 컴포넌트 캐싱
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        
        // 일정 시간 후 물리 비활성화 코루틴 시작
        StartCoroutine(DisablePhysicsAfterDelay());
    }

    public void Update()
    {
        triggerTime += Time.deltaTime;
        
        // 기존 코드에서 physics 관련 처리를 코루틴으로 대체
        if(triggerTime >= 2.2f && !collisionDataCollected)
        {
            // 충돌 데이터 수집 완료 표시
            collisionDataCollected = true;
            
            // 렌더러 상태 설정 (필요한 경우에만 활성화)
            if (collState || startStatus || finalStatus)
            {
                // 중요한 세그먼트는 보이게 유지
                GetComponent<Renderer>().enabled = true;
            }
            else
            {
                // 중요하지 않은 세그먼트는 숨김
                GetComponent<Renderer>().enabled = false;
            }
        }
    }
    
    // 일정 시간 후 물리 시스템 비활성화
    private IEnumerator DisablePhysicsAfterDelay()
    {
        // 충돌 데이터 수집에 필요한 시간(2.2초) 동안 대기
        yield return new WaitForSeconds(2.2f);
        
        // 물리 컴포넌트 비활성화 및 제거
        DisablePhysics();
    }
    
    // 물리 컴포넌트 완전 제거 메소드
    public void DisablePhysics()
    {
        if (gameObject != null)
        {
            // 먼저 물리 상호작용을 끄고
            if (myRigidbody != null)
            {
                myRigidbody.isKinematic = true;
                myRigidbody.detectCollisions = false;
            }
            
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }
            
            // 충돌 데이터가 모두 수집되었는지 확인
            if (collisionDataCollected)
            {
                // 물리 컴포넌트 완전 제거
                if (myRigidbody != null) Destroy(myRigidbody);
                if (myCollider != null) Destroy(myCollider);
                
                // 참조 정리
                myRigidbody = null;
                myCollider = null;
            }
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
            else if ((startStatus == true || finalStatus == true) && parentAlive == true) { }
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
                    if (startStatus == true || finalStatus == true) { }
                    else if (childAlive != true) Destroy(this.gameObject);
                }
            }
            else if ((startStatus == true || finalStatus == true) && childAlive == true) { } // 수정
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 충돌 데이터 수집 기간 동안만 처리
        if (triggerTime <= 2.0f)
        {
            if (other.gameObject.CompareTag("Start")) startStatus = true;
            if (other.gameObject.CompareTag("Final")) finalStatus = true;
            if (other.CompareTag("Fiber"))
            {
                arbiContact = other.gameObject.GetComponent<EachSegment>().parentNode;
                arbiSegment = other.gameObject.GetComponent<EachSegment>().segmentNum.ToString();
                //string _collisionFiber = arbiContact + "X" + arbiSegment;
                string _collisionFiber = arbiContact;

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
                if (arbiContact != parentNode)
                {
                    this.gameObject.GetComponent<Renderer>().material.color = Color.red;
                    collState = true;
                    if (!uniqueCollisions.Contains(_collisionFiber))
                    {
                        uniqueCollisions.Add(_collisionFiber);
                        uniqueCollCount = uniqueCollisions.Count;

                        if (collisionFiber.Length > 0)
                        {
                            collisionFiber += ",";
                        }
                        collisionFiber += arbiContact.ToString();
                    }
                }
            }
        }
        // 충돌 처리가 완료되면 추가 충돌 감지 방지
        else if (myCollider != null && myCollider.enabled)
        {
            myCollider.enabled = false;
        }
    }
}
