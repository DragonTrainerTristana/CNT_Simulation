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

    // �浹 ����
    // tag -> Fiber
    private bool collisionState = false;

    // �浹 ������Ʈ
    void OnCollisionEnter(Collision col)
    {
        
    }

}