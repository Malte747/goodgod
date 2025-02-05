using UnityEngine;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;

public class BootstrapSceneManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
        InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState += OnClientConnectionStateChange;
        InstanceFinder.NetworkManager.ServerManager.OnServerConnectionState += OnServerConnectionStateChange;
    }

    private void OnDestroy()
    {
        // Unsubscribe, um Memory Leaks zu vermeiden
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
            LoadScene("Connected");
            UnloadScene("MainConnection");
            
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            LoadScene("MainConnection");
            UnloadScene("Connected");
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
        }
    }

    static private void LoadScene(string sceneName)
    {
        if (!InstanceFinder.IsServer)
            return;

        SceneLoadData sld = new SceneLoadData(sceneName);
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    static private void UnloadScene(string sceneName)
    {
        if (!InstanceFinder.IsServer)
            return;

        SceneUnloadData sld = new SceneUnloadData(sceneName);
        InstanceFinder.SceneManager.UnloadGlobalScenes(sld);
    }
}

