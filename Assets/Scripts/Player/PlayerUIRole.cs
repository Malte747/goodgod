using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class PlayerUIRole : NetworkBehaviour
{
    [SerializeField] private string playerRole; // Diese Rolle wird beim Spawn zugewiesen

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner) 
        {
            UIManager.Instance.SetUIForRole(playerRole);
        }
    }
}
