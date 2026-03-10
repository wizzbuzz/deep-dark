    using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Multiplayer")]
    public int playerNumber;
    public string role;
    [Header("Movement")]
    public float moveSpeed;

    [Header("Interaction")]
    public GameObject cameraObject;
    public IInteractable currentTarget;

    [Header("Debug")]
    public bool logInteractableObject;
    public bool logInputActions;
    
    [Header("Animation")]
    [SerializeField] 
    private Animator animator;

    [Header("Running")]
    [SerializeField] private bool isRunning = false;
    [SerializeField] private float runSpeedMultiplier = 1.5f;
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate, staminaRecoveryRate;
    [SerializeField] private RectTransform[] staminaBars;
    [SerializeField] private TextMeshProUGUI[] roleLabels;

    [Header(" ")]
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    InputAction moveInputAction;
    private Vector3 moveDirection;

    InputAction interactInputAction;
    private float interactInput;

    InputAction runningInputAction;
    private float runningInput;

    Rigidbody rb;
    [SerializeField]
    private bool canMove = false;
    private bool staminaFullyDrained = false;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void GetInputActions() {
        moveInputAction = InputSystem.actions.FindAction("move" + playerNumber);
        interactInputAction = InputSystem.actions.FindAction("interact" + playerNumber);
        runningInputAction = InputSystem.actions.FindAction("run" + playerNumber);
        roleLabels[playerNumber - 1].text = role;
    }

    private void Update(){
        ControlRunning();
        MyInput();
        SpeedControl();
        Interact();
        Animate();
        Debugging();
    }

    private void Interact()
    {
        if(currentTarget != null && interactInput == 1)
        {
            currentTarget.Interact(gameObject);
        }
    }

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

    private void Animate()
    {
        if(animator != null)
        {
            animator.speed = rb.linearVelocity.magnitude > 0 ? 1 : 0;
        }
    }

    private void FixedUpdate(){
        if (canMove == true)
        {
            MovePlayer();
        }
    }

    private void MyInput() {
        if (moveInputAction == null || interactInputAction == null)
            return;

        horizontalInput = moveInputAction.ReadValue<Vector2>().x;
        verticalInput = moveInputAction.ReadValue<Vector2>().y;
        interactInput = interactInputAction.ReadValue<float>();
        runningInput = runningInputAction.ReadValue<float>();
    }

    private void EnableMovement()
    {
        canMove = true;
    }

    private void MovePlayer() {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * (isRunning == true ? runSpeedMultiplier : 1), ForceMode.Force);

        
    }

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

    private void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    
        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    void OnEnable()
    {
        EventManager.rolesRandomized += GetInputActions;
        EventManager.startGame += EnableMovement;
    }

    void OnDisable()
    {
        EventManager.rolesRandomized -= GetInputActions;
        EventManager.startGame -= EnableMovement;
    }
}
