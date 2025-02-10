using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using Heathen.SteamworksIntegration;
using Steamworks;
using FishNet.Connection;

public class PlayerNameSync : NetworkBehaviour
{
    [SerializeField] private GameObject nameTagPrefab; // Prefab mit World-Space Canvas + TMP_Text
    private GameObject nameTagInstance;
    private TMP_Text nameText;

    // Synchronisiert die Steam-Namen für alle Clients
    private readonly SyncVar<string> steamName = new SyncVar<string>();

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        steamName.OnChange += OnSteamNameChanged;
     
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Erstellen des Name-Tags über dem Spieler
        if (nameTagPrefab != null)
        {
            nameTagInstance = Instantiate(nameTagPrefab, transform);
            nameText = nameTagInstance.GetComponentInChildren<TMP_Text>();
        }

        // Falls der lokale Client der Besitzer dieses Objekts ist, Steam-Namen setzen
        if (IsOwner)
        {
            if (SteamSettings.Initialized)
            {
                string mySteamName = UserData.Get(SteamUser.GetSteamID()).Name;
                SendSteamNameToAll(mySteamName);
            }
            else
            {
                Debug.LogError("Steam nicht initialisiert!");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendSteamNameToAll(string name, NetworkConnection sender = null)
    {
        steamName.Value = name;
    }

    private void OnSteamNameChanged(string previous, string current, bool asServer)
    {
        if (nameText != null)
        {
            nameText.text = current;
        }
    }
}
