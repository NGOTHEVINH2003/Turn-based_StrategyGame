using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    //zoom value
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 10f;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomIncreaseAmount = 1f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private CinemachineVirtualCamera CinemachineVirtualCamera;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffSet;
    private void Start()
    {
        cinemachineTransposer = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffSet = cinemachineTransposer.m_FollowOffset;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }


    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
        //Vector 2 -> Vector 3 set z=y in inputMoveDir.
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;

    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);
        
        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;

        CinemachineTransposer cinemachineTransposer = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();

    }

    private void HandleZoom()
    {
        targetFollowOffSet.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;
        targetFollowOffSet.y = Mathf.Clamp(targetFollowOffSet.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffSet, Time.deltaTime * zoomSpeed);
    }
}
