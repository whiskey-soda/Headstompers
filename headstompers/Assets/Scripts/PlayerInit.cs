using UnityEngine;

/// <summary>
/// initializes stuff on the player
/// </summary>
public class PlayerInit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (LineMap.Instance != null)
        {
            LineMap.Instance.AddTrackedObject(transform);
        }
    }
}
