using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Jump : MonoBehaviour
{
    WallJump wallJumpScript;
    GroundPound groundPoundScript;
    Grounded groundedScript;
    HeadJump headJumpScript;
    Rigidbody2D rb2d;
    [SerializeField] public bool isJumping { get; private set; }
    [Header("Jump Edit")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float initialVelocity;
    [SerializeField] private float maxHangTime;
    [SerializeField] private float earlyReleaseMultiplier;
    [SerializeField] private float gravityCap;
    [SerializeField] private float gravityScale;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] public float coyoteTime;

    [Header("Jump Debug")]
    public bool jumpHeld;
    public bool jumpPressed;
    private float gravity;
    private float maxGravity;
    private float jumpVelocity;
    private bool reachedPeak;
    private bool initialJumpStarted;
    private bool accelerationCalculated;
    private bool maxGravCalculated;
    private float currentHangTime;
    public float jumpBufferTimer;
    public  float coyoteTimer;

    [SerializeField]public bool startedHoldingWall;
    public bool startedGroundPounding;
    public bool startedHeadJumping;
    public bool headJumping;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        groundedScript = GetComponent<Grounded>();
        wallJumpScript = GetComponent<WallJump>();
        groundPoundScript = GetComponent<GroundPound>();
        headJumpScript = GetComponent<HeadJump>();
    }

    // Update is called once per frame
    void Update()
    {
        //Logic for when the player is not holding onto the wall and not ground pounding
        if (!wallJumpScript.CheckHoldingWall() && !groundPoundScript.CheckGroundPounding())
        {
            CheckJump();
            startedHoldingWall = false;
            startedGroundPounding = false;
        }
        //Logic for when the player is wall sliding
        else if (wallJumpScript.CheckHoldingWall() && !groundPoundScript.CheckGroundPounding() && !headJumpScript.OnHead())
        {
            if (!startedHoldingWall)
            {
                EndJump();
                startedHoldingWall = true;
            }
        }
        //Logic for when the player is ground pounding
        else if (!wallJumpScript.CheckHoldingWall() && groundPoundScript.CheckGroundPounding() && !headJumpScript.OnHead())
        {
            if (!startedGroundPounding)
            {
                EndJump();
                startedGroundPounding = true;
            }
        }

    }

    private void FixedUpdate()
    {

        if (!wallJumpScript.CheckHoldingWall() && !groundPoundScript.CheckGroundPounding())
        {

            //Jump buffer so that the player can input jump early
            if (jumpBufferTimer > 0)
            {
                jumpBufferTimer -= Time.deltaTime;
            }

            //Apply jump and gravity aand jump
            Jumps();
        }
        
    }

    #region Jump

    public void ApplyExternalJump(float force)
    {
        isJumping = true;
        initialJumpStarted = true;

        accelerationCalculated = false;
        maxGravCalculated = false;
        reachedPeak = false;
        currentHangTime = 0f;

        jumpVelocity = force;
    }

    void Jumps()
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
        else if (!groundedScript.isGrounded && !isJumping && !initialJumpStarted)
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


    public void EndJump()
    {
        //Ends the jump and sets all the variables for the next jump
        isJumping = false;
        initialJumpStarted = false;
        accelerationCalculated = false;
        maxGravCalculated = false;
        reachedPeak = false;
        currentHangTime = 0f;
        jumpVelocity = 0f;
        headJumping = false;
    }

    private bool CheckApplyUpwardJump()
    {

        //ends the jump when the  upwards velocity reaches zero
        if (isJumping && !groundedScript.isGrounded && jumpVelocity > 0f)
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
        if (isJumping && !groundedScript.isGrounded && jumpVelocity <= 0f && !reachedPeak)
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
        if (isJumping && !groundedScript.isGrounded && reachedPeak && jumpVelocity > maxGravity)
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
        jumpVelocity += gravity* gravityScale * Time.fixedDeltaTime;
        initialJumpStarted = false;
    }

    private bool CheckForceEndJump()
    {
        //checks if the player is on the ground after jumping and ends it
        if (groundedScript.isGrounded && (!isJumping || !initialJumpStarted))
        {
            return true;
        }
        else { return false; };

    }


    public void EarlyRelease()
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
        maxGravity = -Mathf.Sqrt(gravityCap * Mathf.Abs(gravity) * jumpHeight);
        maxGravCalculated = true;
    }

    private void UpdateVelocity()
    {
        //changes the y linear velocity
        Vector3 velocity = rb2d.linearVelocity;
        velocity.y = jumpVelocity;
        rb2d.linearVelocity = velocity;
    }

    public void StopJump()
    {
        jumpVelocity = 0;
    }

    #endregion

    public void OnJump(InputValue value)
    {
        bool inputPressed = value.isPressed;


        if (!inputPressed && jumpHeld)
        {
            // Player just released the jump button
            jumpPressed = false;

            //the player hasn't reached the peak meaning they let go a little early
            if (isJumping && !reachedPeak)
            {
                EarlyRelease();
            }
            Debug.Log("Jump released");
        }

        jumpHeld = inputPressed;


        //Main jump input
        if (inputPressed)
        {
            jumpPressed = true;
            //Debug.Log("Jump pressed");

        }


        //Buffer for if the player inputs before the player actually lands smoother feel
        if (inputPressed)
        {
            jumpBufferTimer = jumpBufferTime;
        }
    }

    public void CheckJump()
    {
        bool canJump = groundedScript.isGrounded || headJumpScript.OnHead();


        if(headJumpScript.OnHead() && headJumpScript.canHeadJump)
        {
            EndJump();
            headJumpScript.canHeadJump = false;
        }

        //The jump is activate if the player is currently pressing the button or pressed it early but within the buffer window
        // Coyote time is also taken into account to allow the player to jump even if they are already falling
        if ((jumpBufferTimer > 0f || jumpPressed) && !isJumping && coyoteTimer > 0f && canJump)
        {
            jumpBufferTimer = 0f;
            jumpVelocity = 0;

            if(groundedScript.isGrounded)
            {
                InitiateJump();
                Debug.Log("Normal Jump");
            }
            else if(headJumpScript.OnHead())
            {
                ApplyExternalJump(headJumpScript.headJumpVelocity);
                Debug.Log("Strong Head Jump");
            }
        }
        else if(!jumpPressed && headJumpScript.OnHead())
        {
            ApplyExternalJump(headJumpScript.weakHeadJumpVelocity);
            Debug.Log("Weak Head Jump");
        }
    }



}
