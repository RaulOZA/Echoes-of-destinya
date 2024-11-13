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
        }
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        AudioManager.instance.mainMenu.Stop();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Game close");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}