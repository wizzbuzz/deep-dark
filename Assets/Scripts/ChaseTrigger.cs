using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    // Fire the chase-started event when the player enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.playerEnteredChase.Invoke();
        }

    }
    // Fire the chase-escaped event when the player leaves the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.playerEscapedChase.Invoke();
        }
    }
}
