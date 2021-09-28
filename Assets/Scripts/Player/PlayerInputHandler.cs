using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerControls inputActions;
    Vector2 movementInput;
    Vector2 cameraInput;

    public float horizontal;
    public float vertical;
    public float mouseX;
    public float mouseY;
    public float moveAmount;
    float rollInputTimer;
    float jumpInputTimer;
    public bool sprintFlag;
    public bool jumpFlag;
    public bool rollFlag;
    public bool b_input;//xbox East button or B
    public bool a_input;//xbox South button or A



    private void Awake()
    {
        inputActions = new PlayerControls();
        inputActions.Movements.Move.performed += (i) => movementInput = i.ReadValue<Vector2>();
        inputActions.Movements.Camera.performed += (i) => cameraInput = i.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Tick input
    private void Update()
    {
        MoveInput();
        RollInput(Time.deltaTime);
        JumpInput(Time.deltaTime);
    }

    private void LateUpdate()
    {
        rollFlag = false;
        jumpFlag = false;
    }

    private void MoveInput()
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void RollInput(float delta)
    {
        b_input = inputActions.Actions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        if (b_input)
        {
            rollInputTimer += delta;
            sprintFlag = true;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
                rollFlag = true;
            }
            rollInputTimer = 0;
        }
    }

    private void JumpInput(float delta)
    {
        a_input = inputActions.Actions.Jump.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        if (a_input)
        {
            jumpInputTimer += delta;
        }
        else
        {
            if (jumpInputTimer > 0 && jumpInputTimer < 0.5f)
            {
                jumpFlag = true;
            }
            jumpInputTimer = 0;
        }
    }
}
