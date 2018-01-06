using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameController controller;

    public Text gameResultLabel;

    public GameObject background;

    public GameObject mainMenu;

    public GameObject tutorialMenu;

    public GameObject pauseMenu;

    public GameObject tooltipHelp;

    public Text tooltipText;

    public Button newGameButton;

    public Button howToPlayButton;

    public Button exitButton;

    public Button menuButton;

    public Button surrenderButton;

    // Use this for initialization
    void Start()
    {
        newGameButton.onClick.AddListener(handleNewGame);
        howToPlayButton.onClick.AddListener(handleTutorial);
        exitButton.onClick.AddListener(handleExit);
        menuButton.onClick.AddListener(handleMenu);
        surrenderButton.onClick.AddListener(handleSurrender);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void hideHelp()
    {
        tooltipHelp.SetActive(false);
    }

    public void showHelp(HelpDisplay help)
    {
        switch(help)
        {
            case HelpDisplay.SETUP:
                tooltipText.text = "Fill each lane with a card from your hand";
                break;
            case HelpDisplay.SELECT_LANE:
                tooltipText.text = "Select a lane by click / tap and hold until the cards are revealed";
                break;
            case HelpDisplay.FILL_LANE:
                tooltipText.text = "Fill a lane with a card from your hand";
                break;
            default:
                break;
        }
        tooltipHelp.SetActive(true);
    }

    private void handleNewGame()
    {
        controller.NewGame();

        menuButton.gameObject.SetActive(true);
        tooltipHelp.SetActive(true);
        //TODO do a fade in / out
        mainMenu.SetActive(false);
        background.SetActive(false);
    }

    private void handleExit()
    {
        Application.Quit();
    }

    private void handleTutorial()
    {
        tutorialMenu.SetActive(true);
        mainMenu.SetActive(false);
        menuButton.gameObject.SetActive(true);
    }

    private void handleMenu()
    {
        if (tutorialMenu.activeInHierarchy)
        {
            tutorialMenu.SetActive(false);
            mainMenu.SetActive(true);
            menuButton.gameObject.SetActive(false);
        }
        else if (pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            tooltipHelp.SetActive(true);
            gameResultLabel.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            tooltipHelp.SetActive(false);
            gameResultLabel.gameObject.SetActive(false);
        }
    }

    private void handleSurrender()
    {
        handleMenu();
        controller.EndGame();

        menuButton.gameObject.SetActive(false);
        tooltipHelp.SetActive(false);
        //TODO do a fade in / out
        mainMenu.SetActive(true);
        background.SetActive(true);
    }
}
