using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public GameObject Prefab_Dot;
    public int nDots;
    [Space(10f)]
    [Header("Space")]
    public int xmin;
    public int xmax;
    public int ymin;
    public int ymax;
    public int zmin;
    public int zmax;
    private static GameObject Dots;
    // Start is called before the first frame update
    void Start()
    {
        Dots = new GameObject("Dots");
        for (int i = 0; i < nDots; i++)
        {
            GameObject dot = Instantiate(Prefab_Dot) as GameObject;
            int px = Random.Range(xmin, xmax);
            int py = Random.Range(ymin, ymax);
            int pz = Random.Range(zmin, zmax);
            dot.transform.position = new Vector3(px, py, pz);
            dot.name = "Dot" + i;
            dot.transform.SetParent(Dots.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
