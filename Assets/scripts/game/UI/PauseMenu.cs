using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;

    [SerializeField]
    private GameObject pauseMenuUI;


    public void HandlePauseMenu()
    {
        if (gamePaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void Exit()
    {
        AsyncLoader asyncLoader = FindObjectOfType<AsyncLoader>();

        if (asyncLoader != null)
        {
            asyncLoader.LoadLevelButton("Menu");
        }
        else
        {
            Debug.LogError("AsyncLoader script not found in the scene!");
        }
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
        print(gamePaused);
    }
}
