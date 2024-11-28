using UnityEngine;
using TMPro; // Importa esto si usas TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton
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
        UpdateScoreText(); // Mostrar el puntaje inicial al comenzar
    }

    public void AddScore(int points)
    {
        score += points; // Aumenta el puntaje
        UpdateScoreText(); // Actualiza el texto en pantalla
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntaje: " + score; // Muestra el puntaje actual en pantalla
        }
    }
}
