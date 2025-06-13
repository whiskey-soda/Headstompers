using UnityEngine;

public class Grounded : MonoBehaviour
{
    [SerializeField] public bool isGrounded { get; private set; }

    [Header("Edit Grounded")]
    [SerializeField] Collider2D feet;
    [SerializeField] float detectionLength;
    [SerializeField] LayerMask Ground;
    Jump jump;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        jump = GetComponent<Jump>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
    }
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
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded)
        {
            jump.coyoteTimer = jump.coyoteTime; // reset on ground
        }
        else
        {
            jump.coyoteTimer -= Time.deltaTime;
        }
    }

    #endregion
}
