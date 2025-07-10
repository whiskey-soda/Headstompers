using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;
public class Statistics : NetworkBehaviour
{
    public int score { get; private set; } = 0;

    [SerializeField] private CinemachineCamera mainCam;
    private void Start()
    {
        //Only enable local camera
        if(IsLocalPlayer)
        {
            mainCam.enabled = true;
            Debug.Log("Owned Camera");
        }
        else
        {
            mainCam.enabled = false;
            Debug.Log("Did not own Camera");
        }
    }

    public void LoseCollectible()
    {
        ChangeScoreRpc(-1);
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeScoreRpc(int change)
    {
        score += change;
    }

}
