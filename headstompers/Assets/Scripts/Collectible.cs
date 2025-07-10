using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsHost) { return; }

        if (collision.CompareTag("Player"))
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
