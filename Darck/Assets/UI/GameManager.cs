using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Importa esto si usas TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton

    // Salud del jugador
    public float playerHealth = 100f;
    public float maxPlayerHealth = 100f;

    // Imagen de Game Over
    public GameObject gameOverImg;

    // Puntaje
    public int score = 0; // Puntaje inicial
    public TextMeshProUGUI scoreText; // Referencia al texto del puntaje en el Canvas

    private void Awake()
    {
        // Configuramos el Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el GameManager al cambiar escenas
        }
        else
        {
            Destroy(gameObject); // Evitar duplicados
        }
    }

    private void Start()
    {
        // Mostrar el puntaje inicial al comenzar
        UpdateScoreText();
    }

    // Método para agregar puntaje
    public void AddScore(int points)
    {
        score += points; // Aumenta el puntaje
        UpdateScoreText(); // Actualiza el texto en pantalla
    }

    // Método para actualizar el texto del puntaje
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntaje: " + score; // Muestra el puntaje actual en pantalla
        }
    }

    // Método para reiniciar el estado del jugador
    public void ResetState()
    {
        // Reiniciar la salud del jugador
        playerHealth = maxPlayerHealth;

        // Ocultar la imagen de Game Over si existe
        if (gameOverImg != null)
        {
            gameOverImg.SetActive(false);
        }

        // Reiniciar el puntaje
        score = 0;
        UpdateScoreText();
    }

    // Puedes agregar aquí lógica adicional como gestionar la música según la escena
    // Descomenta y ajusta si es necesario
    //private void Start()
    //{
    //    Scene scene = SceneManager.GetActiveScene();
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
}
