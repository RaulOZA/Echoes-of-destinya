using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    public string CurrentUser { get; private set; }
    public int CurrentUserId { get; private set; }  // A�adir un campo para el ID del usuario

    public int CurrentScore { get; set; } // Agregar esta l�nea para almacenar el puntaje actual

    private void Awake()
    {
        // Aseg�rate de que haya solo una instancia de SessionManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mant�n el objeto a trav�s de escenas
        }
        else
        {
            Destroy(gameObject); // Destruye duplicados
        }
    }

    public void StartSession(string username, int userId,int Score) // Modificar StartSession para incluir el ID
    {
        CurrentUser = username;
        CurrentUserId = userId; // Guardar el ID del usuario
        CurrentScore = Score;
    }

    public void EndSession()
    {
        CurrentUser = null;
        CurrentUserId = 0; // Restablecer el ID del usuario al finalizar la sesi�n
        CurrentScore = 0;
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(CurrentUser);
    }
}
