using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class LoadGame : NetworkBehaviour
{
    public void StartGame()
    {
        BootstrapSceneManager.LoadGame();
    }
}
