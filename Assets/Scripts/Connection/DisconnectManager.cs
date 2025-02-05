using UnityEngine;
using UnityEngine.SceneManagement; // Import für Szenenwechsel
using FishNet;
using FishNet.Transporting;

public class DisconnectManager : MonoBehaviour
{
    public static DisconnectManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Disconnect()
    {
        var networkManager = InstanceFinder.NetworkManager;

        if (networkManager == null)
        {
            Debug.LogWarning("NetworkManager nicht gefunden!");
            return;
        }

        if (networkManager.IsServer) // Falls Spieler der Host ist
        {
            Debug.Log("Beende den Server...");
            networkManager.ServerManager.StopConnection(true); // Beendet den Server und kickt alle Spieler
        }
        else if (networkManager.IsClient) // Falls Spieler ein Client ist
        {
            Debug.Log("Trenne die Verbindung als Client...");
            networkManager.ClientManager.StopConnection(); // Trennt den Client vom Server
        }
        else
        {
            Debug.LogWarning("Spieler ist weder Host noch Client. Keine Verbindung aktiv.");
        }

        // Lokal zur "MainConnection" Szene wechseln
        LoadMainConnectionScene();
    }

    private void LoadMainConnectionScene()
    {
        Debug.Log("Lade MainConnection Szene...");
        SceneManager.LoadScene("MainConnection");
    }
}


