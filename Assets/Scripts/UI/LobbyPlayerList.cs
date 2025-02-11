using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using Heathen.SteamworksIntegration;
using Steamworks;
using FishNet.Connection;

public class LobbyPlayerList : NetworkBehaviour
{
    [SerializeField] private GameObject nameTagPrefab; // Prefab mit World-Space Canvas + TMP_Text
    private Transform nameListParent;
    private GameObject nameTagInstance;
    private TMP_Text nameText;

    // Synchronisiert die Steam-Namen für alle Clients
    private readonly SyncVar<string> steamName = new SyncVar<string>();

    void Awake()
    {
        nameListParent = GameObject.Find("PlayerList")?.transform;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        steamName.OnChange += OnSteamNameChanged;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Lokales Nametag für alle Clients erstellen
        if (nameTagPrefab != null && nameListParent != null)
        {
            nameTagInstance = Instantiate(nameTagPrefab, nameListParent);
            nameText = nameTagInstance.GetComponentInChildren<TMP_Text>();
        }

        // Falls der lokale Client der Besitzer ist, sende den Steam-Namen an alle
        if ((IsOwner || IsServer) && SteamSettings.Initialized)
        {
            string mySteamName = UserData.Get(SteamUser.GetSteamID()).Name;
            SendSteamNameToAll(mySteamName);
        }
    }

    [ServerRpc]
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

    public override void OnStopClient()
    {
        base.OnStopClient();
        RemovePlayerFromList();
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        RemovePlayerFromList();
    }

    private void RemovePlayerFromList()
    {
        if (nameTagInstance != null)
        {
            Destroy(nameTagInstance);
        }
    }
}
