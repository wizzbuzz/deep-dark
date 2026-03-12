using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI panels for the loading and end-of-match screens
    [SerializeField]
    private GameObject loadingScreen, endScreen;

    // Crosshair objects for the left and right player viewports
    [SerializeField]
    private GameObject crosshairPrefabLeft, crosshairPrefabRight;

    [Header("UI Elements")]
    // Displays the winner label on the end screen
    [SerializeField]
    private TextMeshProUGUI winnerText;
    // Displays the final score on the end screen
    [SerializeField]
    private TextMeshProUGUI scoreText;
    // Root object for all in-game HUD elements
    [SerializeField]
    private GameObject ingameUI;

    [Header("Tutorial")]
    // Index of the currently displayed tutorial slide
    [SerializeField] private int tutorialTextPosition = -1;
    // Panel that contains the tutorial overlay
    [SerializeField] private GameObject tutorialPanel;
    // Ordered array of tutorial text slides
    [SerializeField] private TextMeshProUGUI[] tutorialTextElements;

    // Whether each player's crosshair is currently in the enlarged indicating state
    public bool[] isIndicating = {false, false};
    // Input action used to advance tutorial text
    private InputAction interactInputAction;
    // Cached value of the interact input axis
    private float interactInput;
    // When true, both crosshairs are centred instead of split to each viewport
    [SerializeField] private bool isSingleplayer = false;

    // Positions each crosshair at the correct screen location for split-screen or single-player
    private void PlaceCrosshairs()
    {
        float middleHeight = Screen.height / 2;
        float leftX = Screen.width / 4;
        float rightX = Screen.width / 4 * 3;

        if (!isSingleplayer)
        {
            crosshairPrefabLeft.transform.position = new Vector3(leftX, middleHeight, 0);
            crosshairPrefabRight.transform.position = new Vector3(rightX, middleHeight, 0);
        }else
        {
            crosshairPrefabLeft.transform.position = new Vector3(Screen.width / 2, middleHeight, 0);
            crosshairPrefabRight.transform.position = new Vector3(Screen.width / 2, middleHeight, 0);

        }
    }

    // Resizes crosshairs based on indication state and advances the tutorial on input release
    private void Update()
    {
        crosshairPrefabLeft.GetComponent<RectTransform>().sizeDelta = isIndicating[0] ? new Vector2(4, 4) : new Vector2(2, 2);
        crosshairPrefabRight.GetComponent<RectTransform>().sizeDelta = isIndicating[1] ? new Vector2(4, 4) : new Vector2(2, 2);

        if (interactInputAction.WasReleasedThisFrame())
        {
            NextTutorialText();
        }
    }

    // Places crosshairs then deactivates the loading screen
    private void HideLoadingScreen()
    {
        PlaceCrosshairs();
        loadingScreen.SetActive(false);
    }

    // Fetches the interact input action for player 1
    void Start()
    {
        interactInputAction = InputSystem.actions.FindAction("interact" + 1);
    }

    // Unlocks the cursor and populates the end screen with the winner and final score
    private void ShowEndScreen()
    {
        // Unlock the cursor and show the end screen
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Change winner label
        winnerText.text = GameDetails.Instance.winner != GameDetails.player.Human ? "The Beast wins!" : "The Human escaped!";

        scoreText.text = $"Final Score: {MatchData.Instance.endScore}";

        endScreen.SetActive(true);
    }

    // Activates the tutorial panel and shows the first slide
    private void ShowTutorial()
    {
        Debug.Log("Showing tutorial.");
        tutorialTextPosition = 0;
        tutorialTextElements[tutorialTextPosition].gameObject.SetActive(true);
        tutorialPanel.SetActive(true);
    }

    // Advances to the next tutorial slide, or ends the tutorial if on the last one
    private void NextTutorialText()
    {
        if (tutorialPanel.activeSelf)
        {
            if (tutorialTextPosition < tutorialTextElements.Length - 1)
            {
                tutorialTextElements[tutorialTextPosition].gameObject.SetActive(false);
                tutorialTextPosition++;
                tutorialTextElements[tutorialTextPosition].gameObject.SetActive(true);
            }
            else
            {
                tutorialPanel.SetActive(false);
                EventManager.tutorialEnded?.Invoke();
            }
        }
    }

    // Subscribe to UI-related events when this object becomes active
    void OnEnable()
    {
        EventManager.hideLoadingScreen += HideLoadingScreen;
        EventManager.showEndScreen += ShowEndScreen;
        EventManager.showTutorial += ShowTutorial;
        EventManager.tutorialEnded += ShowIngameUI;
        EventManager.gameOver += HideIngameUI;
    }

    // Unsubscribe from UI-related events when this object is deactivated
    void OnDisable()
    {
        EventManager.hideLoadingScreen -= HideLoadingScreen;
        EventManager.showEndScreen -= ShowEndScreen;
        EventManager.showTutorial -= ShowTutorial;
        EventManager.tutorialEnded -= ShowIngameUI;
        EventManager.gameOver -= HideIngameUI;
    }

    // Hides the in-game HUD
    private void HideIngameUI()
    {
        ingameUI.SetActive(false);
    }

    // Shows the in-game HUD
    private void ShowIngameUI()
    {
        ingameUI.SetActive(true);
    }
}
