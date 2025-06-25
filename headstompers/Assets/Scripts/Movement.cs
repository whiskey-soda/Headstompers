using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Grounded))]

public class Movement : MonoBehaviour
{
    [SerializeField] float maxMoveSpeed = 5;
    [SerializeField] float acceleration = 5;
    [Space]
    [SerializeField] float sprint_maxMoveSpeed = 10;
    [SerializeField] float sprint_acceleration = 10;
    [Space]
    [SerializeField] float deceleration = 5;
    [Tooltip("multiplier applied to acceleration when player is in the air")]
    [SerializeField] float midairAccelMult = .5f;

    float xVelocity = 0;
    int moveDirection = 0;
    float moveInputStrength = 1;

    bool isSprinting = false;

    [Space]
    bool isSliding = false;
    bool slideInput = false;
    float slideDuration = 0;

    [SerializeField] float slideSpeedMult = 1.2f;
    [SerializeField] float slideDeceleration = 3;
    [Tooltip("time before a player can stop sliding")]
    [SerializeField] float slideMinSeconds = .25f;


    Rigidbody2D rb2d;
    Grounded groundedScript;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        groundedScript = GetComponent<Grounded>();
    }

    public void OnMove(InputValue value)
    {
        float inputFloat = value.Get<float>();

        if (inputFloat == 0) { moveDirection = 0; }
        else { moveDirection = (int)Mathf.Sign(inputFloat); }

        moveInputStrength = Mathf.Abs(inputFloat);
    }

    public void OnSprint(InputValue value)
    {
        // is called every time the Sprint input value changes

        // set isSprinting based on the input recieved
        if (value.Get<float>() == 0) { isSprinting = false; }
        else { isSprinting = true; }
    }

    public void OnSlide(InputValue value)
    {
        if (value.Get<float>() == 0)
        {
            slideInput = false;
        }
        else { slideInput = true; }
    }

    private void FixedUpdate()
    {
        // SLIDE LOGIC
        // ** logic for ending a slide **
        if (isSliding)
        {
            slideDuration += Time.deltaTime;
            // apply sliding deceleration
            xVelocity = Mathf.Sign(xVelocity) * (Mathf.Abs(xVelocity) - slideDeceleration * Time.deltaTime);

            // slide stops if player leaves the ground
            if (!groundedScript.isGrounded) { isSliding = false; }

            // minimum duration passed, slide can be canceled
            if (slideDuration > slideMinSeconds)
            {
                // player ends slide by releasing input OR
                // slide ends automatically because player is moving slower than max walking speed
                if (!slideInput || Mathf.Abs(xVelocity) < maxMoveSpeed)
                {
                    isSliding = false;
                }
            }
        }
        // ** logic for starting a slide **
        // check if conditions are appropriate to begin a slide
        // conditions: slide input is pressed, player is not already sliding, is on the ground, and speed > max walking speed
        else if (slideInput && groundedScript.isGrounded && Mathf.Abs(xVelocity) > maxMoveSpeed)
        {
            xVelocity *= slideSpeedMult;
            isSliding = true;
            slideDuration = 0;
        }


        // MOVEMENT LOGIC
        if (!isSliding)
        {
            float accelMult = 1;
            accelMult = CalculateAccelerationMultiplier();

            // change speed limits if sprinting
            float _maxSpeed = maxMoveSpeed;
            float _acceleration = acceleration;
            if (isSprinting) { _maxSpeed = sprint_maxMoveSpeed; _acceleration = sprint_acceleration; }

            // input strength influences the max speed
            _maxSpeed *= moveInputStrength;


            // if player is currently above max speed, they can only move AGAINST their current move direction
            // if they are not doing that, they will decelerate
            if (Mathf.Abs(xVelocity) > _maxSpeed)
            {
                // player is moving against current velocity
                if (moveDirection != Mathf.Sign(xVelocity))
                {
                    // apply movement
                    ApplyMovement(accelMult, _maxSpeed, _acceleration);
                }

                // player is moving with current velocity
                else if (moveDirection != 0)
                {
                    // decelerate to max speed
                    // this is so the player will not go below max speed if they keep trying to move in the current direction
                    Decelerate(accelMult);
                    if (Mathf.Abs(xVelocity) < _maxSpeed) { xVelocity = _maxSpeed * Mathf.Sign(xVelocity); }
                }

                // no movement
                else if (moveDirection == 0) { Decelerate(accelMult); }
            }
            // player not above max speed, apply movement normally
            else
            {
                ApplyMovement(accelMult, _maxSpeed, _acceleration);

                // cap movement speed
                if (Mathf.Abs(xVelocity) > _maxSpeed)
                {
                    xVelocity = _maxSpeed * Mathf.Sign(xVelocity);
                }
            }
        }

        // apply movement value
        rb2d.linearVelocityX = xVelocity;
    }

    /// <summary>
    /// change the player's x velocity based on movment input and movement variable valuess
    /// </summary>
    /// <param name="accelMult"></param>
    /// <param name="_maxSpeed"></param>
    /// <param name="_acceleration"></param>
    private void ApplyMovement(float accelMult, float _maxSpeed, float _acceleration)
    {
        //moving left
        if (moveDirection < 0) { xVelocity -= _acceleration * Time.deltaTime * accelMult; }

        // moving right
        else if (moveDirection > 0) { xVelocity += _acceleration * Time.deltaTime * accelMult; }

        // no imput
        else if (moveDirection == 0)
        {
            // if player is moving, slow them down to gradually bring them to a stop
            if (xVelocity != 0)
            {
                Decelerate(accelMult);
            }
        }
    }

    /// <summary>
    /// returns an acceleration multiplier determined by the player's grounded/midair status
    /// </summary>
    /// <returns></returns>
    private float CalculateAccelerationMultiplier()
    {
        float accelMult = 1;

        if (!groundedScript.isGrounded) { accelMult = midairAccelMult; } // slower movement in air

        return accelMult;
    }

    /// <summary>
    /// slow the x velocity based on deceleration
    /// </summary>
    /// <param name="accelMult"></param>
    private void Decelerate(float accelMult)
    {
        float newXVelocity = Mathf.Abs(xVelocity);
        newXVelocity -= deceleration * Time.deltaTime * accelMult;
        if (newXVelocity < 0) { newXVelocity = 0; }

        xVelocity = newXVelocity * Mathf.Sign(xVelocity);
    }
}
