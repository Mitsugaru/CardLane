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

    public GameObject pauseMenu;

    public Button newGameButton;

    public Button exitButton;

    public Button menuButton;

    public Button surrenderButton;

    private bool pauseShowing = false;

    // Use this for initialization
    void Start()
    {
        newGameButton.onClick.AddListener(handleNewGame);
        exitButton.onClick.AddListener(handleExit);
        menuButton.onClick.AddListener(handlePause);
        surrenderButton.onClick.AddListener(handleSurrender);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void handleNewGame()
    {
        controller.NewGame();

        menuButton.gameObject.SetActive(true);
        //TODO do a fade in / out
        mainMenu.SetActive(false);
        background.SetActive(false);
    }

    private void handleExit()
    {
        Application.Quit();
    }

    private void handlePause()
    {
        if (pauseShowing)
        {
            Time.timeScale = 1f;
            pauseShowing = false;
            pauseMenu.SetActive(false);
            gameResultLabel.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 0f;
            pauseShowing = true;
            pauseMenu.SetActive(true);
            gameResultLabel.gameObject.SetActive(false);
        }
    }

    private void handleSurrender()
    {
        handlePause();
        controller.EndGame();

        menuButton.gameObject.SetActive(false);
        //TODO do a fade in / out
        mainMenu.SetActive(true);
        background.SetActive(true);
    }
}
