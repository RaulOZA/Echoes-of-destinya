using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Mainmenu; // Botón de login BOTON Logout 
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
        AudioManager.instance.lvlbgmsc.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Game close");
    }

    public void GoToMainMenu()
    {
        AudioManager.instance.lvlbgmsc.Stop();
        SceneManager.LoadScene(0);
    }
}