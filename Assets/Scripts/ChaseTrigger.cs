using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.playerEnteredChase.Invoke();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.playerEscapedChase.Invoke();
        }
    }
}
