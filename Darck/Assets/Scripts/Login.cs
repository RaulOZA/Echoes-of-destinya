using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class Login : MonoBehaviour
{
    public TMP_InputField LoginField;  // Asegúrate de que esto esté asignado en el inspector CAMPO DEL USER
    public TMP_InputField PasswordField; // Asegúrate de que esto esté asignado en el inspector CAMPO DE CONTRA
    public TMP_InputField Condirmpassword; // CAMPO DE LA CONTRA ASERGAYSE
    public TMP_InputField Email; // CAMP[O DE GMAIL

    public GameObject StartBtn; // Botón que aparecerá tras el login BOTON START
    public GameObject LoginButton; // Botón de login BOTON LOGIN
    public GameObject RegisterButton; // 
    public GameObject Registeroption; //
    public GameObject Quitbutton; // 
    public GameObject Updatebutton; // 
    public GameObject Updateoption; // 
    public GameObject Deletebutton; //



    public GameObject Back; // Botón de login BOTON LOGIN
    public GameObject Back2; // Botón de login BOTON LOGIN


    public GameObject LogoutButton; // Botón de login BOTON Logout 



    // Nueva variable para el texto que mostrará el nombre de usuario
    public TextMeshProUGUI UserNameDisplay; // O si es un objeto UI estándar: public Text UserNameDisplay;

    public TextMeshProUGUI LevelDisplay; // Usa TextMeshProUGUI si usas TMP, o Text si usas el UI Text estándar.
    public TextMeshProUGUI ScoreDisplay; // Usa TextMeshProUGUI si usas TMP, o Text si usas el UI Text estándar.
    void Start()
    {
        if (SessionManager.Instance != null)
        {
            Debug.Log("SessionManager is available");
            if (SessionManager.Instance.IsLoggedIn())
            {
                if (UserNameDisplay != null)
                {
                    UserNameDisplay.text = "Welcome, " + SessionManager.Instance.CurrentUser;
                }
                else
                {
                    Debug.LogError("UserNameDisplay is not assigned in the Inspector!");
                }
            }
            else
            {
                if (UserNameDisplay != null)
                {
                    UserNameDisplay.text = ""; // Limpia el texto si no hay sesión
                    LevelDisplay.text = ""; // Limpia el texto si no hay sesión
                    ScoreDisplay.text = ""; // Limpia el texto si no hay sesión
                }
                else
                {
                    Debug.LogError("UserNameDisplay is not assigned in the Inspector!");
                }
            }
        }
        else
        {
            Debug.LogError("SessionManager instance is null!");
        }
    }



    public void Logear()
    {
        string _log = "SELECT * FROM `users` WHERE `User` = @username AND `Pass` = @password";

        AdminMySQL _adminMYSQL = GameObject.Find("Admin_BD").GetComponent<AdminMySQL>();
        MySqlDataReader reader = _adminMYSQL.Select(_log, new Dictionary<string, object>
    {
        { "@username", LoginField.text },
        { "@password", PasswordField.text }
    });

        if (reader.HasRows)
        {
            reader.Read(); // Leer la primera fila para obtener el ID

            // Asegúrate de que los nombres de columnas son correctos
            int userId = reader.GetInt32(reader.GetOrdinal("id")); // Obtener el ID del usuario
            int level = reader.GetInt32(reader.GetOrdinal("Level")); // Obtener el nivel del usuario
            int score = reader.GetInt32(reader.GetOrdinal("Score")); // Obtener el puntaje del usuario

            Debug.Log("Login Correcto");
            reader.Close();

            // Inicia la sesión y pasa el ID del usuario
            SessionManager.Instance.StartSession(LoginField.text, userId,score);

            // Oculta los campos de login y muestra el botón de inicio
            LoginField.gameObject.SetActive(false);
            PasswordField.gameObject.SetActive(false);
            LoginButton.SetActive(false); // Oculta el botón de login
            StartBtn.SetActive(true); // Muestra el botón StartBtn
            LogoutButton.SetActive(true); // Muestra el botón Loguotboton
            Registeroption.gameObject.SetActive(false);
            Updateoption.gameObject.SetActive(true);   




            // Muestra los valores en el UI
            LevelDisplay.text = "Level completed: " + level.ToString();
            ScoreDisplay.text = "Score: " + score.ToString();

            // Actualiza el texto con el nombre del usuario después de iniciar sesión
            UserNameDisplay.text = "Welcome, " + LoginField.text;




        }
        else
        {
            Debug.Log("Login Incorrecto");
        }

        reader.Close(); // Cierra el lector de datos
    }




    public void registerUI()
    {



        // Oculta los campos de login y muestra el botón de inicio
        LoginField.gameObject.SetActive(true);
        PasswordField.gameObject.SetActive(true);
        Condirmpassword.gameObject.SetActive(true);
        Email.gameObject.SetActive(true);
        RegisterButton.gameObject.SetActive(true);
        Back.gameObject.SetActive(true);
        Quitbutton.gameObject.SetActive(false);
        Registeroption.gameObject.SetActive(false);

        LoginButton.SetActive(false); // Oculta el botón de login
        StartBtn.SetActive(false); // Muestra el botón StartBtn
        LogoutButton.SetActive(false); // Muestra el botón Loguotboton



    }




    public void UpdateUI()
    {



        // Oculta los campos de login y muestra el botón de inicio
        LoginField.gameObject.SetActive(true);
        PasswordField.gameObject.SetActive(true);
        Condirmpassword.gameObject.SetActive(true);
        Email.gameObject.SetActive(true);
        Updatebutton.gameObject.SetActive(true);
        Back2.gameObject.SetActive(true);
        Quitbutton.gameObject.SetActive(false);
        Registeroption.gameObject.SetActive(false);
        Deletebutton.gameObject.SetActive(true);
        Updateoption.gameObject.SetActive(false);

        LoginButton.SetActive(false); // Oculta el botón de login
        StartBtn.SetActive(false); // Muestra el botón StartBtn
        LogoutButton.SetActive(false); // Muestra el botón Loguotboton



    }


    public void back2()
    {



        // Oculta los campos de login y muestra el botón de inicio
        LoginField.gameObject.SetActive(false);
        PasswordField.gameObject.SetActive(false);
        Condirmpassword.gameObject.SetActive(false);
        Email.gameObject.SetActive(false);
        RegisterButton.gameObject.SetActive(false);
        Quitbutton.gameObject.SetActive(true);

        LoginButton.SetActive(false); // Oculta el botón de login
        StartBtn.SetActive(true); // Muestra el botón StartBtn
        LogoutButton.SetActive(true); // Muestra el botón Loguotboton
        Registeroption.SetActive(false);
        Updateoption.gameObject.SetActive(true);
        Back2.gameObject.SetActive(false);   
        Deletebutton.gameObject.SetActive(false);
        Updatebutton.gameObject.SetActive(false);




    }




    public void back()
    {



        // Oculta los campos de login y muestra el botón de inicio
        LoginField.gameObject.SetActive(true);
        PasswordField.gameObject.SetActive(true);
        Condirmpassword.gameObject.SetActive(false);
        Email.gameObject.SetActive(false);
        RegisterButton.gameObject.SetActive(true);
        Back.gameObject.SetActive(false);
        Quitbutton.gameObject.SetActive(true);

        LoginButton.SetActive(true); // Oculta el botón de login
        StartBtn.SetActive(false); // Muestra el botón StartBtn
        LogoutButton.SetActive(false); // Muestra el botón Loguotboton
        Registeroption.SetActive(false);



    }





    public void Logout()
    {
        SessionManager.Instance.EndSession(); // Cierra la sesión

        // Actualiza el texto del nombre de usuario en la interfaz
        UserNameDisplay.text = ""; // Limpia el texto de bienvenida
        LevelDisplay.text = ""; // Limpia el texto de bienvenida
        ScoreDisplay.text = ""; // Limpia el texto de bienvenida


        // Oculta los campos de login y muestra el botón de inicio
        LoginField.gameObject.SetActive(true);
        PasswordField.gameObject.SetActive(true);
        LoginButton.SetActive(true); // Oculta el botón de login
        StartBtn.SetActive(false); // Muestra el botón StartBtn
        LogoutButton.SetActive(false); // Muestra el botón Loguotboton
        Registeroption.SetActive(true); // Muestra el botón Loguotboton
        Updateoption.gameObject.SetActive(false);


        Debug.Log("Usuario ha cerrado sesión.");

    }



    public void RegisterUser()
    {
        if (LoginField.text.Length >= 3 && LoginField.text.Length <= 12)
        {
            if (PasswordField.text == Condirmpassword.text)
            {
                string queryCheck = "SELECT * FROM `users` WHERE `User` = @username";
                AdminMySQL _adminMYSQL = GameObject.Find("Admin_BD").GetComponent<AdminMySQL>();

                MySqlDataReader reader = _adminMYSQL.Select(queryCheck, new Dictionary<string, object>
                {
                    { "@username", LoginField.text }
                });

                if (reader.HasRows)
                {
                    Debug.Log("User already exists");
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    string queryInsert = "INSERT INTO `users` (`User`, `Pass`, `Email`) VALUES (@username, @password, @Email)";

                    bool success = _adminMYSQL.Insert(queryInsert, new Dictionary<string, object>
                    {
                        { "@username", LoginField.text },
                        { "@password", PasswordField.text },
                        { "@Email", Email.text }
                    });

                    if (success)
                    {

                        // Oculta los campos de login y muestra el botón de inicio
                        LoginField.gameObject.SetActive(true);
                        PasswordField.gameObject.SetActive(true);
                        Condirmpassword.gameObject.SetActive(false);
                        Email.gameObject.SetActive(false);
                        RegisterButton.gameObject.SetActive(true);
                        Back.gameObject.SetActive(false);
                        Quitbutton.gameObject.SetActive(true);
                        Registeroption.gameObject.SetActive(false);

                        LoginButton.SetActive(true); // Oculta el botón de login
                        StartBtn.SetActive(false); // Muestra el botón StartBtn
                        LogoutButton.SetActive(false); // Muestra el botón Loguotboton


                        Debug.Log("User created");
                    }
                    else
                    {
                        Debug.Log("Error creating user");
                    }
                }
            }
            else
            {
                Debug.Log("Password confirmation is incorrect");
            }
        }
        else
        {
            Debug.Log("Username must be between 3 and 12 characters");
        }
    }


    public void UpdateUser()
    {
        int loggedInUserId = SessionManager.Instance.CurrentUserId; // Obtener el ID del usuario actual

        // Asegúrate de que el nuevo nombre de usuario tenga una longitud válida
        if (LoginField.text.Length >= 3 && LoginField.text.Length <= 12)
        {
            // Verifica si el campo de confirmación de contraseña es correcto
            if (PasswordField.text == Condirmpassword.text)
            {
                // Verificar si el nuevo nombre de usuario ya existe
                string queryCheck = "SELECT * FROM `users` WHERE `User` = @username AND `id` != @id";
                AdminMySQL _adminMYSQL = GameObject.Find("Admin_BD").GetComponent<AdminMySQL>();

                MySqlDataReader reader = _adminMYSQL.Select(queryCheck, new Dictionary<string, object>
            {
                { "@username", LoginField.text },
                { "@id", loggedInUserId }
            });

                if (reader.HasRows)
                {
                    Debug.Log("User already exists");
                    reader.Close();
                }
                else
                {
                    reader.Close();

                    string queryUpdate = "UPDATE `users` SET `User` = @username, `Pass` = @password, `Email` = @Email WHERE `id` = @id";

                    bool success = _adminMYSQL.UpdateRecord(queryUpdate, new Dictionary<string, object>
                    {
                        { "@username", LoginField.text },
                        { "@password", PasswordField.text },
                        { "@Email", Email.text },
                        { "@id", loggedInUserId }  // ID del usuario que se va a actualizar
                    });

                    if (success)
                    {

                        UserNameDisplay.text = ""; // Limpia el texto de bienvenida
                        LevelDisplay.text = ""; // Limpia el texto de bienvenida
                        ScoreDisplay.text = ""; // Limpia el texto de bienvenida



                        // Oculta los campos de login y muestra el botón de inicio
                        LoginField.gameObject.SetActive(false);
                        PasswordField.gameObject.SetActive(false);
                        Condirmpassword.gameObject.SetActive(false);
                        Email.gameObject.SetActive(false);
                        RegisterButton.gameObject.SetActive(false);
                        Quitbutton.gameObject.SetActive(true);

                        LoginButton.SetActive(false); // Oculta el botón de login
                        StartBtn.SetActive(true); // Muestra el botón StartBtn
                        LogoutButton.SetActive(true); // Muestra el botón Loguotboton
                        Registeroption.SetActive(false);
                        Updateoption.gameObject.SetActive(true);
                        Back2.gameObject.SetActive(false);
                        Deletebutton.gameObject.SetActive(false);
                        Updatebutton.gameObject.SetActive(false);

                        Debug.Log("User updated successfully");
                    }
                    else
                    {
                        Debug.Log("Error updating user");
                    }
                }
            }
            else
            {
                Debug.Log("Password confirmation is incorrect");
            }
        }
        else
        {
            Debug.Log("Username must be between 3 and 12 characters");
        }
    }





    public void DeleteUser()
    {
        int loggedInUserId = SessionManager.Instance.CurrentUserId; // Obtener el ID del usuario actual

        // Verificar que el ID del usuario logeado no sea nulo o negativo
        if (loggedInUserId > 0)
        {
            string queryDelete = "DELETE FROM `users` WHERE `id` = @id";
            AdminMySQL _adminMYSQL = GameObject.Find("Admin_BD").GetComponent<AdminMySQL>();

            // Crear un diccionario para los parámetros
            Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "@id", loggedInUserId }  // ID del usuario que se va a eliminar
        };

            // Ejecutar la eliminación usando la nueva función
            bool success = _adminMYSQL.Delete(queryDelete, parameters);

            if (success)
            {
                SessionManager.Instance.EndSession(); // Cierra la sesión

                UserNameDisplay.text = ""; // Limpia el texto de bienvenida
                LevelDisplay.text = ""; // Limpia el texto de bienvenida
                ScoreDisplay.text = ""; // Limpia el texto de bienvenida

                // Oculta los campos de login y muestra el botón de inicio
                LoginField.gameObject.SetActive(true);
                PasswordField.gameObject.SetActive(true);
                Condirmpassword.gameObject.SetActive(false);
                Email.gameObject.SetActive(false);
                RegisterButton.gameObject.SetActive(false);
                Quitbutton.gameObject.SetActive(true);

                LoginButton.SetActive(false); // Oculta el botón de login
                StartBtn.SetActive(false); // Muestra el botón StartBtn
                LogoutButton.SetActive(false); // Muestra el botón Loguotboton
                Registeroption.SetActive(true);
                Updateoption.gameObject.SetActive(false);
                Back2.gameObject.SetActive(false);
                Deletebutton.gameObject.SetActive(false);
                Updatebutton.gameObject.SetActive(false);
                LoginButton.gameObject.SetActive(true);
                
                Debug.Log("User deleted successfully");
                SessionManager.Instance.EndSession(); // Termina la sesión del usuario
            }
            else
            {
                Debug.Log("Error deleting user");
            }
        }
        else
        {
            Debug.Log("No logged in user to delete");
        }
    }



    public void ChangeToSampleScene()
    {
        // Verifica si el usuario ha iniciado sesión
        if (SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn())
        {
            // Cambia a la escena llamada "SampleScene"
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.LogWarning("No puedes cambiar de escena sin haber iniciado sesión.");
        }
    }
    












    public void OnLogoutButtonPressed()
    {
        Logout(); // Llama al método de logout
    }










}
