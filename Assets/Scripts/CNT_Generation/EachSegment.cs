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
    public Vector3 collisionPos; // 해야함

    public string collisionFiber; // 자기 제외 충돌 된 Fiber

    // 충돌 변수
    // tag -> Fiber
    private bool collisionState = false;

    // 충돌 오브젝트
    void OnCollisionEnter(Collision col)
    {
        
    }

}