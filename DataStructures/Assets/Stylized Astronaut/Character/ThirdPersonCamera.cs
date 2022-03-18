using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -15.0f;
    private const float Y_ANGLE_MAX = 50.0f;

    public Transform lookAt;
    public Transform camTransform;
    public float distance = 20.0f;

    private float currentX = 0.0f;
    private float currentY = 45.0f;
    private float sensitivityX = 50.0f;
    private float sensitivityY = 50.0f;

    private void Start()
    {
        camTransform = transform;
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X");
        currentY += Input.GetAxis("Mouse Y");

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {
        float x = 0;
        Vector3 dir;
        if (currentY < -10)
            dir = new Vector3(0, 0, -distance + (Mathf.Abs(currentY)-10));
        else
            dir = new Vector3(0, 0, -distance);

        if(!SortManager.isView)
        {
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            camTransform.position = lookAt.position + rotation * dir;
            camTransform.LookAt(lookAt.position);
        }
        else
        {
            camTransform.position = new Vector3(0, 16, -82);
            camTransform.rotation = Quaternion.Euler(-4, 1, 0);
        }

    }
}
