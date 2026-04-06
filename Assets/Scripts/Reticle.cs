using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float maxAngle = 90f;
    public bool isRotating = true;
    public bool clockwise = true;
    void Start()
    {
        
    }

    public void rotating()
    {
        if (isRotating && !clockwise)
        {
            transform.Rotate(0,0,rotationSpeed*Time.deltaTime);
        }
        if (isRotating && clockwise)
        {
            transform.Rotate(0,0,-rotationSpeed*Time.deltaTime);
        }
    }
    public void checkPos()
    {
        float angle = transform.eulerAngles.z;
        if (angle > 180f) { angle -= 360f; } // keep angle in bounds

        if (angle >= maxAngle)
        {
            clockwise = true;
        }
        else if (angle <= -maxAngle)
        {
            clockwise = false;
        }


    }

    public void stopRotating()
    {
        isRotating = false;
    }
    public void startRotating()
    {
        isRotating = true;
    }
    public Vector2 getFireDir()
    {
        return transform.up;
    } //used to obtain directions for ball to travel 

    void Update()
    {
        checkPos();
        rotating();
    }
}
