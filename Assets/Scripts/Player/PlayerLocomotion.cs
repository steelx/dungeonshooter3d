using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public Vector3 moveDirection;
    PlayerInputHandler inputHandler;
    Transform cameraObject;
    AnimatorHandler animatorHandler;
    [HideInInspector]
    public CharacterController chController;

    [Header("Fall Detections Stats")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector3 playerVelocity;
    float inAirTimer;


    [Header("Movement Stats")]
    const float walkingSpeed = 5f;
    const float sprintSpeed = 9.81f;
    const float rotationSpeed = 10;
    const float gravityValue = -14.81f;
    const float jumpHeight = 15f;

    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInAir;
    public bool isJumping;

    private void Awake()
    {
        chController = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
    }

    private void Update()
    {
        HandleMovement(Time.deltaTime);
        HandleRollingAndSprint(Time.deltaTime);
        HandleFallingAndLanding(Time.deltaTime);
        HandleGravity(Time.deltaTime);
    }
    private void LateUpdate()
    {
        UpdateInAirTimer(Time.deltaTime);
    }

    void HandleRotation(float delta)
    {
        Vector3 targetDir = cameraObject.forward * inputHandler.vertical;
        targetDir += cameraObject.right * inputHandler.horizontal;
        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = chController.transform.forward;
        }

        Quaternion rotation = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(chController.transform.rotation, rotation, rotationSpeed * delta);

        chController.transform.rotation = targetRotation;
    }

    void HandleGravity(float delta)
    {
        // Gravity
        if (inputHandler.jumpFlag && chController.isGrounded)
        {
            playerVelocity.y += jumpHeight;
        }
        if (chController.isGrounded)
        {
            playerVelocity.y = 0;
        }
        else
        {
            playerVelocity.y += gravityValue * delta;
        }
        chController.Move(playerVelocity * delta);
    }
    void UpdateInAirTimer(float delta)
    {
        if (isInAir)
        {
            inAirTimer += delta;
        }
    }

    void HandleMovement(float delta)
    {
        if (inputHandler.rollFlag) return;

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0; // FIX walking on air due to camera

        float speed = walkingSpeed;
        if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5)
        {
            speed = sprintSpeed;
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
        moveDirection *= speed;

        chController.Move(delta * moveDirection);

        // Animation
        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, isSprinting);
        if (animatorHandler.CanRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollingAndSprint(float delta)
    {
        if (animatorHandler.IsInteracting) return;

        if (inputHandler.rollFlag)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;

            if (inputHandler.moveAmount > 0)
            {
                isSprinting = true;//so to make it roll faster
                animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                chController.transform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    void HandleFallingAndLanding(float delta)
    {
        Vector3 origin = chController.transform.position;
        //Debug.DrawRay(origin, Vector3.down, Color.red);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit _, 1f, groundLayer))
        {
            // if were inAir during detection
            if (isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    Debug.LogWarning("You were in air for: " + inAirTimer);
                    animatorHandler.PlayTargetAnimation("Land", true);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Empty", false);
                }
                inAirTimer = 0;
                isInAir = false;
            }
        }
        else
        {
            if (!isInAir)
            {
                if (!animatorHandler.IsInteracting)
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }

                isInAir = true;
            }
        }
    }

}
