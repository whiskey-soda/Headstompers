using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

[RequireComponent(typeof(TextMeshProUGUI))]

public class ScoreDisplay : MonoBehaviour
{
    TextMeshProUGUI scoreText;

    List<Statistics> players = new List<Statistics>();

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //  refresh player list if a player has joined or left
        if (NetworkManager.Singleton.ConnectedClients.Count != players.Count) { RefreshPlayerList(); }

        // update text with player scores
        scoreText.text = "Score\n";
        foreach (Statistics player in players)
        {
            scoreText.text += $"p{players.IndexOf(player) + 1}: {player.score}";
        }
    }

    /// <summary>
    /// clears the list of player statistics scripts and fetches all player statistics scripts in order of instance ID
    /// </summary>
    void RefreshPlayerList()
    {
        players.Clear();
        players = FindObjectsByType<Statistics>(FindObjectsSortMode.InstanceID).ToList();
    }

}
