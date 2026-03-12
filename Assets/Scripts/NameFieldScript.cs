using TMPro;
using UnityEngine;

public class NameFieldScript : MonoBehaviour
{
    
    // Auto-focus the input field when this object becomes active
    void OnEnable()
    {
        gameObject.GetComponent<TMP_InputField>().Select();
    }

    // Broadcasts the current input field text as the player's name
    public void GetName()
    {
        EventManager.NameChanged.Invoke(gameObject.GetComponent<TMP_InputField>().text);
    }
}
