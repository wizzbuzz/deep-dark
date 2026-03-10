using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreen, endScreen;

    [SerializeField]
    private GameObject crosshairPrefabLeft, crosshairPrefabRight;

    [Header("UI Elements")]
    [SerializeField]
    private TextMeshProUGUI winnerText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private GameObject ingameUI;

    [Header("Tutorial")]
    [SerializeField] private int tutorialTextPosition = -1;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI[] tutorialTextElements;

    public bool[] isIndicating = {false, false};
    private InputAction interactInputAction;
    private float interactInput;
    [SerializeField] private bool isSingleplayer = false;

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

    private void Update()
    {
        crosshairPrefabLeft.GetComponent<RectTransform>().sizeDelta = isIndicating[0] ? new Vector2(4, 4) : new Vector2(2, 2);
        crosshairPrefabRight.GetComponent<RectTransform>().sizeDelta = isIndicating[1] ? new Vector2(4, 4) : new Vector2(2, 2);

        if (interactInputAction.WasReleasedThisFrame())
        {
            NextTutorialText();
        }
    }

    private void HideLoadingScreen()
    {
        PlaceCrosshairs();
        loadingScreen.SetActive(false);
    }

    void Start()
    {
        interactInputAction = InputSystem.actions.FindAction("interact" + 1);
    }

    private void ShowEndScreen()
    {
        // Unlock the cursor and show the end screen
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Change winner label
        winnerText.text = GameDetails.Instance.winner != GameDetails.player.Human ? "The Monster eats..." : "The Human escaped!";

        scoreText.text = $"Final Score: {MatchData.Instance.endScore}";

        endScreen.SetActive(true);
    }

    private void ShowTutorial()
    {
        Debug.Log("Showing tutorial.");
        tutorialTextPosition = 0;
        tutorialTextElements[tutorialTextPosition].gameObject.SetActive(true);
        tutorialPanel.SetActive(true);
    }

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

    void OnEnable()
    {
        EventManager.hideLoadingScreen += HideLoadingScreen;
        EventManager.showEndScreen += ShowEndScreen;
        EventManager.showTutorial += ShowTutorial;
        EventManager.tutorialEnded += ShowIngameUI;
        EventManager.gameOver += HideIngameUI;
    }

    void OnDisable()
    {
        EventManager.hideLoadingScreen -= HideLoadingScreen;
        EventManager.showEndScreen -= ShowEndScreen;
        EventManager.showTutorial -= ShowTutorial;
        EventManager.tutorialEnded -= ShowIngameUI;
        EventManager.gameOver -= HideIngameUI;
    }

    private void HideIngameUI()
    {
        ingameUI.SetActive(false);
    }

    private void ShowIngameUI()
    {
        ingameUI.SetActive(true);
    }
}
