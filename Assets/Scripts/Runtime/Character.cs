using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private Animator animator;

    private bool isJumping = false;

    private float jumpCooldownTimer;

    private CharacterController controller;

    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField] private float maxHealth = 100.0f;
    private float currentHealth;

    [SerializeField]
    private bool passiveHealActive = false;

    [SerializeField]
    private float heal = 5;

    [SerializeField]
    private float jumpCooldown;

    [SerializeField]
    private float characterSpeed;

    [SerializeField]
    private float dampening;

    [SerializeField]
    private float gravity;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float platformRayDistance;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private AudioClip walkingClip;

    [SerializeField]
    private AudioClip jumpingClip;

    private Vector3 characterMovement;
    private Vector3 jumpVelocity;
    private Vector3 platformVelocity;
    private Vector3 characterGravity;

    private AudioSource audioSource;

    private ParticleSystem walkDust;

    private void Start()
    {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;

        this.currentHealth = this.maxHealth;

        this.animator = this.GetComponent <Animator>();
        GameObject particleGameObject = GameObject.Find("WalkingDust");
        this.walkDust = particleGameObject.GetComponent<ParticleSystem>();

        audioSource = this.GetComponent<AudioSource>();
        this.audioSource.loop = true;
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f)
        {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;
        }

        if (this.controller.isGrounded && !this.isJumping && this.jumpAction.WasPressedThisFrame())
        {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;
        }

        if (this.jumpVelocity.y > 0.0f)
        {
            this.jumpVelocity.y -= Time.fixedDeltaTime;
        }
        else
        {
            this.jumpVelocity = Vector3.zero;
        }

        this.jumpCooldownTimer -= Time.fixedDeltaTime;
    }

    private void HandlePlatforms()
    {
        this.platformVelocity = Vector3.zero;
        if(this.controller.isGrounded 
            && Physics.Raycast(this.transform.position, Vector3.down, out var hit, this.platformRayDistance, LayerMask.GetMask("Platforms"))) {
            var platformObject = hit.collider.gameObject;
            var movingPlatform = platformObject.GetComponent<MovingPlatform>();
            if(movingPlatform != null)
            {
                this.platformVelocity = movingPlatform.GetVelocity();
            }
        }
    }

    private void FixedUpdate()
    {
        this.HandleJumping();
        this.HandlePlatforms();

        var inputMovement = this.moveAction.ReadValue<Vector2>();

        var inputRightDirection = this.cameraTransform.right;
        var inputForwardDirection = this.cameraTransform.forward;

        inputRightDirection.y = 0f;
        inputForwardDirection.y = 0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        //Since we do not use the physics system, we have to simulate gravity ourselves
        if (this.controller.isGrounded) {
            this.characterGravity.y = 0.0f;
        }

        this.characterGravity.y += this.gravity * Time.fixedDeltaTime;
        this.characterMovement += this.characterGravity * Time.fixedDeltaTime;
        this.characterMovement += this.jumpVelocity * Time.fixedDeltaTime;
        this.characterMovement += inputRightDirection * inputMovement.x * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement += inputForwardDirection * inputMovement.y * this.characterSpeed * Time.fixedDeltaTime;

        this.characterMovement *= (1.0f - this.dampening);

        Vector3 characterForward = this.characterMovement;
        characterForward.y = 0.0f;

        if(characterForward.sqrMagnitude > 0.0f && characterForward != Vector3.zero) {
            this.transform.forward = characterForward.normalized;
        }

        this.controller.Move(this.characterMovement + this.platformVelocity * Time.fixedDeltaTime);

        this.SetAnimationState(inputMovement);

        this.ManageAudio(inputMovement);

        if (inputMovement != Vector2.zero)
        {
            walkDust.Play();
        }
        else
        {
            walkDust.Stop();
        }

        if (passiveHealActive) PassiveHeal();
    }

    public void Damage(float damage)
    {
        this.currentHealth = currentHealth - damage;
        this.currentHealth = Mathf.Clamp(currentHealth, 0.0f, maxHealth);
    }

    public float GetCurrentHealth() => this.currentHealth;
    public float GetMaxHealth() => this.maxHealth;
    public float GetHealth() => this.currentHealth / this.maxHealth;

    public float SetHealth(float health) => this.currentHealth = health;

    public void PassiveHeal()
    {
        currentHealth = currentHealth + heal;
        this.currentHealth = Mathf.Clamp(currentHealth, 0.0f, maxHealth);
    }

    void SetAnimationState(Vector2 inputMovement)
    {
        this.animator.SetBool("IsJumping", this.isJumping);
        this.animator.SetBool("IsRunning", inputMovement != Vector2.zero);
        this.animator.SetFloat("MovementForward", inputMovement.magnitude);
    }

    void ManageAudio(Vector2 inputMovement)
    {
        if (this.isJumping)
        {
            this.audioSource.Stop();
            this.audioSource.clip = jumpingClip;
            this.audioSource.Play();
        }

        if(inputMovement != Vector2.zero && audioSource.clip != walkingClip)
        {
            this.audioSource.Stop();
            this.audioSource.clip = walkingClip;
            this.audioSource.Play();
        }

        if(inputMovement == Vector2.zero)
        {
            this.audioSource.Stop();
            this.audioSource.clip = null;
        }
    }
}
