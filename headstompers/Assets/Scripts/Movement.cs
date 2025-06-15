using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Grounded))]

public class Movement : MonoBehaviour
{
    [SerializeField] float maxMoveSpeed = 5;
    [SerializeField] float acceleration = 5;
    [SerializeField] float deceleration = 5;
    [Tooltip("multiplier applied to acceleration when player is in the air")]
    [SerializeField] float midairAccelMult = .5f;

    float movementValue = 0;
    int moveDirection = 0;

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
    }

    private void FixedUpdate()
    {
        float accelMult = 1;
        if (!groundedScript.isGrounded) { accelMult = midairAccelMult; } // slower movement in air

        //moving left
        if (moveDirection < 0) { movementValue -= acceleration * Time.deltaTime * accelMult; }

        // moving right
        else if (moveDirection > 0) { movementValue += acceleration * Time.deltaTime * accelMult; }

        // no imput
        else if (moveDirection == 0)
        {
            // if player is moving, slow them down to gradually bring them to a stop
            if (movementValue != 0)
            {
                float newMoveVal = Mathf.Abs(movementValue);
                newMoveVal -= deceleration * Time.deltaTime * accelMult;
                if (newMoveVal < 0) { newMoveVal = 0; }

                movementValue = newMoveVal * Mathf.Sign(movementValue);
            }
        }

        // cap movement speed
        if (Mathf.Abs(movementValue) > maxMoveSpeed)
        {
            movementValue = maxMoveSpeed * Mathf.Sign(movementValue);
        }

        // apply movement value
        rb2d.linearVelocityX = movementValue;
    }
}
