using UnityEngine;
using UnityEngine.InputSystem;
public class HeadJump : MonoBehaviour
{
    Jump jumpScript;
    [Header("Edit Grounded")]
    [SerializeField] Collider2D feet;
    [SerializeField] float detectionLength;
    [SerializeField] LayerMask Player;
    [SerializeField] private bool onHead;
    [SerializeField] public bool canHeadJump;
    [SerializeField] public float headJumpVelocity;
    [SerializeField] public float weakHeadJumpVelocity;


    private void Start()
    {
        jumpScript = GetComponent<Jump>();
    }

    private void Update()
    {
        CheckOnHead();
    }
    public bool OnHead()
    {
        return onHead;
    }


    public void CheckOnHead()
    {
        Vector2 castOrigin = new Vector2(feet.bounds.center.x, feet.bounds.min.y); //Defines the center and middle of the collider
        Vector2 castSize = new Vector2(feet.bounds.size.x, detectionLength); //Defines the width as the collider and height customizable


        LayerMask mask = Player; //Set so that we can add other layers
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, castSize, 0f, Vector2.down, detectionLength, mask);

        if (hit.collider != null)
        {
            onHead = true;
        }
        else
        {
            onHead = false;
            canHeadJump = true;
        }

        //Debug.Log("On Head: " + onHead);

        if (onHead)
        {
            jumpScript.coyoteTimer = jumpScript.coyoteTime; // reset on ground
        }
        else
        {
            jumpScript.coyoteTimer -= Time.deltaTime;
        }
    }
}

