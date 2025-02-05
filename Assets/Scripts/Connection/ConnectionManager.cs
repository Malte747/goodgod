using Heathen.SteamworksIntegration;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    private static ConnectionManager instance;
    private string _hostHex;

    [SerializeField] private TMP_InputField connectionInput; // UI-Element
    [SerializeField] private Button hostButton;

    private FishySteamworks.FishySteamworks FishySteamworks => FishySteamManager.Instance?.fishySteamworks;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUIReferences();
    }

    private void FindUIReferences()
    {
        connectionInput = FindObjectOfType<TMP_InputField>(); // Sucht das Input-Feld in der Szene

        if (connectionInput == null)
        {
            Debug.LogError("Connection Input Field wurde nicht gefunden!");
        }

        if (hostButton == null)
        {
            hostButton = GameObject.Find("HostButton")?.GetComponent<Button>();
            if (hostButton != null)
            {
                hostButton.onClick.RemoveAllListeners(); // Verhindert doppelte Events
                hostButton.onClick.AddListener(StartHost);
                Debug.Log("HostButton wieder verbunden!");
            }
            else
            {
                Debug.LogWarning("Host Button nicht gefunden!");
            }
        }
    }

    public void StartHost()
    {
        if (FishySteamworks == null)
        {
            Debug.LogError("FishySteamworks nicht gefunden!");
            return;
        }

        var user = UserData.Get();
        _hostHex = user.ToString();

        FishySteamworks.StartConnection(true);
        FishySteamworks.StartConnection(false);
    }

    public void StartConnection()
    {
        if (connectionInput == null)
        {
            Debug.LogError("Connection Input Field ist nicht verbunden!");
            return;
        }

        _hostHex = connectionInput.text;
        var hostUser = UserData.Get(_hostHex);

        if (!hostUser.IsValid)
        {
            Debug.LogError("Hostuser ist nicht gültig!");
            return;
        }

        FishySteamworks.SetClientAddress(hostUser.id.ToString());
        FishySteamworks.StartConnection(false);
    }

    public static string GetHostHex()
    {
        Debug.Log(instance._hostHex + " ID des Spielers");
        return instance._hostHex;
    }
}
