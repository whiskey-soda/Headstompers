using Unity.Netcode;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }

}
