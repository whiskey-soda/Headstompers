using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class CollectDetection : MonoBehaviour
{
    [SerializeField] Statistics statisticsScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // do nothing if not host (server authoritative logic)
        if (!NetworkManager.Singleton.IsHost) { return; }

        // if collectible is valid, despawn it and update score
        Collectible collectible = collision.GetComponent<Collectible>();
        if (collision.CompareTag("Collectible") && collectible != null)
        {
            collectible.Despawn();
            statisticsScript.ChangeScoreRpc(+1);

            if (CollectibleSpawner.Instance != null) {CollectibleSpawner.Instance.SpawnCollectibleAtRandomPosition();}
        }
    }
}
