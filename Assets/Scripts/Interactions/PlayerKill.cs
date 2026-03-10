using UnityEngine;

public class PlayerKill : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor){
        if (interactor.CompareTag("Monster"))
        {
            GameDetails.Instance.SetWinner(GameDetails.player.Monster);
            EventManager.gameOver?.Invoke();
        }
    }

    public string GetPrompt() => "Press E to climb!";
}