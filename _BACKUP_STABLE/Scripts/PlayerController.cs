using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementState
{
    Idle,
    Crouching,
    Walking,
    Sprinting,
    InAir
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float sprintSpeed = 6.5f;
    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float jumpForce = 8.0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.15f);
    [SerializeField] private LayerMask groundLayer = ~0;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Noise & Stealth (Alien Detection)")]
    [SerializeField] private float idleNoise = 0f;
    [SerializeField] private float crouchNoise = 1.0f;
    [SerializeField] private float walkNoise = 3.0f;
    [SerializeField] private float sprintNoise = 7.0f;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isSprinting;
    private bool isCrouching;
    private bool jumpRequested;
    private bool isGrounded;

    public MovementState CurrentState { get; private set; } = MovementState.Idle;
    public float CurrentNoiseLevel { get; private set; } = 0f;
    public bool IsGrounded => isGrounded;
    public bool IsFacingRight { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        HandleInput();
        UpdateFacingDirection();
        UpdateStateAndNoise();
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyMovement();
    }

    private void HandleInput()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        horizontalInput = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontalInput -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontalInput += 1f;

        isCrouching = kb.leftCtrlKey.isPressed || kb.cKey.isPressed || kb.sKey.isPressed || kb.downArrowKey.isPressed;
        isSprinting = (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed) && !isCrouching;

        if (kb.spaceKey.wasPressedThisFrame && isGrounded && !isCrouching)
        {
            jumpRequested = true;
        }
    }

    private void CheckGround()
    {
        Vector2 checkPos = groundCheckPoint != null ? (Vector2)groundCheckPoint.position : (Vector2)transform.position + Vector2.down * 0.5f;
        Collider2D hit = Physics2D.OverlapBox(checkPos, groundCheckSize, 0f, groundLayer);

        isGrounded = hit != null && hit.gameObject != gameObject;
    }

    private void ApplyMovement()
    {
        float targetSpeed;
        if (isCrouching)
        {
            targetSpeed = crouchSpeed;
        }
        else if (isSprinting && Mathf.Abs(horizontalInput) > 0.01f)
        {
            targetSpeed = sprintSpeed;
        }
        else
        {
            targetSpeed = walkSpeed;
        }

        float currentYVel = rb.linearVelocity.y;

        if (jumpRequested)
        {
            currentYVel = jumpForce;
            jumpRequested = false;
        }

        rb.linearVelocity = new Vector2(horizontalInput * targetSpeed, currentYVel);
    }

    private void UpdateFacingDirection()
    {
        if (Mathf.Abs(horizontalInput) > 0.05f)
        {
            IsFacingRight = horizontalInput > 0f;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !IsFacingRight;
            }
        }
    }

    private void UpdateStateAndNoise()
    {
        if (!isGrounded)
        {
            CurrentState = MovementState.InAir;
            CurrentNoiseLevel = walkNoise;
            return;
        }

        if (Mathf.Abs(horizontalInput) < 0.05f)
        {
            CurrentState = isCrouching ? MovementState.Crouching : MovementState.Idle;
            CurrentNoiseLevel = isCrouching ? crouchNoise * 0.5f : idleNoise;
        }
        else
        {
            if (isCrouching)
            {
                CurrentState = MovementState.Crouching;
                CurrentNoiseLevel = crouchNoise;
            }
            else if (isSprinting)
            {
                CurrentState = MovementState.Sprinting;
                CurrentNoiseLevel = sprintNoise;
            }
            else
            {
                CurrentState = MovementState.Walking;
                CurrentNoiseLevel = walkNoise;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 checkPos = groundCheckPoint != null ? (Vector2)groundCheckPoint.position : (Vector2)transform.position + Vector2.down * 0.5f;
        Gizmos.DrawWireCube(checkPos, groundCheckSize);
    }
}
