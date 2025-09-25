using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviourPun
{
    public float speed = 5f;
    public float rotationSpeed = 720f;
    public float elevationAmount = 0.1f;
    public float stepFrequency = 2f;
    public float gravity = -9.81f;
    public float stopSpeed = 20f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public DynamicJoystick joystick;
    public Button shootButton;
    public Button dashButton;

    private CharacterController controller;
    private Vector3 movementDirection;
    private Vector3 velocity;
    private Vector3 targetRotation;
    private float verticalVelocity;
    private float stepTimer;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private WeaponSystem weaponSystem;
    private Animator animator;   // referencia al Animator
    

    void Start()
    {
        if (!photonView.IsMine) return;
        controller = GetComponent<CharacterController>();
        weaponSystem = GetComponent<WeaponSystem>();
        animator = GetComponentInChildren<Animator>(); // 👈 busca el Animator en hijos

        // 🔹 Buscar los joysticks y botones que ya están en la escena
        if (joystick == null)
            joystick = FindObjectOfType<DynamicJoystick>();

        if (shootButton == null)
        {
            var shootObj = GameObject.Find("ShootButton"); // usa el nombre exacto
            if (shootObj != null) shootButton = shootObj.GetComponent<Button>();
        }

        if (dashButton == null)
        {
            var dashObj = GameObject.Find("DashButton"); // usa el nombre exacto
            if (dashObj != null) dashButton = dashObj.GetComponent<Button>();
        }

        // Solo agrega listeners si se encontraron los botones
        if (shootButton != null) shootButton.onClick.AddListener(OnShootButtonPressed);
        if (dashButton != null) dashButton.onClick.AddListener(OnDashButtonPressed);
    }

    void Update()
        
    {
        if (!photonView.IsMine) return;
        // Cooldown del dash
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        // Gestionar dash
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
            }
        }

        // Entrada del joystick
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Movimiento
        if (inputDirection.magnitude >= 0.1f && !isDashing)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            targetRotation = Vector3.up * targetAngle;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), rotationSpeed * Time.deltaTime);

            stepTimer += Time.deltaTime * stepFrequency;
            movementDirection = inputDirection;
            movementDirection.y = Mathf.Sin(stepTimer) * elevationAmount;
        }
        else if (!isDashing)
        {
            movementDirection = Vector3.Lerp(movementDirection, Vector3.zero, stopSpeed * Time.deltaTime);
            stepTimer = 0f;
        }

        // Gravedad
        if (controller.isGrounded && !isDashing)
            verticalVelocity = 0f;
        else if (!isDashing)
            verticalVelocity += gravity * Time.deltaTime;

        velocity = isDashing ? movementDirection * dashSpeed : movementDirection * speed;
        velocity.y += verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // 🔹 Actualizar parámetro Speed en Animator
        animator.SetFloat("Speed", inputDirection.magnitude);
    }

    void OnShootButtonPressed()
    {
        if (weaponSystem != null)
        {
            weaponSystem.Shoot();
        }

        // 🔹 Activar animación de disparo
        animator.SetTrigger("IsShooting");
    }

    void OnDashButtonPressed()
    {
        if (dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = 0f;
            movementDirection = transform.forward;
        }
    }
}
