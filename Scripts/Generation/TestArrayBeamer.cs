using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestArrayBeammer : MonoBehaviour
{
    public GameObject myEpoxy;
    public GameObject parentCnt;
    public GameObject prefabCntBeamer;
    private BoxCollider myEpoxyCollider;
    public int numCnt;
    public GameObject[] cntArray; 

    public void Start()
    {
        myEpoxyCollider = myEpoxy.GetComponent<BoxCollider>();
        cntArray = new GameObject[numCnt];
    }

    Vector3 GenerateRandomLocalPosition(float rangeX, float rangeY, float rangeZ)
    {
        float randomX = Random.Range(-rangeX / 2, rangeX / 2);
        float randomY = Random.Range(-rangeY / 2, rangeY / 2);
        float randomZ = Random.Range(-rangeZ / 2, rangeZ / 2);

        return new Vector3(randomX, randomY, randomZ);
    }

    public void GenerateCNT()
    {

        Vector3 localSize = myEpoxyCollider.size;

        for (int i = 0; i < numCnt; i++)
        {
            float rx = Random.Range(0f, 360f);
            float ry = Random.Range(0f, 360f);
            float rz = Random.Range(0f, 360f);

            Vector3 randomLocalPosition = GenerateRandomLocalPosition(localSize.x, localSize.y, localSize.z);

            Vector3 worldPosition = myEpoxy.transform.TransformPoint(randomLocalPosition);

            GameObject cnt = Instantiate(prefabCntBeamer, worldPosition, Quaternion.Euler(rx, ry, rz));

            cnt.transform.SetParent(parentCnt.transform);
            cnt.name = i.ToString();
            cntArray[i] = cnt;
            
            cntArray[i].GetComponent<Beamer>().myParentNum = i;
            
        }
    }

}
