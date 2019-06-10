using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float rotationX=0, rotationY=0;
    private void Update()
    {
        float moveY = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            moveY = 1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            moveY = -1f;
        }
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveY, Input.GetAxis("Vertical"));
        transform.Translate(moveDirection*0.1f);
        if (Input.GetMouseButton(0)) {
            rotationX += Input.GetAxis("Mouse Y");
            if (rotationX >= 30)
            {
                rotationX = 30;
            } else if (rotationX <= -30)
            {
                rotationX = -30;
            }
            rotationY += Input.GetAxis("Mouse X");
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
    }
}
