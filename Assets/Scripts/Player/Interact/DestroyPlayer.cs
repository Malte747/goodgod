using UnityEngine;
using FishNet.Object;

public class DestroyPlayer : NetworkBehaviour, IInteractable
{
    public void Interact()
    {
        // Standard-Interaktion (wird nicht mehr direkt genutzt)
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyPlayerObject()
    {
        if (IsServer)
        {
            Destroy(gameObject); // Spielerobjekt entfernen
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyInteractableObject()
    {
        if (IsServer)
        {
            gameObject.SetActive(false); // Nur deaktivieren, falls Objekt später respawned
        }
    }
}
