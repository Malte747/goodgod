using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Prefabs für verschiedene Rollen")]
    [SerializeField] private GameObject godUIPrefab;
    [SerializeField] private GameObject satanUIPrefab;
    [SerializeField] private GameObject discipleUIPrefab;
    [SerializeField] private GameObject spectatorUIPrefab;

    private GameObject currentUI; // Das individuelle UI für den Spieler

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton erstellen, damit es zentral bleibt
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUIForRole(string role)
    {
        // Zentrales Canvas in der Szene suchen
        GameObject globalCanvas = GameObject.Find("MainCanvas");
        if (globalCanvas == null)
        {
            Debug.LogError("GlobalUI Canvas nicht gefunden!");
            return;
        }

        // Falls bereits eine UI existiert, lösche sie
        if (currentUI != null)
        {
            Destroy(currentUI);
        }

        // Wähle das UI basierend auf der Rolle aus
        GameObject selectedUIPrefab = null;
        switch (role)
        {
            case "God":
                selectedUIPrefab = godUIPrefab;
                break;
            case "Satan":
                selectedUIPrefab = satanUIPrefab;
                break;
            case "Disciple":
                selectedUIPrefab = discipleUIPrefab;
                break;
            case "Spectator":
                selectedUIPrefab = spectatorUIPrefab;
                break;
            default:
                Debug.LogError("Unbekannte Rolle: " + role);
                return;
        }

        // Erstelle die UI für die Rolle als Kind des GlobalUI Canvas
        currentUI = Instantiate(selectedUIPrefab, globalCanvas.transform);
    }
}
