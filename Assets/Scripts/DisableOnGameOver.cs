using UnityEngine;

public class DisableOnGameOver : MonoBehaviour
{
    // Components to disable when the game ends
    [SerializeField]
    private MonoBehaviour[] listToDisable;
    // Disables all listed components when the game-over event fires
    private void GameOver()
    {
        foreach(MonoBehaviour obj in listToDisable)
        {
            obj.enabled = false;
        }
    }

    // Subscribe to the game-over event when this object becomes active
    void OnEnable()
    {
        EventManager.gameOver += GameOver;
    }

    // Unsubscribe from the game-over event when this object is deactivated
    void OnDisable()
    {
        EventManager.gameOver -= GameOver;
    }
}
