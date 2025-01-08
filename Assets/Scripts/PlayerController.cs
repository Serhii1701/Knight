using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float RotationY { get; private set; }
    public float RotationX { get; private set; }
    readonly float rotationSpeed = 2;
    readonly float minVerticalAngle = 0;
    readonly float maxVerticalAngle = 45;

    public Vector3 TrackPlayerInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        RotationX += Input.GetAxis("Mouse Y") * rotationSpeed;
        RotationX = Mathf.Clamp(RotationX, minVerticalAngle, maxVerticalAngle);

        RotationY += Input.GetAxis("Mouse X") * rotationSpeed;

        var moveInput = new Vector3(horizontal, 0, vertical).normalized;
        var moveDirection = PlanarRotation() * moveInput;

        return moveDirection;
    }

    public Quaternion PlanarRotation()
    {
        return Quaternion.Euler(0, RotationY, 0);
    }

    public bool CheckOnWalk()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool CheckOnAttack()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    public bool CheckOnBlock()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }
}
