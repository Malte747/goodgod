using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;

public class PlayerSpawner : MonoBehaviour
{
    ConnectionManager connectionManager;
    [Header("Player Prefabs")]
    [SerializeField] private NetworkObject disciplePrefab;
    [SerializeField] private NetworkObject godPrefab;
    [SerializeField] private NetworkObject satanPrefab;
    [SerializeField] private NetworkObject spectatorPrefab;

    [Header("Spawnpunkte")]
    [SerializeField] private Transform[] discipleSpawnPoints;
    [SerializeField] private Transform[] godSpawnPoints;
    [SerializeField] private Transform[] satanSpawnPoints;
    [SerializeField] private Transform[] spectatorSpawnPoints;

    [Header("Rollenverteilung")]
    [SerializeField] private List<RoleDistribution> roleDistributions;

    private NetworkManager networkManager;
    private Dictionary<NetworkConnection, string> playerRoles = new Dictionary<NetworkConnection, string>();

    private void Start()
{
    connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
    networkManager = InstanceFinder.NetworkManager;

    if (networkManager == null)
    {
        Debug.LogError("PlayerSpawner: Kein NetworkManager gefunden!");
        return;
    }

    // Rollenverteilung und Spawning direkt für alle bereits verbundenen Spieler (inkl. Host)
    if (networkManager.IsServer)
    {
        AssignRolesForAllPlayers();
    }

    // Falls ein neuer Spieler joint, weise ihm eine Rolle zu und spawne ihn
    networkManager.ServerManager.OnRemoteConnectionState += OnClientConnected;
}

private void OnDestroy()
{
    if (networkManager != null)
    {
        networkManager.ServerManager.OnRemoteConnectionState -= OnClientConnected;
    }
}
private void OnClientConnected(NetworkConnection conn, RemoteConnectionStateArgs args)
{
    if (args.ConnectionState == RemoteConnectionState.Started)
    {
        Debug.Log($"🔥 Neuer Spieler verbunden: {conn.ClientId}");
        playerRoles[conn] = "spectator"; // Neue Spieler sind immer Spectator
        SpawnPlayer(conn, "spectator");
    }
}

// 🔥 Methode zum Rollenverteilen und Spawnen aller Spieler bei Szenenstart
private void AssignRolesForAllPlayers()
{
    int playerCount = connectionManager.currentPlayerCount;
    RoleDistribution distribution = GetRoleDistribution(playerCount);

    List<string> roles = new List<string>();
    roles.AddRange(GenerateRoles("god", distribution.godCount));
    roles.AddRange(GenerateRoles("satan", distribution.satanCount));
    roles.AddRange(GenerateRoles("disciple", distribution.discipleCount));

    roles = ShuffleList(roles);

    int index = 0;
    foreach (NetworkConnection conn in networkManager.ServerManager.Clients.Values)
    {
        if (index < roles.Count)
        {
            playerRoles[conn] = roles[index];
        }
        else
        {
            playerRoles[conn] = "spectator";
        }

        SpawnPlayer(conn, playerRoles[conn]);
        index++;
    }
}

    private void SpawnPlayer(NetworkConnection conn, string role)
    {
        NetworkObject prefab = GetPrefabByRole(role);
        Transform spawnPoint = GetSpawnPointByRole(role);

        if (prefab == null || spawnPoint == null)
        {
            Debug.LogError($"PlayerSpawner: Kein Prefab oder Spawnpunkt für Rolle '{role}' gefunden!");
            return;
        }

        NetworkObject playerInstance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        networkManager.ServerManager.Spawn(playerInstance, conn);

        Debug.Log($"Spieler {conn.ClientId} wurde als {role} gespawnt.");
    }

    private NetworkObject GetPrefabByRole(string role)
    {
        return role switch
        {
            "god" => godPrefab,
            "satan" => satanPrefab,
            "disciple" => disciplePrefab,
            "spectator" => spectatorPrefab,
            _ => null
        };
    }

    private Transform GetSpawnPointByRole(string role)
    {
        return role switch
        {
            "god" => GetRandomSpawnPoint(godSpawnPoints),
            "satan" => GetRandomSpawnPoint(satanSpawnPoints),
            "disciple" => GetRandomSpawnPoint(discipleSpawnPoints),
            "spectator" => GetRandomSpawnPoint(spectatorSpawnPoints),
            _ => null
        };
    }

    private Transform GetRandomSpawnPoint(Transform[] spawnPoints)
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private RoleDistribution GetRoleDistribution(int playerCount)
    {
        foreach (var distribution in roleDistributions)
        {
            if (distribution.playerCount == playerCount)
            {
                return distribution;
            }
        }
        return new RoleDistribution { playerCount = playerCount, godCount = 1, satanCount = 1, discipleCount = playerCount - 2 };
    }

    private List<string> GenerateRoles(string role, int count)
    {
        List<string> roles = new List<string>();
        for (int i = 0; i < count; i++)
        {
            roles.Add(role);
        }
        return roles;
    }

    private List<string> ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }

    [System.Serializable]
    public class RoleDistribution
    {
        public int playerCount;
        public int godCount;
        public int satanCount;
        public int discipleCount;
    }
}
