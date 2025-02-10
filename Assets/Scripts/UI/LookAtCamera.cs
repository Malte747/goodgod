using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cam;

    void LateUpdate()
    {
        if (cam == null) 
        {
            cam = Camera.main?.transform; // Hol die Kamera erst, wenn sie verfügbar ist
            Debug.Log("keine Kamera gefunden");
        }

        if (cam != null)
        {
            transform.LookAt(transform.position + cam.forward); // Nametag immer zur Kamera ausrichten
            Debug.Log("Kamera gefunden");
        }
    }
}
