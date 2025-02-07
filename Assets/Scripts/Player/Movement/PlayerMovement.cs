using System.Collections;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private MovemnentSettings _settings = null;

    private Vector3 _moveDirection;
    private CharacterController _controller;
    private HeadBob headBob;

    [SerializeField] private Camera playerCamera;
    
    private bool isJumping = false; // Um zu tracken, ob Spieler gerade springt

    void Awake()
    {
        headBob = GetComponent<HeadBob>();
    }

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
    }

    private void LateUpdate()
    {
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (_controller.isGrounded)
        {
            isJumping = false; // Spieler ist nicht mehr in der Luft

            if (input.x != 0 && input.y != 0)
            {
                input *= 0.777f; // Diagonalbewegung ausbalancieren
            }

            _moveDirection = new Vector3(input.x * _settings.speed, -_settings.antiBump, input.y * _settings.speed);
            _moveDirection = transform.TransformDirection(_moveDirection);

            headBob.NormFrequency();

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            ApplyAirControl(input);
            _moveDirection.y -= _settings.gravity * Time.deltaTime;
        }
    }

    private void ApplyAirControl(Vector2 input)
    {
        // Erlaube Luftsteuerung, aber schwächer als am Boden
        float airControlFactor = 0.5f; // 50% der normalen Kontrolle

        Vector3 airMove = new Vector3(input.x * (_settings.speed * airControlFactor), 0f, input.y * (_settings.speed * airControlFactor));
        airMove = transform.TransformDirection(airMove);

        // X- und Z-Bewegung in der Luft anpassen, aber nicht abrupt überschreiben
        _moveDirection.x = Mathf.Lerp(_moveDirection.x, airMove.x, Time.deltaTime * 2f);
        _moveDirection.z = Mathf.Lerp(_moveDirection.z, airMove.z, Time.deltaTime * 2f);
    }

    private void Jump()
    {
        if (!_controller.isGrounded || isJumping) return; // Kein Doppelsprung

        _moveDirection.y = _settings.jumpForce;
        isJumping = true; // Verhindert mehrfaches Springen in der Luft
    }
}
