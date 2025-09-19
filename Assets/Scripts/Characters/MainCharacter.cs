using UnityEngine;
using UnityEngine.InputSystem;
public class MainCharacter : MonoBehaviour, IControllable
{
    private Rigidbody2D rigidbody2D;
    private MarioActions marioActions;
    void Awake()
    {
        GameManager.Instance.RegisterControllable(this);
        marioActions = new MarioActions();
        marioActions.MarioMovement.Move.started += OnMovement;
        marioActions.MarioMovement.Move.canceled += OnMovement;
        marioActions.MarioMovement.Jump.started += OnJumpStarted;
        marioActions.MarioMovement.Jump.canceled += OnJumpCanceled;
        marioActions.MarioMovement.DashLeft.performed += OnDashLeft;
        marioActions.MarioMovement.DashLeft.canceled += OnDashLeft;
        marioActions.MarioMovement.DashRight.performed += OnDashRight;
        marioActions.MarioMovement.DashRight.canceled += OnDashRight;

    }
    void Start()
    {
        this.rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        ResetJumpsImmediate();
    }
    private void OnEnable()
    {
        marioActions?.MarioMovement.Enable();
    }

    private void OnDisable()
    {
        marioActions?.MarioMovement.Disable();
    }
    //Controls Section for controllable character
    private bool isInControl;
    private bool camerasettings_changed = false;
    [Header("Jump config")]
    public int maxJumps = 2;                 // total jumps available before needing to touch ground
    public float jumpForce = 7f;             // impulse applied when jumping
    public float minJumpInterval = 0.15f;    // minimum seconds between successive jumps
    public bool holdToRepeat = true;         // if true, holding the jump button will repeatedly jump at minJumpInterval

    [Header("Ground detection")]
    public LayerMask groundLayers;           // set which layers count as "ground"
    [Range(0.0f, 1.0f)]
    public float groundNormalThreshold = 0.65f; // required contact normal.y to be considered ground (useful for slopes)

    [Header("Movement config")]
    public float maxMoveSpeed;
    public float acceleration;
    public bool useCancelMech = true;
    public float velCancelAmt = 0.9f;
    public float initialImpulse = 5.0f;
    public float lowspeedThreshold = 1.0f;
    [Header("Dash")]
    public float dashForce = 12f;           // impulse applied for dash
    public float dashCooldown = 1f;         // minimum seconds between dashes
    public float dashUpAmount = 0.1f;
    public bool useDashCancelMech = true;
    public float dashCancelYAmt = 0.5f;

    [Header("Camera Params")]
    [SerializeField]
    private float cameraLerpAggression = 1.0f;
    [SerializeField]
    private Vector2 targetCharacterPosition = new(0.3f, 0.2f);
    [SerializeField]
    private float targetLookAheadRatio = 0.5f;
    [SerializeField]
    private float lookdirectionDistance = 1f;
    private Vector2 moveInput;
    private bool jumpHeld = false;
    private int jumpsRemaining;
    private float lastJumpTime = -999f;
    private float lastDashTime = -999f;
    private bool dashAvailable = true;
    private bool grounded = false;
    
    public void OnControlRemoved()
    {
        isInControl = false;
    }

    public bool OnTakeControl()
    {
        isInControl = true;
        return true;
    }

    public bool IsInControl()
    {
        return isInControl;
    }

    public Vector2 TargetCharacterPosition()
    {
        return targetCharacterPosition;
    }
    private Vector2 CharacterLookDirection = Vector2.right;
    public Vector2 TargetLookAhead()
    {
        if (rigidbody2D == null) return Vector2.zero;
        Vector2 bias_dir = Vector2.zero;
        if (rigidbody2D.linearVelocity.magnitude < lowspeedThreshold)
        {
            bias_dir = CharacterLookDirection.normalized;
        }
        return rigidbody2D.linearVelocity * targetLookAheadRatio + (bias_dir*lookdirectionDistance);
    }
    public Vector3 WorldPosition()
    {
        if (rigidbody2D == null) return Vector2.zero;
        return rigidbody2D.position;
    }

    public float Aggression()
    {
        return cameraLerpAggression;
    }

