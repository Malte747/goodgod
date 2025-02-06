using UnityEngine;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;

public class BootstrapSceneManager : MonoBehaviour
{
    private static BootstrapSceneManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState += OnClientConnectionStateChange;
        InstanceFinder.NetworkManager.ServerManager.OnServerConnectionState += OnServerConnectionStateChange;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.NetworkManager != null)
        {
            InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState -= OnClientConnectionStateChange;
            InstanceFinder.NetworkManager.ServerManager.OnServerConnectionState -= OnServerConnectionStateChange;
        }
    }

    private void OnClientConnectionStateChange(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            LoadClientScene("Connected");
            UnloadClientScene("MainConnection");
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            LoadClientScene("MainConnection");
            UnloadClientScene("Connected");
            UnloadClientScene("MainGame");
        }
    }

    private void OnServerConnectionStateChange(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            LoadScene("Connected");
            UnloadScene("MainConnection");
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            LoadScene("MainConnection");
            UnloadScene("Connected");
            UnloadScene("MainGame");
        }
    }

    public static void LoadGame()
    {
        LoadScene("MainGame");
        UnloadScene("Connected");
        LoadClientScene("MainGame"); // Alle Clients in die MainGame Szene schicken
    }

    public void EndGame()
    {
        LoadScene("Connected");
        UnloadScene("MainGame");
        LoadClientScene("Connected"); // Clients zurück in "Connected" bringen
    }

    static private void LoadScene(string sceneName)
    {
        if (!InstanceFinder.IsServer) return;

        SceneLoadData sld = new SceneLoadData(sceneName)
        {
            ReplaceScenes = ReplaceOption.All // Ersetzt alte Szenen, verhindert Stapelung
        };
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    static private void UnloadScene(string sceneName)
    {
        if (!InstanceFinder.IsServer) return;

        SceneUnloadData sld = new SceneUnloadData(sceneName);
        InstanceFinder.SceneManager.UnloadGlobalScenes(sld);
    }

    static private void LoadClientScene(string sceneName)
    {
        if (!InstanceFinder.IsServer) return;

        SceneLoadData sld = new SceneLoadData(sceneName)
        {
            ReplaceScenes = ReplaceOption.All
        };
        InstanceFinder.SceneManager.LoadConnectionScenes(sld);
    }

    static private void UnloadClientScene(string sceneName)
    {
        if (!InstanceFinder.IsServer) return;

        SceneUnloadData sld = new SceneUnloadData(sceneName);
        InstanceFinder.SceneManager.UnloadConnectionScenes(sld);
    }
}

