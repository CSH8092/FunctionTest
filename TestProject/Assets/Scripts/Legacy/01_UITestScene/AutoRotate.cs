using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public bool yAxis = false;
    public bool xAxis = false;
    public bool zAxis = false;

    Transform thisTransform;
    float speed = 50f;

    void Start()
    {
        thisTransform = this.transform;
    }

    void Update()
    {
        if (yAxis)
        {
            thisTransform.Rotate(Vector3.up, Time.deltaTime * speed);
        }
        else if (xAxis)
        {
            thisTransform.Rotate(Vector3.right, Time.deltaTime * speed);
        }
        else
        {
            thisTransform.Rotate(Vector3.forward, Time.deltaTime * speed);
        }
    }
}
