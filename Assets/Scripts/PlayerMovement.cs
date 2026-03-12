    using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Multiplayer")]
    // Controller index used to bind input actions and UI elements to this player
    public int playerNumber;
    // Role assigned to this player (e.g. Human or Monster)
    public string role;
    [Header("Movement")]
    // Base movement speed applied as a physics force
    public float moveSpeed;

    [Header("Interaction")]
    // Camera attached to this player, used for look direction
    public GameObject cameraObject;
    // The interactable object currently in range of this player
    public IInteractable currentTarget;

    [Header("Debug")]
    // Logs the current interactable target to the console each frame when true
    public bool logInteractableObject;
    // Logs raw input values to the console each frame when true
    public bool logInputActions;
    
    [Header("Animation")]
    // Animator controlling this player's movement animations
    [SerializeField] 
    private Animator animator;

    [Header("Running")]
    // Whether the player is currently sprinting
    [SerializeField] private bool isRunning = false;
    // Speed multiplier applied to movement while sprinting
    [SerializeField] private float runSpeedMultiplier = 1.5f;
    // Current stamina level
    [SerializeField] private float stamina;
    // Maximum stamina the player can have
    [SerializeField] private float maxStamina = 100f;
    // Rates at which stamina drains while sprinting and recovers while not
    [SerializeField] private float staminaDrainRate, staminaRecoveryRate;
    // UI bars that visually represent each player's stamina
    [SerializeField] private RectTransform[] staminaBars;
    // UI labels that display each player's assigned role
    [SerializeField] private TextMeshProUGUI[] roleLabels;

    [Header(" ")]
    // Reference transform used as the forward and right direction for movement
    public Transform orientation;

    // Raw horizontal and vertical movement input values
    float horizontalInput;
    float verticalInput;

    // Input action for reading movement axes
    InputAction moveInputAction;
    // Computed world-space direction the player should move towards
    private Vector3 moveDirection;

    // Input action for triggering interactions
    InputAction interactInputAction;
    // Cached value of the interact input
    private float interactInput;

    // Input action for toggling sprinting
    InputAction runningInputAction;
    // Cached value of the sprint input
    private float runningInput;

    // Rigidbody used to apply physics-based movement
    Rigidbody rb;
    // Guards movement until the game officially starts
    [SerializeField]
    private bool canMove = false;
    // Prevents sprinting from resuming until stamina has partially recovered
    private bool staminaFullyDrained = false;

    // Caches the Rigidbody component on startup
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Binds the move, interact, and run input actions for this player's controller and sets the role label
    private void GetInputActions() {
        moveInputAction = InputSystem.actions.FindAction("move" + playerNumber);
        interactInputAction = InputSystem.actions.FindAction("interact" + playerNumber);
        runningInputAction = InputSystem.actions.FindAction("run" + playerNumber);
        roleLabels[playerNumber - 1].text = role;
    }

    // Runs input, running, interaction, speed control, animation, and debug checks each frame
    private void Update(){
        ControlRunning();
        MyInput();
        SpeedControl();
        Interact();
        Animate();
        Debugging();
    }

    // Triggers the current target's interaction while the interact button is held
    private void Interact()
    {
        if(currentTarget != null && interactInput == 1)
        {
            currentTarget.Interact(gameObject);
        }
    }

    // Logs debug info to the console when the corresponding debug flags are enabled
    private void Debugging()
    {
        if (logInteractableObject)
        {
            Debug.Log(currentTarget);
        }

        if(logInputActions)
        {
            Debug.Log(interactInput);
        }
    }

    // Pauses the animator when the player is stationary
    private void Animate()
    {
        if(animator != null)
        {
            animator.speed = rb.linearVelocity.magnitude > 0 ? 1 : 0;
        }
    }

    // Applies movement forces on the physics step when movement is allowed
    private void FixedUpdate(){
        if (canMove == true)
        {
            MovePlayer();
        }
    }

    // Reads and caches all input values for this frame
    private void MyInput() {
        if (moveInputAction == null || interactInputAction == null)
            return;

        horizontalInput = moveInputAction.ReadValue<Vector2>().x;
        verticalInput = moveInputAction.ReadValue<Vector2>().y;
        interactInput = interactInputAction.ReadValue<float>();
        runningInput = runningInputAction.ReadValue<float>();
    }

    // Allows the player to move once the game has started
    private void EnableMovement()
    {
        canMove = true;
    }

    // Applies a directional physics force based on input and move speed
    private void MovePlayer() {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * (isRunning == true ? runSpeedMultiplier : 1), ForceMode.Force);

        
    }

    // Manages sprint state, stamina drain and recovery, and stamina bar visuals
    private void ControlRunning()
    {
        isRunning = runningInput != 0f && stamina > 0 && staminaFullyDrained == false;
        staminaBars[playerNumber - 1].sizeDelta = new Vector2(stamina / maxStamina * 100 * 5, staminaBars[playerNumber - 1].sizeDelta.y);

        if (isRunning)
        {
            if(stamina > 0)
            {
                stamina -= staminaDrainRate;
            }
        }
        else
        {
            if(stamina < maxStamina)
            {
                stamina += staminaRecoveryRate;
            }
        }

        if(stamina <= 0)
        {
            staminaBars[playerNumber - 1].GetComponentInChildren<RawImage>().color = new Color(50, 0, 0);
            staminaFullyDrained = true;
        }

        if(stamina >= 50)
        {
            staminaBars[playerNumber - 1].GetComponentInChildren<RawImage>().color = new Color(170, 0, 0);
            staminaFullyDrained = false;
        }
    }

    // Clamps horizontal velocity to the maximum move speed
    private void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    
        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    // Subscribe to role and start events when this object becomes active
    void OnEnable()
    {
        EventManager.rolesRandomized += GetInputActions;
        EventManager.startGame += EnableMovement;
    }

    // Unsubscribe from role and start events when this object is deactivated
    void OnDisable()
    {
        EventManager.rolesRandomized -= GetInputActions;
        EventManager.startGame -= EnableMovement;
    }
}
