using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] GameObject collectiblePrefab;
    [Space]
    [SerializeField] List<Transform> spawnLocations = new List<Transform>();

    void SpawnCollectible(Vector2 spawnPosition)
    {
        // do nothing if not host
        if (!NetworkManager.Singleton.IsHost) { return; }

        // spawn collectible locally and position it
        GameObject collectible = Instantiate(collectiblePrefab, transform);
        collectible.transform.position = spawnPosition;

        // spawn collectible over the network
        collectible.GetComponent<NetworkObject>().Spawn();
    }

    [ContextMenu("spawn collectible at random position")]
    void SpawnCollectibleAtRandomPosition()
    {
        // do nothing if there are no spawn locations
        if (!spawnLocations.Any()) { return; }

        SpawnCollectible(spawnLocations[Random.Range(0, spawnLocations.Count - 1)].position);
    }
}
