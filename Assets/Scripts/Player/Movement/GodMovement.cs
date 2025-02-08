using UnityEngine;
using FishNet.Object;

public class GodMovement : NetworkBehaviour
{
    [SerializeField] private MovemnentSettings _settings = null;

    private Vector3 _moveDirection;
    private CharacterController _controller;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private float hoverHeight = 5f; // Höhe, auf der der Spieler schweben soll
    [SerializeField] private float hoverSmoothness = 5f; // Wie schnell sich die Höhe anpasst

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            enabled = false;
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
            Destroy(playerCamera.GetComponent<AudioListener>());
        }
        if (IsOwner)
    {
        Transform playerModel = transform.Find("Body"); // Ersetze "PlayerModel" mit dem Namen deines Modells
        if (playerModel != null)
        {
            playerModel.gameObject.SetActive(false);
        }
    }
    }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        MaintainHoverHeight();
    }

    private void LateUpdate()
    {
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (input.x != 0 && input.y != 0)
        {
            input *= 0.777f; // Diagonalbewegung ausbalancieren
        }

        // Bewegungsrichtung basierend auf der Kamera
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // Die Y-Komponente entfernen, damit der Spieler nicht nach oben/unten fliegt
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Bewegung basierend auf Eingabe und Kameraausrichtung
        _moveDirection = (forward * input.y + right * input.x) * _settings.speed;
    }

    private void MaintainHoverHeight()
    {
        float targetY = hoverHeight; // Zielhöhe
        float currentY = transform.position.y;
        float newY = Mathf.Lerp(currentY, targetY, hoverSmoothness * Time.deltaTime);

        Vector3 position = transform.position;
        position.y = newY;
        transform.position = position;
    }
}
