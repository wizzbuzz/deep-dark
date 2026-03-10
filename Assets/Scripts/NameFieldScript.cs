using TMPro;
using UnityEngine;

public class NameFieldScript : MonoBehaviour
{
    
    void OnEnable()
    {
        gameObject.GetComponent<TMP_InputField>().Select();
    }

    public void GetName()
    {
        EventManager.NameChanged.Invoke(gameObject.GetComponent<TMP_InputField>().text);
    }
}