    public bool CameraSettingsChanged()
    {
        if (camerasettings_changed)
        {
            camerasettings_changed = false;
            return true;
        }
        return false;
    }
    private void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        CharacterLookDirection = moveInput.x > 0 ? Vector2.right : Vector2.left;
        //Debug.Log($"Moving: {moveInput}");
    }
    private float lastTapTime = 0f;
    private int tapCount = 0;
    [Header("Custom Bindings")]
    public float maxTapInterval = 0.25f; // max time allowed between taps
    private bool CheckDoubleTap()
    {
        float now = Time.time;

        if (now - lastTapTime <= maxTapInterval)
            tapCount++;
        else
            tapCount = 0; // reset if too slow

        lastTapTime = now;

        if (tapCount == 2) // double-tap detected
        {
            //Debug.Log("Double-tap!");
            tapCount = 0; // reset
            return true;
        }
        return false;
    }
    private void OnDashLeft(InputAction.CallbackContext context)
    {
        // performed = pressed, canceled = released
        if (context.performed)
        {
            if (CheckDoubleTap())
            {
                //Debug.Log($"Dash Left");
                TryDash(new Vector2(-1f, dashUpAmount).normalized);
            }
        }
        else if (context.canceled)
        {
            // reset dash availability when released
            dashAvailable = true;
        }
    }

    // called by your InputAction asset's DashRight binding
    private void OnDashRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CheckDoubleTap())
            {
                //Debug.Log($"Dash Right");
                TryDash(new Vector2(1f, dashUpAmount).normalized);
            }
        }
        else if (context.canceled)
        {
            dashAvailable = true;
        }
    }

    private void TryDash(Vector2 dir)
    {
        // check cooldown and availability
        if (!dashAvailable) return;
        if (Time.time - lastDashTime < dashCooldown) return;
        rigidbody2D.linearVelocityX = 0;
        if (useDashCancelMech)
        {
            rigidbody2D.linearVelocityY = rigidbody2D.linearVelocityY * Mathf.Clamp01(dashCancelYAmt * Time.fixedDeltaTime);
        }

        // add impulse dash
        rigidbody2D.AddForce(dir.normalized * dashForce, ForceMode2D.Impulse); //Maybe we add a little up later especially if we have sloped terrain

        lastDashTime = Time.time;
        dashAvailable = false;
    }
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (holdToRepeat && jumpHeld && Time.time - lastJumpTime >= minJumpInterval)
        {
            TryJump();
        }

        float currentX = rigidbody2D.linearVelocity.x;

        if (Mathf.Abs(currentX) < lowspeedThreshold)
        {
            rigidbody2D.AddForce(new Vector2(moveInput.x * initialImpulse, 0f), ForceMode2D.Impulse);
        }
        // if under max speed, add force in the input direction
        else if (Mathf.Abs(currentX) < maxMoveSpeed)
        {
            rigidbody2D.AddForce(new Vector2(moveInput.x * acceleration, 0f), ForceMode2D.Force);
        }
        if (useCancelMech)
        {
            if ((rigidbody2D.linearVelocityX > 0 && moveInput.x < 0) || (rigidbody2D.linearVelocityX < 0 && moveInput.x > 0))
            {
                rigidbody2D.linearVelocityX = Mathf.Clamp01(velCancelAmt * Time.fixedDeltaTime) * rigidbody2D.linearVelocityX;
            }
        }
    }
    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        jumpHeld = true;
        TryJump(); // try an immediate jump on press
    }

    // Called when jump input is released
    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        Debug.Log("Jump Cancelled");
        jumpHeld = false;
    }
    private void TryJump()
    {
        // respect minimum interval between jumps
        if (Time.time - lastJumpTime < minJumpInterval) return;

        if (jumpsRemaining <= 0) return;
        //Cancel
        rigidbody2D.linearVelocityY = 0;
        // Execute jump
        rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        lastJumpTime = Time.time;
        jumpsRemaining--;
    }
    private void ResetJumpsImmediate()
    {
        jumpsRemaining = maxJumps;
        // allow immediate jump after landing
        lastJumpTime = Time.time - minJumpInterval;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        //collision.gameObject.tag use this on a dictionary of actions later
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Un Grounded");
            grounded = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Grounded");
            grounded = true;
            ResetJumpsImmediate();
        }
    }
}