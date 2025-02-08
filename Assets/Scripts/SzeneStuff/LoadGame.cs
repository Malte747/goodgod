using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class LoadGame : NetworkBehaviour
{
    public void StartGame()
    {
        BootstrapSceneManager.LoadGame();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Falls der Client nicht der Host ist, UI-Element deaktivieren.
        if (!IsHost)
        {
            gameObject.SetActive(false);
        }
    }
}
