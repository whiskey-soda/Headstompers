using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class LineMap : MonoBehaviour
{
    [SerializeField] GameObject iconPrefab;
    [Space]
    [SerializeField] RectTransform lineImageRect; // icons are created as children of the line image for easier positioning
    [Space]
    [SerializeField] Transform leftMapEdge;
    [SerializeField] Transform rightMapEdge;

    List<Transform> trackedTransforms = new List<Transform>();
    List<RectTransform> iconRects = new List<RectTransform>();

    public static LineMap Instance;
    private void Awake()
    {
        // singleton code
        if (Instance == null) { Instance = this; }
        else if (Instance != this) { Destroy(this); }
    }

    /// <summary>
    /// adds an object's transform to a list for position tracking, and creates a new icon for that object
    /// </summary>
    /// <param name="objectTransform"></param>
    public void AddTrackedObject(Transform objectTransform)
    {
        if (trackedTransforms.Contains(objectTransform)) { return; }

        trackedTransforms.Add(objectTransform);

        // instantiates a new icon as a child of the line image, so local scale can be used for positioning
        GameObject newIcon = Instantiate(iconPrefab, lineImageRect);
        iconRects.Add(newIcon.GetComponent<RectTransform>());

        newIcon.GetComponent<TextMeshProUGUI>().text = trackedTransforms.Count.ToString();
    }

    private void Update()
    {
        RemoveNullTransforms();

        // sets the positions of all icons according to the tracked objects' positions
        for (int i = 0; i < trackedTransforms.Count; i++)
        {
            Transform playerTransform = trackedTransforms[i];
            RectTransform iconRect = iconRects[i];

            // calculates the position of the object on the map from 0 to 1, 0 being the left edge and 1 being the right edge
            float mapProgress = (playerTransform.position.x - leftMapEdge.position.x) / (rightMapEdge.position.x - leftMapEdge.position.x);
            Debug.Log(mapProgress);

            // use mapProgress float to set the position of the object's icon along the line
            iconRect.localPosition = new Vector3(Mathf.Lerp(lineImageRect.offsetMin.x, lineImageRect.offsetMax.x, mapProgress),
                                                    iconRect.localPosition.y);
        }
    }

    /// <summary>
    /// iterates over the tracked objects list, removing one null at a time (to avoid errors due to removed elements).
    /// if a null is removed, iterates over the list again until no nulls are found
    /// </summary>
    private bool RemoveNullTransforms()
    {
        bool listModified = false;

        bool freeOfNulls = false; // sets to false if a null is found+removed while parsing the list
        while (!freeOfNulls)
        {
            bool nullRemoved = false;
            // remove a null from the list
            foreach (Transform transform in trackedTransforms)
            {
                if (transform == null)
                {
                    // remove tracked transform and its corresponding icon

                    // destroy icon and remove from list
                    Destroy(iconRects[trackedTransforms.IndexOf(transform)].gameObject);
                    iconRects.RemoveAt(trackedTransforms.IndexOf(transform));

                    trackedTransforms.Remove(transform);

                    nullRemoved = true;
                    break; // exit loop after modifying list to avoid errors
                }
            }

            // no nulls found, list is clear of null objects and logic can proceed
            if (!nullRemoved) { freeOfNulls = true; }
            // if a null was found, it iterates over the list again
            else { listModified = true; }
        }

        return listModified;
    }
}
