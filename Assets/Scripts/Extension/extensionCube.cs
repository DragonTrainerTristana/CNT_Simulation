using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class extensionCube : MonoBehaviour
{
    public GameObject cubeEdge;
    public GameObject[] paddingObject;
    private Vector3[] originalVec;

    // stretch
    private float timeCount;
    private float timeCount2;
    private bool deadCount;
    private float alter;
    private float bottom;
    private float height;
    private float volume;
    private bool cubeStretch = true;
    private bool okay = false;

    // padding
    private float dividedX;
    private float dividedY;
    private float dividedZ;
    public int numObj;


    void Start()
    {
        
        originalVec = new Vector3[numObj];
        for (int i = 0; i < numObj; i++)
        {
            //Debug.Log(paddingObject[i].transform.localPosition);
            originalVec[i] = new Vector3(paddingObject[i].transform.localPosition.x,
            paddingObject[i].transform.localPosition.y,
            paddingObject[i].transform.localPosition.z);
        }

        alter = 0.0f;
        bottom = 0.0f;
        dividedX = 0.0f;
        dividedY = 0.0f;
        dividedZ = 0.0f;
        volume = cubeEdge.transform.localScale.x * cubeEdge.transform.localScale.z * cubeEdge.transform.localScale.y;

        //Initialize
        timeCount = 0.0f;
        timeCount2 = 0.0f;
        deadCount = false;
    }

    void Update()
    {


        timeCount += Time.deltaTime;
        timeCount2 += Time.deltaTime;

        if (timeCount2 > 3.0)
        {

            if (timeCount > 0.3f && cubeStretch == true)
            {
                timeCount = 0.0f;

                cubeEdge.transform.localScale += new Vector3(0.05f, 0f, 0f);
                bottom = cubeEdge.transform.localScale.x * cubeEdge.transform.localScale.z;
                height = volume / bottom;
                cubeEdge.transform.localScale = new Vector3(cubeEdge.transform.localScale.x, height, cubeEdge.transform.localScale.z);

                alter = bottom * height;

                Debug.Log("Volume " + alter);
                Debug.Log("Width " + bottom);
                Debug.Log("Height " + height);

                dividedX = cubeEdge.transform.localScale.x / 2;
                dividedY = cubeEdge.transform.localScale.y / 2;
                dividedZ = cubeEdge.transform.localScale.z / 2;

                for (int i = 0; i < numObj; i++)
                {
                    paddingObject[i].transform.localPosition = new Vector3(dividedX * originalVec[i].x, dividedY * originalVec[i].y, 0.0f);
                }


                if (cubeEdge.transform.localScale.y - 0.1f <= 0.0f)
                {
                    cubeStretch = false;
                }
            }
        }

    }
}
