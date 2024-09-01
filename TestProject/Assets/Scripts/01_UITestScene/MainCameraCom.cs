using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraCom : MonoBehaviour
{
    public Transform mainCam;

    void Start()
    {
        
    }


    void LateUpdate()
    {
        RotateCam();
    }

    void RotateCam()
    {
        mainCam.Rotate(Vector3.up, Time.deltaTime);
    }
}
