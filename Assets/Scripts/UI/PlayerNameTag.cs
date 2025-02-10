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

    // Lokales Nametag für alle Clients erstellen (nicht über das Netzwerk)
    if (nameTagPrefab != null)
    {
        nameTagInstance = Instantiate(nameTagPrefab, transform);
        nameText = nameTagInstance.GetComponentInChildren<TMP_Text>();

        // LookAt-Skript setzen (sodass Nametag zur Kamera schaut)
        nameTagInstance.AddComponent<LookAtCamera>();
    }

    // Falls der lokale Client der Besitzer ist, sende den Steam-Namen an alle
    if (IsOwner && SteamSettings.Initialized)
    {
        string mySteamName = UserData.Get(SteamUser.GetSteamID()).Name;
        SendSteamNameToAll(mySteamName);
    }
}
    
    [ServerRpc]
private void SpawnNametagOnServer(NetworkConnection sender = null)
{
    if (nameTagPrefab != null)
    {
         
        GameObject tag = Instantiate(nameTagPrefab, transform);
        nameText = tag.GetComponentInChildren<TMP_Text>();
        Spawn(tag, sender); // Spawnt das Nametag für alle Clients
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
