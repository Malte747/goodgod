using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerCountVariable : NetworkBehaviour
{
    public static PlayerCountVariable Instance { get; private set; }

    public readonly SyncVar<int> PlayerCount = new SyncVar<int>(new SyncTypeSettings(1f)); // SyncVar mit Callback

    [SerializeField] private TMP_Text playerCountText; // UI-Text zur Anzeige der Spielerzahl


    private void Awake()
    {
        PlayerCount.OnChange += on_player;
    }



    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        // Singleton-Instanz setzen
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Es gibt bereits eine Instanz von PlayerCountManager. Diese Instanz wird zerstört.");
            Destroy(gameObject);
        }
    }

    private void on_player(int prev, int next, bool asServer)
    {
        ChangeVariableText(next);
    }


    [ServerRpc(RequireOwnership = false)]
    public void IncreasePlayerCount()
    {
        if (!IsServer) return;
        PlayerCount.Value++;
        
        Debug.Log($"Spieler beigetreten. Aktuelle Anzahl: {PlayerCount.Value}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecreasePlayerCount()
    {
        
        PlayerCount.Value = Mathf.Max(0, PlayerCount.Value - 1);
        
        Debug.Log($"Spieler hat verlassen. Aktuelle Anzahl: {PlayerCount.Value}");
    }

    [ObserversRpc]
    public void ChangeVariableText(int count)
    {
        playerCountText.text = $"Players: {PlayerCount.Value}";
    }

}

