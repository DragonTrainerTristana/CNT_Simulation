using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ONE_CNT : MonoBehaviour
{
    public GameObject prefabCnt; 
    
    public int numCnt;

    // 부모 CNT 배열
    private GameObject[] parentCnt;
    // CNT 생성 범위
    private float[] xmin, xmax, ymin, ymax, zmin, zmax;

    void Start()
    {
        //배열 크기 할당
        xmin = new float[6]; xmax = new float[6];
        ymin = new float[6]; ymax = new float[6];
        zmin = new float[6]; zmax = new float[6];
        parentCnt = new GameObject[6];

        //배열 변수 값 할당
        SetParentCnt();
        SetArrayRange();

        //오브젝트 생성
        GenerateCNT();

        /*
        for (int i = 0; i < numCnt; i++) {

            GameObject cnt = Instantiate(prefabCnt) as GameObject;
            float px = Random.Range(xmin[0], xmax[0]);
            float py = Random.Range(ymin, ymax);
            float pz = Random.Range(zmin, zmax);
            cnt.transform.position = new Vector3(px, py, pz);
            cnt.name = "cnt" + i;
            cnt.transform.SetParent(parentCnt_ONE.transform);

        }*/
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
    }

    void GenerateCNT() {
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < numCnt; j++) {
                
                float px = Random.Range(xmin[i], xmax[i]);
                float py = Random.Range(ymin[i], ymax[i]);
                float pz = Random.Range(zmin[i], zmax[i]);
                GameObject cnt = Instantiate(prefabCnt) as GameObject; 
                cnt.transform.position = new Vector3(px, py, pz);
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
