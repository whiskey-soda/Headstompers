using UnityEngine;

public class LoopHandler : MonoBehaviour
{
    //All Physical Objects will have this script and will either be static or non moving objects or dynamically moving objects
    public enum LoopObjectType { Static, Dynamic }
    public LoopObjectType objectType;

    //Holds the Game objects for the clones created for the loops
    public GameObject leftClone;
    public GameObject rightClone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Disables the script if the object does not move
        if (objectType == LoopObjectType.Static)
        {
            enabled = false;
        }
        else if (objectType == LoopObjectType.Dynamic)
        {
            enabled = true;
        }

        //Creates the clones that will be used to display the effect of the loop
        leftClone = CreateClone();
        rightClone = CreateClone();

        //Gives the clones that player's positions relative to where they are 
        GiveClonePositions();
    }

    /// <summary>
    /// Makes a clone that copies the sprite of the player
    /// </summary>
    /// <returns></returns>
    private GameObject CreateClone()
    {
        GameObject clone = new GameObject();
        clone.AddComponent<SpriteRenderer>();
        SpriteRenderer cloneSprite = clone.GetComponent<SpriteRenderer>();
        cloneSprite = gameObject.GetComponent<SpriteRenderer>();
        return clone;
    }


    private void FixedUpdate()
    {
        CheckLoopBoundsCrossed();
        GiveClonePositions();
    }

    /// <summary>
    /// Gives the clones thier positions relative to the player
    /// </summary>
    private void GiveClonePositions()
    {
        leftClone.transform.position = new Vector2(transform.position.x - Mathf.Abs(LoopSetup.Instance.leftmostBound - LoopSetup.Instance.rightmostBound), transform.position.y);
        rightClone.transform.position = new Vector2(transform.position.x + Mathf.Abs(LoopSetup.Instance.leftmostBound - LoopSetup.Instance.rightmostBound), transform.position.y);
    }

    /// <summary>
    /// Checks if the player has looped beyond the bounds set by the loop manager
    /// </summary>
    private void CheckLoopBoundsCrossed()
    {
        //Check if the player crossed the left boundary
        if(transform.position.x < LoopSetup.Instance.leftmostBound)
        {
            TeleportPlayer(LoopSetup.Instance.rightmostBound);
        }

        //Check if the player crossed the right boundary
        if (transform.position.x > LoopSetup.Instance.rightmostBound)
        {
            TeleportPlayer(LoopSetup.Instance.leftmostBound);
        }
    }

    /// <summary>
    /// Teleports the player according to which boundary is crossed
    /// </summary>
    /// <param name="xLocation"></param>
    private void TeleportPlayer(float xLocation)
    {
        transform.position = new Vector2(xLocation, transform.position.y);
    }

}
