using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerCountManager : NetworkBehaviour
{
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsServer)
        {
            // Erhöhe die Spieleranzahl, wenn ein Client startet
            PlayerCountVariable.Instance?.IncreasePlayerCount();
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();


          
            PlayerCountVariable.Instance?.DecreasePlayerCount();
            ConnectionManager.LoadMainSceneIfKicked();

    }
}


