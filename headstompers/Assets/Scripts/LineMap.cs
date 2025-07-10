using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class LineMap : MonoBehaviour
{
    [SerializeField] GameObject iconPrefab;
    [Space]
    [SerializeField] RectTransform lineImageRect;
    [Space]
    [SerializeField] Transform leftMapEdge;
    [SerializeField] Transform rightMapEdge;

    List<Transform> playerTransforms = new List<Transform>();
    List<RectTransform> iconRects = new List<RectTransform>();

    public static LineMap Instance;
    private void Awake()
    {
        // singleton code
        if (Instance == null) { Instance = this; }
        else if (Instance != this) { Destroy(this); }
    }

    /// <summary>
    /// adds a players transform to a list for position tracking, and creates a new icon for that player
    /// </summary>
    /// <param name="newPlayerTransform"></param>
    public void AddPlayer(Transform newPlayerTransform)
    {
        if (playerTransforms.Contains(newPlayerTransform)) { return; }

        playerTransforms.Add(newPlayerTransform);

        // instantiates a new icon as a child of the line image, so local scale can be used for positioning
        GameObject newIcon = Instantiate(iconPrefab, lineImageRect);
        iconRects.Add(newIcon.GetComponent<RectTransform>());

        newIcon.GetComponent<TextMeshProUGUI>().text = playerTransforms.Count.ToString();
    }

    private void Update()
    {

        // sets the positions of all icons according to the players' positions
        for (int i = 0; i < playerTransforms.Count; i++)
        {
            Transform playerTransform = playerTransforms[i];
            RectTransform iconRect = iconRects[i];

            // calculates the position of the player on the map from 0 to 1, 0 being the left edge and 1 being the right edge
            float mapProgress = (playerTransform.position.x - leftMapEdge.position.x) / (rightMapEdge.position.x - leftMapEdge.position.x);
            Debug.Log(mapProgress);
            
            // use mapProgress float to set the position of the player's icon along the line
            iconRect.localPosition = new Vector3(   Mathf.Lerp(lineImageRect.offsetMin.x, lineImageRect.offsetMax.x, mapProgress),
                                                    iconRect.localPosition.y);
        }
    }

}
