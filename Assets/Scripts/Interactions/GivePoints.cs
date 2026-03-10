using System.Collections;
using UnityEngine;

public class GivePoints : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string objectName;
    [SerializeField]
    private int points;

    [SerializeField]
    private AudioClip[] pickupSounds;
    public void Interact(GameObject interactor){
        if (interactor.CompareTag("Player"))
        {
            EventManager.GivePoints?.Invoke(points);
            PickupSound();
            StartCoroutine(StartDestruction());
        }
    }

    IEnumerator StartDestruction() { 
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void PickupSound() 
    { 
        GetComponent<AudioSource>().PlayOneShot(pickupSounds[Random.Range(0, pickupSounds.Length)]);
    }

    public string GetPrompt() => "Press E to climb!";
}
