using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour
{

    [SerializeField] float maxMoveSpeed = 5;

    float movementValue = 0;

    Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        if (value.Get<float>() != 0) // non-zero input value
        {
            movementValue = value.Get<float>() * maxMoveSpeed;
        }
        else { movementValue = 0; } // input value is 0

    }

    private void FixedUpdate()
    {
        // apply movement value
        rb2d.linearVelocityX = movementValue;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
