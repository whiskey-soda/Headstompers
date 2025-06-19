using UnityEngine;

public class Statistics : MonoBehaviour
{
    private int collectibles;

    public void LoseCollectible()
    {
        collectibles -= 1;
    }
}
