using UnityEngine;

/// <summary>
/// Handles detection of interactable objects in front of the player using raycasting.
/// </summary>
public class Interactor : MonoBehaviour
{
    public float interactRange = 1.5f; // Distance to check for interactables
    public LayerMask interactableMask; // Layer mask to filter interactable objects
    public PlayerMovement playerScript; // Reference to player movement component
    public UIManager uIManager; // Reference to UI manager for interaction indicators

    void Update()
    {
        // Ensure required components are assigned
        if (uIManager == null || playerScript == null)
        {
            return;
        }

        // Perform raycast to detect interactables in front of player
        RaycastHit hit = new();
        if(Physics.Raycast(transform.position, transform.forward, out hit, interactRange, interactableMask))
        {
            if(hit.collider != null)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if(interactable != null)
                {
                    // Valid interactable found - update target and show indicator
                    playerScript.currentTarget = interactable;
                    uIManager.isIndicating[playerScript.playerNumber - 1] = true;
                }
                else
                {
                    // Hit object is not interactable - clear target
                    playerScript.currentTarget = null;
                    uIManager.isIndicating[playerScript.playerNumber - 1] = false;
                }
            }
            else
            {
                playerScript.currentTarget = null;
                uIManager.isIndicating[playerScript.playerNumber - 1] = false;
            }
        }
        else
        {
            // No object detected by raycast - clear target and indicator
            uIManager.isIndicating[playerScript.playerNumber - 1] = false;
            playerScript.currentTarget = null;
        }
    }
}
