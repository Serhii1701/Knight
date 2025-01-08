using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    readonly float distanceZ = 5;
    //float distanceY = 2;

    [SerializeField] Transform followTarget;
    [SerializeField] Vector2 framingOffset;
    PlayerController playerController;
    Vector3 cameraOffset;
    
    void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraOffset = new Vector3(0, 0, distanceZ);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        var targetRotation = Quaternion.Euler(playerController.RotationX, playerController.RotationY, 0);
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);
        transform.SetPositionAndRotation(focusPosition - targetRotation * cameraOffset, targetRotation);
        //transform.position = focusPosition - targetRotation * cameraOffset;
        //transform.rotation = targetRotation;
    }
}
