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

        //Creates the main level and then gets the bounds of the level based on the platforms inside of it
        GameObject mainLevel = Instantiate(level, new Vector2(0, 0), Quaternion.identity);
        GetBoundsOfLevel(mainLevel);

        //Creates the clone of the test level. Will probably isolate this to individual ground tiles later but for now the whole level is copied.
        GameObject leftLevel = Instantiate(level, new Vector2(mainLevel.transform.position.x - Mathf.Abs(leftmostBound - rightmostBound), mainLevel.transform.position.y), Quaternion.identity);
        GameObject rightLevel = Instantiate(level, new Vector2(mainLevel.transform.position.x + Mathf.Abs(leftmostBound - rightmostBound), mainLevel.transform.position.y), Quaternion.identity);

        Debug.Log(leftmostBound);
        Debug.Log(rightmostBound);
    }

    /// <summary>
    /// Sets the left and right bounds of the level
    /// </summary>
    /// <param name="level"></param>
    public void GetBoundsOfLevel(GameObject level)
    {
        //Gets each of the colliders of the level's platforms and sees where they are in the world to get the full bounds of the level
        Collider2D[] childrenColliders = level.GetComponentsInChildren<Collider2D>();
        leftmostBound = FindLeftBound(childrenColliders);
        rightmostBound = FindRightBound(childrenColliders);
        
    }

    /// <summary>
    /// Finds the right most point of the level
    /// </summary>
    /// <param name="childrenColliders"></param>
    /// <returns></returns>
    public float FindRightBound(Collider2D[] childrenColliders)
    {
        //Sets the farthest point to negative infinity so that we dont mistakenly get a bad value on a really big level
        float farthestPoint = -Mathf.Infinity;

        //iterates through each of the bounds of the colliders and sets accordingly if the size is less than negative infinity
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

    /// <summary>
    /// Finds the left most point of the level
    /// </summary>
    /// <param name="childrenColliders"></param>
    /// <returns></returns>
    public float FindLeftBound(Collider2D[] childrenColliders)
    {
        //Sets the farthest point to positive infinity so that we dont mistakenly get a bad value on a really big level
        float farthestPoint = Mathf.Infinity;

        //iterates through each of the bounds of the colliders and sets accordingly if the size is greater than infinity
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
