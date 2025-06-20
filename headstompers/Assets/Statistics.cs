using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;
public class Statistics : NetworkBehaviour
{
    private int collectibles;
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
        collectibles -= 1;
    }
}
