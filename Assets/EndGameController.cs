using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    public void ReplayGame()
    {
        SceneManager.LoadSceneAsync("MainGameScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync("StartGameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
