using UnityEngine;

public class DisableOnGameOver : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour[] listToDisable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void GameOver()
    {
        foreach(MonoBehaviour obj in listToDisable)
        {
            obj.enabled = false;
        }
    }

    void OnEnable()
    {
        EventManager.gameOver += GameOver;
    }

    void OnDisable()
    {
        EventManager.gameOver -= GameOver;
    }
}
