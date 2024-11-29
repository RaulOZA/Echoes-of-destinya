using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "MainMenu")
        {
            AudioManager.instance.gameOver.Stop();
            AudioManager.instance.PlayAudio(AudioManager.instance.mainMenu);
            //AudioManager.instance.PlayAudio(AudioManager.instance.thunder);
        }
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        Time.timeScale = 1; // Ensure time scale is reset
        if (AudioManager.instance != null)
        {
            AudioManager.instance.mainMenu.Stop();
            //AudioManager.instance.level1Music.Play();
        }

        // Reset GameManager state
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetState();
        }
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Game close");
    }

    public void GoToMainMenu()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.level1Music.Stop();
            AudioManager.instance.level2Music.Stop();
            AudioManager.instance.level3Music.Stop();
            AudioManager.instance.level4Music.Stop();

        }

        // Reset GameManager state
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetState();
        }

        // Reset the GameOver UI
        GameObject gameOverUI = GameObject.Find("GameOver");
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        SceneManager.LoadScene(0);
    }
}