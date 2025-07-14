using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class GroundPound : MonoBehaviour
{
    Jump jumpScript;
    Grounded groundedScript;
    Rigidbody2D rb2d;
    [SerializeField] private bool groundPounding;
    [SerializeField] private bool breakGroundPound;
    [SerializeField] private float chargeTime;
    [SerializeField] private float currentChargeTimer;
    [SerializeField] private float groundPoundSpeed;

    private void Start()
    {
        jumpScript = GetComponent<Jump>();
        groundedScript = GetComponent<Grounded>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(groundedScript.isGrounded)
        {
            groundPounding = false;
        }

        if(groundPounding)
        {
            if(currentChargeTimer > 0f)
            {
                currentChargeTimer -= Time.deltaTime;
            }
            else
            {
                ApplyGroundPound();
            }
        }
    }

    private void ApplyGroundPound()
    {
        Debug.Log("Ground Pounding");
        rb2d.linearVelocityY = groundPoundSpeed;
        rb2d.linearVelocityX = 0;
    }

    public bool CheckGroundPounding()
    {
        return groundPounding;
    }
    public void OnGroundPound(InputValue value)
    {
        if (value.Get<float>() < 0 && !groundedScript.isGrounded)
        {
            Debug.Log("Ground Pounding");
            groundPounding = true;
            currentChargeTimer = chargeTime;
            rb2d.linearVelocity = Vector2.zero;
        }
        else if (value.Get<float>() > 0)
        {
            groundPounding = false;
        }
    }
}
