using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Root objects for the main menu and lobby screens
    public GameObject menu, lobby;
    // Tracks whether each player has set their name before starting
    public bool player1ChangedName = false;
    public bool player2ChangedName = false;
    // Load the main game scene
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    // Switch from the main menu to the lobby screen
    public void Lobby()
    {
        menu.SetActive(false);
        lobby.SetActive(true);
    }

    // Exit the application
    public void Quit()
    {
        Application.Quit();
    }

    // Hide the main menu to show the how-to-play screen
    public void HowToPlay()
    {
        menu.SetActive(false);
    }
}
