using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float maxMoveSpeed = 5;

    float movementValue = 0;

    Rigidbody2D rb2d;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    #region Inputs
    public void OnMove(InputValue value)
    {
        if (value.Get<float>() != 0) // non-zero input value
        {
            movementValue = value.Get<float>() * maxMoveSpeed;
        }
        else { movementValue = 0; } // input value is 0

    }


    

    #endregion

    private void Update()
    {
        // apply movement value
        rb2d.linearVelocityX = movementValue;
    }
}
