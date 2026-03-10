using UnityEngine;

public class LadderExit : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor){
        if (interactor == null)
        {
            return;
        }

        if (interactor.CompareTag("Player"))
        {
            GameDetails.Instance.SetWinner(GameDetails.player.Human);
            EventManager.gameOver?.Invoke();
            EventManager.playerReachedLadder?.Invoke();
        }
    }
}
