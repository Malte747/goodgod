using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject playerPrefab; // Player-Prefab im Inspector zuweisen
    [SerializeField] private Transform[] spawnPoints;   // Liste der Spawnpunkte

    private NetworkManager networkManager;
    private int nextSpawnIndex = 0; // Für sequentielles Spawning

    private void Start()
    {
        networkManager = InstanceFinder.NetworkManager;
        if (networkManager == null)
        {
            Debug.LogError("PlayerSpawner: Kein NetworkManager gefunden!");
            return;
        }

        // Reagiere auf Szenenwechsel
        networkManager.SceneManager.OnClientLoadedStartScenes += SpawnPlayerOnSceneLoad;

        // Falls Server aktiv ist, bereits verbundene Clients + Host spawnen
        if (networkManager.IsServer)
        {
            foreach (NetworkConnection conn in networkManager.ServerManager.Clients.Values)
            {
                SpawnPlayer(conn);
            }
        }
    }

    private void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.SceneManager.OnClientLoadedStartScenes -= SpawnPlayerOnSceneLoad;
        }
    }

    private void SpawnPlayerOnSceneLoad(NetworkConnection conn, bool asServer)
    {
        if (asServer)
        {
            SpawnPlayer(conn);
        }
    }

    private void SpawnPlayer(NetworkConnection conn)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: Kein PlayerPrefab gesetzt!");
            return;
        }

        // Spawnpunkt auswählen
        Transform spawnPoint = GetNextSpawnPoint();

        // Player-Objekt instanziieren
        NetworkObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        networkManager.ServerManager.Spawn(playerInstance, conn);

        Debug.Log($"Spieler {conn.ClientId} wurde gespawnt.");
    }

    private Transform GetNextSpawnPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("PlayerSpawner: Keine Spawnpunkte gesetzt! Verwende Standardposition.");
            return transform;
        }

        Transform spawnPoint = spawnPoints[nextSpawnIndex];

        // Nächsten Index setzen (zyklisch)
        nextSpawnIndex = (nextSpawnIndex + 1) % spawnPoints.Length;

        return spawnPoint;
    }
}
