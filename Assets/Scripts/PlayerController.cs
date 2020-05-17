using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float movementSpeed = 1f;
    public CharacterController characterController;
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Vector3 movementDirection;

    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        movementDirection = new Vector3(0, movementDirection.y, 0);

        characterController.Move(movementDirection * Time.deltaTime);

        if (Input.GetKey("w"))
        {
            transform.Translate(0, 0, Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(0, 0, -1f * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKey("a"))
        {
            transform.Translate(-1f * Time.deltaTime * movementSpeed, 0, 0);
        }
        if (Input.GetKey("d"))
        {
            transform.Translate(Time.deltaTime * movementSpeed, 0, 0);
        }
    }
}
