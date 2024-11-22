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
        AudioManager.instance.mainMenu.Stop();
        SceneManager.LoadScene(1);
        AudioManager.instance.lvl1bgmsc.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Game close");
    }

    public void GoToMainMenu()
    {
        AudioManager.instance.lvl1bgmsc.Stop();
        SceneManager.LoadScene(0);
    }
}
