using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        // If the game isn't paused
        if (Time.timeScale != 0)
        {
            // Gets the mouse movement since last frame
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Rotates the camera around the y axis (up and down) and binds it to 90deg both ways
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Rotates the player around the x axis (left and right)
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
