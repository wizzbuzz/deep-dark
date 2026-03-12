using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles player camera rotation and splitscreen viewport setup
public class PlayerCamera : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public float sensX; // Horizontal look sensitivity
    public float sensY; // Vertical look sensitivity

    public Transform orientation; // Player body orientation transform

    float xRotation; // Vertical camera rotation
    float yRotation; // Horizontal camera rotation

    InputAction lookAction; // Input action for camera look
    private bool isIndicating;
    private bool canMove = false;

    [SerializeField] private bool isSinglePlayer = false;

    // Configures camera viewport for splitscreen and binds input action
    private void SetupCamera()
    {
        lookAction = InputSystem.actions.FindAction("look" + playerMovement.playerNumber);
        if (!isSinglePlayer)
        {
            gameObject.GetComponent<Camera>().rect = new Rect((playerMovement.playerNumber - 1) * .5f, 0, 0.5f, 1);
        }
        else
        {
            gameObject.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        }
    }

    // Lock and hide cursor on game start
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (lookAction == null)
            return;

        if (canMove == true) { 
        
            // Read mouse/controller input
            Vector2 lookValue = lookAction.ReadValue<Vector2>();

            float mouseX = lookValue.x * sensX;
            float mouseY = lookValue.y * sensY;

            // Calculate rotation with vertical clamping
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Apply rotations to camera and player orientation
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        // Indicate
        if (isIndicating)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime * 1.5f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 1f);

            if(Vector3.Distance(transform.localPosition, Vector3.zero) < .2f)
            {
                isIndicating = false;
                gameObject.GetComponent<IgnoreLayer>().Hide();
            }

        }
    }
    private void EnableMovement()
    {
        canMove = true;
    }

    void Indicate()
    {
        transform.localPosition = new Vector3(0, 4, 0);
        transform.localRotation = Quaternion.Euler(90, 0, 0);
        isIndicating = true;
    }

    void OnEnable()
    {
        EventManager.rolesRandomized += SetupCamera;
        EventManager.showTutorial += Indicate;
        EventManager.startGame += EnableMovement;

    }

    void OnDisable()
    {
        EventManager.rolesRandomized -= SetupCamera;
        EventManager.showTutorial -= Indicate;
        EventManager.startGame -= EnableMovement;

    }



}
