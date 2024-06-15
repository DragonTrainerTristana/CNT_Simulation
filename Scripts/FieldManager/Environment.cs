using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{

    private float fiberCollisionCount;
    public bool fieldCollisionState;

    void Start()
    {
        fiberCollisionCount = 0;
        fieldCollisionState = false;
    }

    // Update is called once per frame
    void Update()
    {
        fiberCollisionCount += Time.deltaTime;
        if(fiberCollisionCount >= 3.0f)
        {
            fieldCollisionState = true;
        }
    }
}
