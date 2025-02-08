using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elemente für verschiedene Rollen")]
    [SerializeField] private GameObject godUI;
    [SerializeField] private GameObject satanUI;
    [SerializeField] private GameObject discipleUI;
    [SerializeField] private GameObject spectatorUI;

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;  // Singleton-Pattern
    }

    public void SetUIForRole(string role)
    {
        // Alle UI-Elemente deaktivieren
        godUI.SetActive(false);
        satanUI.SetActive(false);
        discipleUI.SetActive(false);
        spectatorUI.SetActive(false);

        // Entsprechendes UI-Element aktivieren
        switch (role)
        {
            case "God":
                godUI.SetActive(true);
                break;
            case "Satan":
                satanUI.SetActive(true);
                break;
            case "Disciple":
                discipleUI.SetActive(true);
                break;
            case "Spectator":
                spectatorUI.SetActive(true);
                break;
            default:
                Debug.LogError("Unbekannte Rolle: " + role);
                break;
        }
    }
}
