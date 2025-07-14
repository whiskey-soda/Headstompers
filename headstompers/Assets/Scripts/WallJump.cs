using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallJump : MonoBehaviour
{
    //Components
    private Grounded groundedScript;
    private Jump jumpScript;
    private Rigidbody2D rb2d;
    public enum Side { Left, Right }
    [Header("Input Directions")]
    [SerializeField] private Side directionTouched;
    [SerializeField] private Side directionHeld;

    [Header("Wall Jump States")]
    [SerializeField] private bool isHoldingOnWall;
    [SerializeField] private bool isTouchingWall;
    [SerializeField] private bool onWall;
    [SerializeField] private bool wallJumped;
    [SerializeField] private bool reloadingWallJump;

    [Header("Wall Jump Editable Values")]
    [SerializeField] private float wallDescentSpeed;
    [SerializeField] private float wallJumpReload;
    [SerializeField] private float wallJumpForceX;
    [SerializeField] private float wallJumpForceY;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        groundedScript = GetComponent<Grounded>();
        jumpScript = GetComponent<Jump>();
    }
    public void FixedUpdate()
    {
        if (isHoldingOnWall && isTouchingWall && !groundedScript.isGrounded && (directionTouched==directionHeld))
        {
            if(wallJumped && !reloadingWallJump)
            {
                Jump();
            }
            else if(!wallJumped && !reloadingWallJump)
            {
                onWall = true;
                ApplyWallGravity();
            }
            
        }
        else
        {
            onWall = false;
        }
    }

    public bool CheckHoldingWall()
    {
        return onWall;
    }

    private void ApplyWallGravity()
    {
        rb2d.linearVelocityY = -wallDescentSpeed;
        Debug.Log(rb2d.linearVelocity);
    }

    private void Jump()
    {
        Debug.Log("Wall Jumping");
        rb2d.linearVelocity = Vector2.zero;
        int direction = directionHeld == Side.Left ? 1 : -1;
        rb2d.linearVelocity = new Vector2(wallJumpForceX * direction,rb2d.linearVelocityY);
        jumpScript.ApplyExternalJump(wallJumpForceY);
        StartCoroutine(ResetWallJump());
    }

    private IEnumerator ResetWallJump()
    {
        wallJumped = true;
        reloadingWallJump = true;
        yield return new WaitForSeconds(wallJumpReload);
        wallJumped = false;
        reloadingWallJump = false;
    }

    


    #region Collider Logic
    /// <summary>
    /// Establishes that the player is touching the wall
    /// </summary>
    /// <param name="sideTouched"></param>
    public void DetermineDirectionAndWallTouch(Side sideTouched)
    {
        isTouchingWall = true;
        directionTouched = sideTouched;
    }


    /// <summary>
    /// Stops the player from wall jumping
    /// </summary>
    public void DisableWallJump()
    {
        isTouchingWall = false;
    }

    #endregion
    #region Inputs
    /// <summary>
    /// Checks to see if the player is holding the left or right button on the wall
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(InputValue value)
    {
        if(value.Get<float>() != 0)
        {
            isHoldingOnWall = true;
            if(value.Get<float>() < 0)
            {
                directionHeld = Side.Left;
            }
            else if(value.Get<float>() > 0)
            {
                directionHeld = Side.Right;
            }
        }
        else
        {
            isHoldingOnWall = false;
        }
    }

    public void OnJump(InputValue value)
    {
        if(value.isPressed)
        {
            if(isHoldingOnWall && isTouchingWall)
            {
                Debug.Log("Jump");
                wallJumped = true;             
            }
        }
    }
    #endregion


}
