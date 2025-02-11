using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using TMPro;
using Heathen.SteamworksIntegration;
using Steamworks;

public class PlayerListManager : NetworkBehaviour
{
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerEntryPrefab;
    private Dictionary<NetworkConnection, GameObject> playerEntries = new Dictionary<NetworkConnection, GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServer)
        {
            InstanceFinder.ServerManager.OnRemoteConnectionState += OnPlayerStateChanged;
            AddHostToList();
        }
        UpdatePlayerList();
    }

    private void OnDestroy()
    {
        if (base.IsServer)
        {
            InstanceFinder.ServerManager.OnRemoteConnectionState -= OnPlayerStateChanged;
        }
    }

    private void OnPlayerStateChanged(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            AddPlayerToList(conn);
        }
        else if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            RemovePlayerFromList(conn);
        }
    }

    private void AddHostToList()
    {
        foreach (var conn in InstanceFinder.ServerManager.Clients.Values)
        {
            if (conn.IsHost)
            {
                AddPlayerToList(conn);
                break;
            }
        }
    }

    [Server]
    private void AddPlayerToList(NetworkConnection conn)
    {
        if (playerEntries.ContainsKey(conn)) return;

        // FishNet-spezifisches Instanziieren & Spawnen
        GameObject entry = Instantiate(playerEntryPrefab, playerListContainer);
        Spawn(entry, conn); // Wichtig: Über FishNet spawnen
        entry.GetComponent<TMP_Text>().text = GetSteamName(conn);
        playerEntries.Add(conn, entry);
    }

    [Server]
    private void RemovePlayerFromList(NetworkConnection conn)
    {
        if (playerEntries.TryGetValue(conn, out GameObject entry))
        {
            Despawn(entry); // Wichtig: Über FishNet despawnen
            playerEntries.Remove(conn);
        }
    }

    private void UpdatePlayerList()
    {
        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            AddPlayerToList(conn);
        }
    }

    private string GetSteamName(NetworkConnection conn)
    {
        return conn.IsHost 
            ? SteamFriends.GetPersonaName() 
            : UserData.Get(SteamUser.GetSteamID()).Name;
    }
}