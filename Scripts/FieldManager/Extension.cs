using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Extension : MonoBehaviour
{
    public GameObject cubeEdge;
    public GameObject paddingObject;
    private Vector3 initialScalePadding; // 패딩 오브젝트의 초기 스케일 저장 변수
    private Vector3 initialScaleCube;    // 큐브의 초기 스케일 저장 변수

    private float timeCount;
    private float timeCount2;
    private bool cubeStretch = true;

    private float volume;                // 큐브의 초기 부피
    private float bottom;                // 큐브의 바닥 면적 계산을 위한 변수
    private float height;                // 큐브의 높이 계산을 위한 변수

    void Start()
    {
        initialScaleCube = cubeEdge.transform.localScale;  // 큐브의 초기 스케일을 저장
        initialScalePadding = paddingObject.transform.localScale; // 패딩 오브젝트의 초기 스케일을 저장
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

                // 큐브의 가로 길이 증가
                cubeEdge.transform.localScale += new Vector3(0.05f, 0f, 0f);

                // 새로운 바닥 면적 계산
                bottom = cubeEdge.transform.localScale.x * cubeEdge.transform.localScale.z;

                // 높이 계산
                height = volume / bottom;

                // 큐브 스케일 업데이트
                cubeEdge.transform.localScale = new Vector3(cubeEdge.transform.localScale.x, height, cubeEdge.transform.localScale.z);

                // 패딩 오브젝트 스케일 조정
                float scaleRatioX = cubeEdge.transform.localScale.x / initialScaleCube.x;
                float scaleRatioY = height / initialScaleCube.y;
                float scaleRatioZ = cubeEdge.transform.localScale.z / initialScaleCube.z;
                paddingObject.transform.localScale = new Vector3(
                    initialScalePadding.x * scaleRatioX,
                    initialScalePadding.y * scaleRatioY,
                    initialScalePadding.z * scaleRatioZ);

                // 큐브 높이가 0.1 이하가 되면 스트레치 중단
                if (height <= 0.1f)
                {
                    cubeStretch = false;
                }
            }
        }
    }
}
