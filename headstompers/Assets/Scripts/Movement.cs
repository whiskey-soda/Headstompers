using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour
{

    [Header("Player States")]
    [SerializeField] public bool isGrounded { get; private set; }
    [SerializeField] public bool isJumping { get; private set; }

    [Space]
    [Header("Movement")]
    [SerializeField] float maxMoveSpeed = 5;

    float movementValue = 0;

    Rigidbody2D rb2d;

    [Space]
    [Header("Edit Grounded")]
    [SerializeField] Collider2D feet;
    [SerializeField] float detectionLength;
    [SerializeField] LayerMask Ground;

    [Space]
    [Header("Jump Edit")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float initialVelocity;
    [SerializeField] private float maxHangTime;
    [SerializeField] private float earlyReleaseMultiplier;
    [SerializeField] private float gravityMultiplier;

    private bool jumpHeld;
    private bool jumpPressed;
    private float gravity;
    private float maxGravity;
    private float jumpVelocity;
    private bool reachedPeak;
    private bool initialJumpStarted;
    private bool accelerationCalculated;
    private bool maxGravCalculated;
    private float currentHangTime;


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    #region Jump


    void Jump()
    {
        if (CheckApplyUpwardJump())
        {
            ApplyUpwardJump(); // Checks to continue upwards momentum
        }
        else if (CheckHangAtPeak())
        {
            HandleHangTime();// Checks to see if the player should hang in the air at all and if the player reached the peak of jump
        }
        else if (CheckApplyDownwardForce())
        {
            ApplyDownwardGravity(); // Applies the gravity 
        }
        else if (CheckForceEndJump())
        {
            EndJump(); //Checks to see if the jump is complete
        }
        else if (!isGrounded && !isJumping && !initialJumpStarted)
        {
            // Checks to see if the player is falling without having jumped
            if (!accelerationCalculated)
            {
                CalculateAcceleration();
            }

            if (!maxGravCalculated)
            {
                CalculateTerminalVelocity();
            }

            if (jumpVelocity > maxGravity)
            {
                jumpVelocity += gravity * Time.fixedDeltaTime;
            }
        }

        UpdateVelocity();
    }

    private void InitiateJump()
    {
        //Sets all the jump variables to get it ready for a jump
        isJumping = true;
        initialJumpStarted = true;
        jumpVelocity = initialVelocity;
        accelerationCalculated = false;
        maxGravCalculated = false;
        reachedPeak = false;
        currentHangTime = 0f;
    }


    private void EndJump()
    {
        //Ends the jump and sets all the variables for the next jump
        isJumping = false;
        initialJumpStarted = false;
        accelerationCalculated = false;
        maxGravCalculated = false;
        reachedPeak = false;
        currentHangTime = 0f;
        jumpVelocity = 0f;
    }

    private bool CheckApplyUpwardJump()
    {
        
        //ends the jump when the  upwards velocity reaches zero
        if (isJumping && !isGrounded && jumpVelocity > 0f)
        {
            return true;
        }
        else { return false; }
        
        
    }

    private void ApplyUpwardJump()
    {
        initialJumpStarted = false;
        //Calculatates the acceleration once and applies downward gravity til jump is zero
        if (!accelerationCalculated)
        {
            CalculateAcceleration();
        }

        jumpVelocity += gravity * Time.fixedDeltaTime;
    }

    private bool CheckHangAtPeak()
    {
        //Sees if the player has any hang time once the jump is done applying upwards velocity
        if (isJumping && !isGrounded && jumpVelocity <= 0f && !reachedPeak)
        {
            return true;
        }
        else { return false; }
        
    }

    private void HandleHangTime()
    {
        // No downwards velocity and hangtime increments
        jumpVelocity = 0f;
        currentHangTime += Time.fixedDeltaTime;

        //Checks to see if the hangtime is over
        if (currentHangTime >= maxHangTime)
        {
            reachedPeak = true;
        }
        initialJumpStarted = false;
    }

    private bool CheckApplyDownwardForce()
    {
        //Checks to see if we continue to apply downward velocity
        //Will end if the player becomes grounded
        if(isJumping && !isGrounded && reachedPeak && jumpVelocity > maxGravity)
        {
            return true;
        }
        else { return false; };
        
    }

    private void ApplyDownwardGravity()
    {
        // Calculates the max fall speed
        if (!maxGravCalculated)
        {
            CalculateTerminalVelocity();
        }

        //applys downward velocity until max is reached
        jumpVelocity += gravity * Time.fixedDeltaTime;
        initialJumpStarted = false;
    }

    private bool CheckForceEndJump()
    {
        //checks if the player is on the ground after jumping and ends it
        if(isGrounded && (!isJumping || !initialJumpStarted))
        {
            return true;
        }
        else { return false; };
       
    }


    private void EarlyRelease() 
    {
        jumpVelocity *= earlyReleaseMultiplier; // fall faster when released early
        reachedPeak = true;
    }


    private void CalculateAcceleration()
    {
        //using the equation -u^2/2s = a to figure out the necessesary acceleration that i need to apply to decrease the amount of velocity to the jump height 
        gravity = (-(initialVelocity * initialVelocity)) / (2f * jumpHeight);
        accelerationCalculated = true;
    }

    private void CalculateTerminalVelocity()
    {
        //Calculates the gravity based on the height and the gravity mult
        maxGravity = -Mathf.Sqrt(gravityMultiplier * Mathf.Abs(gravity) * jumpHeight);
        maxGravCalculated = true;
    }

    private void UpdateVelocity()
    {
        //changes the y linear velocity
        Vector3 velocity = rb2d.linearVelocity;
        velocity.y = jumpVelocity;
        rb2d.linearVelocity = velocity;
    }

    #endregion 
    #region Grounded

    public void CheckGrounded()
    {
        
        Vector2 castOrigin = new Vector2(feet.bounds.center.x, feet.bounds.min.y); //Defines the center and middle of the collider
        Vector2 castSize = new Vector2(feet.bounds.size.x, detectionLength); //Defines the width as the collider and height customizable


        LayerMask mask = Ground; //Set so that we can add other layers
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, castSize, 0f, Vector2.down, detectionLength, mask); 

        if (hit.collider != null) 
        { 
            isGrounded = true;
            //Debug.Log("Grounded");
        }
        else{ isGrounded = false; }
    }

    #endregion
    #region Inputs
    public void OnMove(InputValue value)
    {
        if (value.Get<float>() != 0) // non-zero input value
        {
            movementValue = value.Get<float>() * maxMoveSpeed;
        }
        else { movementValue = 0; } // input value is 0

    }

    public void OnJump(InputValue value)
    {
        bool inputPressed = value.isPressed;

        if (!inputPressed && jumpHeld)
        {
            // Player just released the jump button
            jumpPressed = false;

            if (isJumping && !reachedPeak)
            {
                EarlyRelease();
            }
            Debug.Log("Jump released");
        }

        jumpHeld = inputPressed;

        if (inputPressed && !isJumping && isGrounded)
        {
            jumpPressed = true;
            Debug.Log("Jump pressed");
            
        }
    }
    public void CheckJump()
    {
        if (jumpPressed && !isJumping && isGrounded)
        {
            jumpVelocity = 0;
            InitiateJump();

        }
    }

    #endregion

    private void FixedUpdate()
    {
        // apply movement value
        rb2d.linearVelocityX = movementValue;

        //Apply jump and gravity aand jump
        Jump();
    }

    // Update is called once per frame
    void Update()
    {
        //These need to be checked every possible fram
        CheckGrounded();
        if(!isJumping)
        {
            CheckJump();
        }
        
    }
}
