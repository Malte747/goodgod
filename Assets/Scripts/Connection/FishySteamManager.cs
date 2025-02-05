using UnityEngine;
using FishySteamworks;

public class FishySteamManager : MonoBehaviour
{
    public static FishySteamManager Instance;
    public FishySteamworks.FishySteamworks fishySteamworks; // Manuell in Unity zuweisen

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
