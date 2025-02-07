using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerHealth : NetworkBehaviour
{
    public readonly SyncVar<bool> alive = new SyncVar<bool>(true); // Ersetzt health durch alive (bool)

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<PlayerHealth>().enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateHealth();
        }


    }

    [ServerRpc]
    public void UpdateHealth()
    {
        alive.Value = false; // Setzt alive auf false
        DestroyPlayer();
    }

    // Funktion zum Zerstören des Players
    private void DestroyPlayer()
    {

            Despawn(gameObject);

    }
}





