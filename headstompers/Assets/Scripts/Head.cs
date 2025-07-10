using Unity.Netcode;
using UnityEngine;

public class Head : MonoBehaviour
{
    [SerializeField]private bool hitHead;
    [SerializeField] private Jump jumpScript;
    [SerializeField] private Statistics statistics;
    private void Awake()
    {
        jumpScript = transform.parent.gameObject.GetComponent<Jump>();
        statistics = transform.parent.gameObject.GetComponent<Statistics>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!hitHead)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //Stop jump and lose a collectible if jumped on
                jumpScript.StopJump();

                // if local player is the one doing damage, lose collectible (client authoritative)
                if (collision.GetComponent<NetworkObject>().IsLocalPlayer) { statistics.LoseCollectible(); }

            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                //Just stop the jump if the player hits the ceiling
                jumpScript.StopJump();
                Debug.Log("Hit Head on Ceiling");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hitHead)
        {
            hitHead = false;
        }
    }
}
