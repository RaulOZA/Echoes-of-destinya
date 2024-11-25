using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float playerHealth = 100f;  // Player's health
    public float maxPlayerHealth = 100f;

    public GameObject gameOverImg;    // Reference to the Game Over image

    private void Awake()
    {
        if (instance == null)
        {
            //instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    //private void Start()
    //{
    //    Scene scene = SceneManager.GetActiveScene();
    //    //if (scene.name == "Level 1")
    //    //{
    //    //    level1Music.Play();
    //    //}
    //    if (scene.name == "Level2")
    //    {
    //       AudioManager.instance.level2Music.Play();
    //    }
    //    if (scene.name == "Level3")
    //    {
    //        AudioManager.instance.level3Music.Play();
    //    }
    //    if (scene.name == "FinalBoss")
    //    {
    //        AudioManager.instance.level4Music.Play();
    //    }
    //}

    public void ResetState()
    {
        // Reset health
        playerHealth = maxPlayerHealth;

        // Hide the Game Over image if it exists
        if (gameOverImg != null)
        {
            gameOverImg.SetActive(false);
        }
    }
}
