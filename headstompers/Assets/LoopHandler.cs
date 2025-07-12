using UnityEngine;

public class LoopHandler : MonoBehaviour
{
    public enum LoopObjectType { Static, Dynamic }
    public LoopObjectType objectType;

    public GameObject leftClone;
    public GameObject rightClone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (objectType == LoopObjectType.Static)
        {
            enabled = false;
        }
        else if (objectType == LoopObjectType.Dynamic)
        {
            enabled = true;
        }
        leftClone = CreateClone();
        rightClone = CreateClone();
        GiveClonePositions();
    }

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

    private void GiveClonePositions()
    {
        leftClone.transform.position = new Vector2(transform.position.x - Mathf.Abs(LoopSetup.Instance.leftmostBound - LoopSetup.Instance.rightmostBound), transform.position.y);
        rightClone.transform.position = new Vector2(transform.position.x + Mathf.Abs(LoopSetup.Instance.leftmostBound - LoopSetup.Instance.rightmostBound), transform.position.y);

    }
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

    private void TeleportPlayer(float xLocation)
    {
        transform.position = new Vector2(xLocation, transform.position.y);
    }

}
