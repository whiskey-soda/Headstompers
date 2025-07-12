using UnityEngine;
using System.Collections.Generic;
public class LoopSetup : MonoBehaviour
{
    public GameObject level;
    [SerializeField] public float leftmostBound { get; private set; }
    [SerializeField] public float rightmostBound { get; private set; }
    public static LoopSetup Instance;

    private void Start()
    {
        Instance = this;
        GameObject mainLevel = Instantiate(level, new Vector2(0, 0), Quaternion.identity);
        GetBoundsOfLevel(mainLevel);

        GameObject leftLevel = Instantiate(level, new Vector2(mainLevel.transform.position.x - Mathf.Abs(leftmostBound - rightmostBound), mainLevel.transform.position.y), Quaternion.identity);
        GameObject rightLevel = Instantiate(level, new Vector2(mainLevel.transform.position.x + Mathf.Abs(leftmostBound - rightmostBound), mainLevel.transform.position.y), Quaternion.identity);
        Debug.Log(leftmostBound);
        Debug.Log(rightmostBound);
    }


    public void GetBoundsOfLevel(GameObject level)
    {
        Collider2D[] childrenColliders = level.GetComponentsInChildren<Collider2D>();
        leftmostBound = FindLeftBound(childrenColliders);
        rightmostBound = FindRightBound(childrenColliders);
        
    }

    public float FindRightBound(Collider2D[] childrenColliders)
    {
        float farthestPoint = -Mathf.Infinity;
        foreach(Collider2D childCollider in childrenColliders)
        {
            float tempCheck = childCollider.bounds.max.x;
            if (tempCheck > farthestPoint)
            {
                farthestPoint = tempCheck;
            }
        }
        return farthestPoint;
    }
    public float FindLeftBound(Collider2D[] childrenColliders)
    {
        float farthestPoint = Mathf.Infinity;
        foreach (Collider2D childCollider in childrenColliders)
        {
            float tempCheck = childCollider.bounds.min.x;
            if (tempCheck < farthestPoint)
            {
                farthestPoint = tempCheck;
            }
        }
        return farthestPoint;
    }
}
