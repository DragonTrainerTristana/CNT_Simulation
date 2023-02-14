using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ONE_CNT : MonoBehaviour
{
    public GameObject prefabCnt; 
    
    public int numCnt = 100;
    int cntNameNum;

    public bool startState = false;
    public bool startOnce = true;

    // 부모 CNT 배열
    private GameObject[] parentCnt;
    public GameObject[] cntArray;
    // CNT 생성 범위
    private float[] xmin, xmax, ymin, ymax, zmin, zmax;
    private float[] rotationminX, rotationminY, rotationminZ;
    private float[] rotationmaxX, rotationmaxY, rotationmaxZ;

    /*
    void Update()
    {
        //startState == true
        if (startOnce == true) {
            startOnce = false;
            startState = false;
            Initialize();

        }

    }*/

    public void Start()
    {

        cntNameNum = 0;

        //CNT 생성 배열 크기 할당
        xmin = new float[6]; xmax = new float[6];
        ymin = new float[6]; ymax = new float[6];
        zmin = new float[6]; zmax = new float[6];

        //CNT Rotation 배열 크기 할당
        rotationminX = new float[6]; rotationminY = new float[6]; rotationminZ = new float[6];
        rotationmaxX = new float[6]; rotationmaxY = new float[6]; rotationmaxZ = new float[6];

        //CNT 정육면체
        parentCnt = new GameObject[6];
        cntArray = new GameObject[numCnt * 6];

        //배열 변수 값 할당
        SetParentCnt();
        SetArrayRange();

        
        //오브젝트 생성
        GenerateCNT();
    }

    void SetParentCnt() { // 부모 오브젝트 할당 (정육면체 한 면)
        parentCnt[0] = GameObject.Find("CNT_<ONE>");
        parentCnt[1] = GameObject.Find("CNT_<TWO>");
        parentCnt[2] = GameObject.Find("CNT_<THREE>");
        parentCnt[3] = GameObject.Find("CNT_<FOUR>");
        parentCnt[4] = GameObject.Find("CNT_<FIVE>");
        parentCnt[5] = GameObject.Find("CNT_<SIX>");
    }

    void SetArrayRange() {
        //CNT_ONE Range
        xmin[0] = 0.0f; xmax[0] = 20.5f;
        ymin[0] = 0.0f; ymax[0] = 20.5f;
        zmin[0] = 0.0f; zmax[0] = 0.0f;
        //CNT_TWO Range
        xmin[1] = 0.0f; xmax[1] = 20.5f;
        ymin[1] = 0.0f; ymax[1] = 20.5f;
        zmin[1] = 20.5f; zmax[1] = 20.5f;
        //CNT_THREE Range
        xmin[2] = 0.0f; xmax[2] = 20.5f;
        ymin[2] = 0.0f; ymax[2] = 0.0f;
        zmin[2] = 0.0f; zmax[2] = 20.5f;
        //CNT_FOUR Range
        xmin[3] = 0.0f; xmax[3] = 20.5f;
        ymin[3] = 20.5f; ymax[3] = 20.5f;
        zmin[3] = 0.0f; zmax[3] = 20.5f;
        //CNT_FIVE Range
        xmin[4] = 0.0f; xmax[4] = 0.0f;
        ymin[4] = 0.0f; ymax[4] = 20.5f;
        zmin[4] = 0.0f; zmax[4] = 20.5f;
        //CNT_SIX Range
        xmin[5] = 20.5f; xmax[5] = 20.5f;
        ymin[5] = 0.0f; ymax[5] = 20.5f;
        zmin[5] = 0.0f; zmax[5] = 20.5f;

        //CNT_ONE_Angle
        rotationminX[0] = -60.0f; rotationmaxX[0] = 60.0f;
        rotationminY[0] = -60.0f; rotationmaxY[0] = 60.0f;
        rotationminZ[0] = 0.0f; rotationmaxZ[0] = 0.0f;

        //CNT_TWO_Angle
        rotationminX[1] = -60.0f; rotationmaxX[1] = 60.0f;
        rotationminY[1] = 120.0f; rotationmaxY[1] = 240.0f;
        rotationminZ[1] = 0.0f; rotationmaxZ[1] = 0.0f;

        //CNT_THREE_Angle
        rotationminX[2] = -30.0f; rotationmaxX[2] = -150.0f;
        rotationminY[2] = 0.0f; rotationmaxY[2] = 0.0f;
        rotationminZ[2] = 0.0f; rotationmaxZ[2] = 0.0f;

      
        //CNT_FOUR_Angle
        rotationminX[3] = -30.0f+180.0f; rotationmaxX[3] = -150.0f+180.0f;
        rotationminY[3] = 0.0f; rotationmaxY[3] = 0.0f;
        rotationminZ[3] = 0.0f; rotationmaxZ[3] = 0.0f;
         
        //CNT_FIVE_Angle
        rotationminX[4] = -60.0f; rotationmaxX[4] = 60.0f;
        rotationminY[4] = 30.0f; rotationmaxY[4] = 150.0f;
        rotationminZ[4] = 0.0f; rotationmaxZ[4] = 0.0f;

        //CNT_SIX_Angle
        rotationminX[5] = 60.0f; rotationmaxX[5] = -60.0f;
        rotationminY[5] = -30.0f; rotationmaxY[5] = -150.0f;
        rotationminZ[5] = 0.0f; rotationmaxZ[5] = 0.0f;
      
    }

    void GenerateCNT() {
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < numCnt; j++) {
                
                float px = Random.Range(xmin[i], xmax[i]);
                float py = Random.Range(ymin[i], ymax[i]);
                float pz = Random.Range(zmin[i], zmax[i]);

                float rx = Random.Range(rotationminX[i], rotationmaxX[i]);
                float ry = Random.Range(rotationminY[i], rotationmaxY[i]);
                float rz = Random.Range(rotationminZ[i], rotationmaxZ[i]);

                GameObject cnt = Instantiate(prefabCnt) as GameObject;
                cntArray[cntNameNum] = cnt;
                cntNameNum++;
                

                cnt.transform.position = new Vector3(px, py, pz);

                //각 Segment의 Parent Node(여기서는 cnt.name)임, Beamer로 봐도 무관함
                //GenerateSegment.cs에서 Child Node인 Segment를 관리함 

                cnt.transform.localEulerAngles = new Vector3(rx, ry, rz);

                if (i == 0) cnt.name = "cnt_ONE" + j;    
                if (i == 1) cnt.name = "cnt_TWO" + j;
                if (i == 2) cnt.name = "cnt_THREE" + j;
                if (i == 3) cnt.name = "cnt_FOUR" + j;
                if (i == 4) cnt.name = "cnt_FIVE" + j;
                if (i == 5) cnt.name = "cnt_SIX" + j;
                cnt.transform.SetParent(parentCnt[i].transform);
                cnt.tag = "CNT";
            }
        }
     
    }
}
