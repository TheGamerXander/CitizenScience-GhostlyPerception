using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private GhostGameManager gameManager;

    [SerializeField] public GameObject MistCanvas;

    // movment Variables
    [SerializeField] private CharacterController controller;
    [SerializeField] private float moveSpeed = 5f;

    // camera Variables
    [SerializeField] private float lookSensitivity = 130.0f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform camaraTransform;
    float xRotation = 1.3f;

    // gravity Variables
    private Vector3 playerVelocity;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundChecker;
    private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded = false;
    private bool isfalling = false;
    [SerializeField] private Material[] groundedMats;
    [SerializeField] private PlayerItemsManager itemsManager;

    private bool paused = false;
    private bool unPausing = false;

    private float restorePosSpeed = 2.0f;
    private float restoreRotSpeed = 2.5f;

    private Vector3 prePausePos;
    private Quaternion prePauseRot;

    private void Start()
    {
        camaraTransform.Rotate(new Vector3 (1.3f,0,0));      
    }

    // Update is called once per frame
    void Update()
    {
        GroundedCheck();
        if (!paused) {

            CalculateLookDirection();
            CalculateMovement();

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.PauseGame();
            }
        }
        else if (paused && unPausing) 
        { 
            if(RestorePosAndRotate())
            {
                unPausing = false;
                paused = false;
            }
        }

        ApplyGravity();
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    public bool PlayerPaused()
    {
        return paused;
    }

    public void PausePlayer()
    {
        gameManager.HideUIMenu();
        prePausePos = transform.position;
        prePauseRot = transform.rotation;
        prePauseRot = Quaternion.Euler(0, prePauseRot.eulerAngles.y, 0);
        transform.rotation = prePauseRot;
        itemsManager.FreeHands();
        UnlockMouse();
        paused = true;
    }

    public void UnpausePlayer()
    {
        gameManager.ShowUIMenu();   
        LockMouse();
        unPausing = true;
    }

    private void CalculateLookDirection()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

        // Apply vertical (up/down) rotation directly to the camera's local rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 55f); // Limit vertical rotation between -45f and 50 degrees
        camaraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Apply horizontal (left/right) rotation to the player body
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void CalculateMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && isfalling)
        {
            playerVelocity.y = -5f;
            controller.Move(playerVelocity * Time.deltaTime);
            isfalling = false;
        }
        else if (!isGrounded)
        {
            playerVelocity.y += gravity * Time.deltaTime;

            if (playerVelocity.y < -30)
            {
                playerVelocity.y = -30;
            }

            controller.Move(playerVelocity * Time.deltaTime);
            isfalling = true;
        }
    }

    private void GroundedCheck()
    {
        bool previousCheck = isGrounded;
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundMask);       
    }

    public void TriggerCaptureBeam(GameObject ghost)
    {
        itemsManager.TriggerCaptureBeam(ghost);
    }

    public Vector3 GetCamPos()
    {
        return camaraTransform.position;
    }

    private bool RestorePosAndRotate()
    {
        bool destinationReached = false;
        bool lookingAtTarget = false;

        float distanceToTarget = Vector3.Distance(transform.position, prePausePos);
        if (distanceToTarget < 0.4f)
        {
            destinationReached = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, prePausePos, restorePosSpeed * Time.deltaTime);
        }

        // Smoothly rotate towards the desired rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, prePauseRot, restoreRotSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, prePauseRot) < 1.0)
        {
            lookingAtTarget = true;
        }
        else
        {
            lookingAtTarget = false;
        }

        if (destinationReached && lookingAtTarget)
        {
            transform.rotation = prePauseRot;
            transform.position = prePausePos;

            return true;
        }

        return false;
    }
}
