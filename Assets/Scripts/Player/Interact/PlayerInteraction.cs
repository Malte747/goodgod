using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera playerCamera;

    private bool isCooldown = false; // Cooldown für "Satan"-Spieler
    [SerializeField] private bool canDestroyOnce = false; // Einmalige Zerstörung für "Disciple"

    private GameObject killPlayerUI; // UI für das Töten eines Spielers
    private GameObject interactableObjectUI; // UI für interagierbare Objekte

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            StartCoroutine(FindUIElements());
        }
    }

    private IEnumerator FindUIElements()
    {
        while (UIManager.Instance == null)
        {
            yield return null;
        }

        killPlayerUI = GameObject.Find("KillPlayer");
        interactableObjectUI = GameObject.Find("InteractableObject");

        if (killPlayerUI != null) killPlayerUI.SetActive(false);
        if (interactableObjectUI != null) interactableObjectUI.SetActive(false);
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            TryInteract();
        }

        // UI aktivieren/deaktivieren je nach Spieler oder Objekt in Reichweite
        CheckForInteractableObjects();
    }

    private void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            NetworkObject netObj = hit.collider.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                int objectLayer = hit.collider.gameObject.layer;
                InteractWithObjectServerRpc(netObj, objectLayer, gameObject.tag);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
private void InteractWithObjectServerRpc(NetworkObject targetObject, int objectLayer, string playerTag)
{
    if (targetObject == null) return;

    DestroyPlayer destroyScript = targetObject.GetComponent<DestroyPlayer>();
    if (destroyScript == null) return;

    if (objectLayer == LayerMask.NameToLayer("Player"))
    {
        if (playerTag == "Satan" && !isCooldown)
        {
            destroyScript.DestroyPlayerObject();
            StartCoroutine(StartCooldown());
        }
        else if (playerTag == "Disciple" && canDestroyOnce)
        {
            destroyScript.DestroyPlayerObject();
            canDestroyOnce = false;
        }
    }
    else if (objectLayer == LayerMask.NameToLayer("InteractableObject"))
    {
        destroyScript.DestroyInteractableObject();
        DisableObjectForAllClientsObserversRpc(targetObject);
    }
}

[ObserversRpc]
private void DisableObjectForAllClientsObserversRpc(NetworkObject targetObject)
{
    if (targetObject != null)
    {
        targetObject.gameObject.SetActive(false);
    }
}

    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(10f);
        isCooldown = false;
    }

    public void EnableDiscipleDestroy() 
    {
        canDestroyOnce = true; // Externe Aktivierung für einmalige Zerstörung
    }

    private void CheckForInteractableObjects()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        bool playerInRange = Physics.Raycast(ray, out hit, interactDistance) &&
                             hit.collider.gameObject.layer == LayerMask.NameToLayer("Player");

        bool interactableInRange = Physics.Raycast(ray, out hit, interactDistance) &&
                                   hit.collider.gameObject.layer == LayerMask.NameToLayer("InteractableObject");

        bool canDestroy = (gameObject.tag == "Satan" && !isCooldown) || 
                          (gameObject.tag == "Disciple" && canDestroyOnce);

        if (killPlayerUI != null)
        {
            killPlayerUI.SetActive(playerInRange && canDestroy);
        }

        if (interactableObjectUI != null)
        {
            interactableObjectUI.SetActive(interactableInRange);
        }
    }
}
