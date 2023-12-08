using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holster : MonoBehaviour
{

    public GameObject centerEyeAnchor;
    private float rotationSpeed = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(centerEyeAnchor.transform.position.x, centerEyeAnchor.transform.position.y/2, centerEyeAnchor.transform.position.z);
        //var rotationDifference = Math.Abs(centerEyeAnchor.transform.eulerAngles.y - transform.eulerAngles.y);
        var finalRotationSpeed = rotationSpeed;

        //var step = finalRotationSpeed * Time.deltaTime;
        var step = finalRotationSpeed * 0.9;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, centerEyeAnchor.transform.eulerAngles.y, 0), (float)step);
    }
}
