using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    GameManager gameManager;

    private void Awake()
    {
       gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else 
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        AudioListener.pause = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        AudioListener.pause = true;
        GameIsPaused = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartGameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
