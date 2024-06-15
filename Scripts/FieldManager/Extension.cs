using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Extension : MonoBehaviour
{
    public GameObject cubeEdge;
    public GameObject paddingObject;
    private Vector3 initialScalePadding; // �е� ������Ʈ�� �ʱ� ������ ���� ����
    private Vector3 initialScaleCube;    // ť���� �ʱ� ������ ���� ����

    private float timeCount;
    private float timeCount2;
    private bool cubeStretch = true;

    private float volume;                // ť���� �ʱ� ����
    private float bottom;                // ť���� �ٴ� ���� ����� ���� ����
    private float height;                // ť���� ���� ����� ���� ����

    void Start()
    {
        initialScaleCube = cubeEdge.transform.localScale;  // ť���� �ʱ� �������� ����
        initialScalePadding = paddingObject.transform.localScale; // �е� ������Ʈ�� �ʱ� �������� ����
        volume = initialScaleCube.x * initialScaleCube.y * initialScaleCube.z;

        timeCount = 0.0f;
        timeCount2 = 0.0f;
    }

    void Update()
    {
        timeCount += Time.deltaTime;
        timeCount2 += Time.deltaTime;

        if (timeCount2 > 3.0)
        {
            if (timeCount > 0.3f && cubeStretch)
            {
                timeCount = 0.0f;

                // ť���� ���� ���� ����
                cubeEdge.transform.localScale += new Vector3(0.05f, 0f, 0f);

                // ���ο� �ٴ� ���� ���
                bottom = cubeEdge.transform.localScale.x * cubeEdge.transform.localScale.z;

                // ���� ���
                height = volume / bottom;

                // ť�� ������ ������Ʈ
                cubeEdge.transform.localScale = new Vector3(cubeEdge.transform.localScale.x, height, cubeEdge.transform.localScale.z);

                // �е� ������Ʈ ������ ����
                float scaleRatioX = cubeEdge.transform.localScale.x / initialScaleCube.x;
                float scaleRatioY = height / initialScaleCube.y;
                float scaleRatioZ = cubeEdge.transform.localScale.z / initialScaleCube.z;
                paddingObject.transform.localScale = new Vector3(
                    initialScalePadding.x * scaleRatioX,
                    initialScalePadding.y * scaleRatioY,
                    initialScalePadding.z * scaleRatioZ);

                // ť�� ���̰� 0.1 ���ϰ� �Ǹ� ��Ʈ��ġ �ߴ�
                if (height <= 0.1f)
                {
                    cubeStretch = false;
                }
            }
        }
    }
}
