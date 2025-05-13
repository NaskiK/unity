using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("Wall Jump")]
    public float wallCheckDistance = 1f;
    public LayerMask wallLayer;
    public float wallJumpForce = 8f;
    public float wallJumpPush = 5f;
    public float wallJumpCooldown = 0.2f;

    [Header("Camera Tilt")]
    public float tiltAmount = 10f;
    public float tiltSpeed = 5f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    private bool isTouchingWall = false;
    private Vector3 wallNormal;
    private float lastWallJumpTime = -999f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckWall();

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jump and Wall Jump
        if (Input.GetButtonDown("Jump") && canMove)
        {
            if (characterController.isGrounded)
            {
                moveDirection.y = jumpPower;
            }
            else if (isTouchingWall && Time.time > lastWallJumpTime + wallJumpCooldown)
            {
                moveDirection.y = wallJumpForce;
                moveDirection += wallNormal * wallJumpPush;
                lastWallJumpTime = Time.time;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Crouch
        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            // Mouse Look (Y-axis)
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            // Movement-based tilt
            float horizontalInput = Input.GetAxis("Horizontal");
            float movementTiltZ = Mathf.Abs(horizontalInput) > 0.1f ? -horizontalInput * tiltAmount : 0f;

            // Wall jump tilt override
            if (Time.time < lastWallJumpTime + 0.2f)
            {
                movementTiltZ = -Vector3.Dot(wallNormal, transform.right) * tiltAmount * 2f;
            }

            // Apply look (X) and tilt (Z)
            Quaternion targetRotation = Quaternion.Euler(rotationX, 0, movementTiltZ);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);

            // Mouse Look (X-axis) for the player body
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void CheckWall()
    {
        isTouchingWall = false;
        wallNormal = Vector3.zero;

        RaycastHit hit;
        Vector3[] directions = { transform.right, -transform.right, transform.forward, -transform.forward };

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out hit, wallCheckDistance, wallLayer))
            {
                isTouchingWall = true;
                wallNormal = hit.normal;
                break;
            }
        }
    }
}
