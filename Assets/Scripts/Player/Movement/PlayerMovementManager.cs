using System.Collections;
using UnityEngine;
using FishNet.Object;

public class PlayerMovementManager : NetworkBehaviour
{
    [SerializeField] private MovemnentSettings _settings = null;
    [SerializeField] private Camera playerCamera;

    private CharacterController _controller;
    private PlayerUIRole _roleScript;

    private Vector3 _moveDirection;
    private bool isJumping = false;

    private bool isGodMode = false; // Flag zur Unterscheidung der Bewegungsarten

    [SerializeField] private float hoverHeight = 5f; 
    [SerializeField] private float hoverSmoothness = 5f; 

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            enabled = false;
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
            Destroy(playerCamera.GetComponent<AudioListener>());
            return;
        }
        if (IsOwner)
        {
        Transform playerModel = transform.Find("Body"); // Ersetze "PlayerModel" mit dem Namen deines Modells
        if (playerModel != null)
        {
            playerModel.gameObject.SetActive(false);
        }
        }

        _controller = GetComponent<CharacterController>();
        _roleScript = GetComponent<PlayerUIRole>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CheckRole();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (isGodMode)
        {
            HandleGodMovement();
            MaintainHoverHeight();
        }
        else
        {
            HandlePlayerMovement();
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    private void CheckRole()
    {
        if (_roleScript == null)
        {
            Debug.LogError("PlayerUIRole script missing!");
            return;
        }

        string role = _roleScript.playerRole.ToLower();

        if (role == "god" || role == "spectator")
        {
            isGodMode = true;
        }
        else if (role == "satan" || role == "disciple")
        {
            isGodMode = false;
        }
        else
        {
            Debug.LogWarning($"Unknown role: {role}");
        }
    }

    private void HandlePlayerMovement()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (_controller.isGrounded)
        {
            isJumping = false;

            if (input.x != 0 && input.y != 0)
            {
                input *= 0.777f; 
            }

            _moveDirection = new Vector3(input.x * _settings.speed, -_settings.antiBump, input.y * _settings.speed);
            _moveDirection = transform.TransformDirection(_moveDirection);

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
        float airControlFactor = 0.5f;

        Vector3 airMove = new Vector3(input.x * (_settings.speed * airControlFactor), 0f, input.y * (_settings.speed * airControlFactor));
        airMove = transform.TransformDirection(airMove);

        _moveDirection.x = Mathf.Lerp(_moveDirection.x, airMove.x, Time.deltaTime * 2f);
        _moveDirection.z = Mathf.Lerp(_moveDirection.z, airMove.z, Time.deltaTime * 2f);
    }

    private void Jump()
    {
        if (!_controller.isGrounded || isJumping) return;

        _moveDirection.y = _settings.jumpForce;
        isJumping = true;
    }

    private void HandleGodMovement()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (input.x != 0 && input.y != 0)
        {
            input *= 0.777f; 
        }

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        _moveDirection = (forward * input.y + right * input.x) * _settings.speed;
    }

    private void MaintainHoverHeight()
    {
        float targetY = hoverHeight;
        float currentY = transform.position.y;
        float newY = Mathf.Lerp(currentY, targetY, hoverSmoothness * Time.deltaTime);

        Vector3 position = transform.position;
        position.y = newY;
        transform.position = position;
    }
}
