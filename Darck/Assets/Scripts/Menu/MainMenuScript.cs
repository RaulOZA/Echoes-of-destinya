using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public GameObject Mainmenu; // Botón de login BOTON Logout 
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
        Mainmenu.SetActive(false); // Muestra el botón Loguotboton
        AudioManager.instance.mainMenu.Stop();
        SceneManager.LoadScene(1);
        AudioManager.instance.level1Music.Play();
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