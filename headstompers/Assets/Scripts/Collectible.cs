using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Despawn()
    {
        if (!NetworkManager.Singleton.IsHost) { return; }

        GetComponent<NetworkObject>().Despawn();
    }
}
