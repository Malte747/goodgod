using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object;
using System.Collections;

public class PlayerUIRole : NetworkBehaviour
{
    [SerializeField] public string playerRole; 

IEnumerator WaitForUIManager()
{
    while (UIManager.Instance == null)
    {
        yield return null;
    }

    // Stelle sicher, dass nur die UI des lokalen Spielers geändert wird
    if (IsOwner)
    {
        UIManager.Instance.SetUIForRole(playerRole);
    }
}

public override void OnStartClient()
{
    base.OnStartClient();
    StartCoroutine(WaitForUIManager());
}

}