using UnityEngine;

public class WallJumpIndicator : MonoBehaviour
{
    [SerializeField] private WallJump wallJump;
    [SerializeField] private WallJump.Side side;
    private void Start()
    {
        wallJump = transform.parent.gameObject.transform.parent.gameObject.GetComponent<WallJump>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            wallJump.DetermineDirectionAndWallTouch(side);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            wallJump.DisableWallJump();
        }
    }
}
