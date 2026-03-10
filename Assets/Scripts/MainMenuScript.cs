using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject menu, lobby;
    public bool player1ChangedName = false;
    public bool player2ChangedName = false;
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Lobby()
    {
        menu.SetActive(false);
        lobby.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void HowToPlay()
    {
        menu.SetActive(false);
    }
}
