using UnityEngine;
using MySql.Data.MySqlClient; // Aseg�rate de que esta l�nea est� incluida
using System;
using System.Collections.Generic;

public class AdminMySQL : MonoBehaviour
{
    private string DataCone;

    private MySqlConnection connection;

    // Start is called before the first frame update
    void Start()
    {
        DataCone = "Server=autorack.proxy.rlwy.net;Port=37390;Database=railway;Uid=root;Pwd=MXyNzHLqHbsQbnsQTHOwIhnIYpMqHHvT;";

        ConectarserviDB();

    }

    private void ConectarserviDB()
    {
        connection = new MySqlConnection(DataCone);
        try
        {
            connection.Open();
            Debug.Log("Conexion Exitosa");
        }
        catch (MySqlException e)
        {
            Debug.LogError("No se pudo conectar: " + e.Message);
        }
    }

    public MySqlDataReader Select(string query, Dictionary<string, object> parameters = null)
    {
        MySqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = query;

        // Agregar par�metros si se proporcionan
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        MySqlDataReader reader = cmd.ExecuteReader();
        return reader;
    }

    public bool Insert(string query, Dictionary<string, object> parameters)
    {
        using (MySqlCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = query;

            // Agregar los par�metros a la consulta
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            // Ejecutar la consulta y devolver true si se insert� al menos una fila
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }


    public bool UpdateRecord(string query, Dictionary<string, object> parameters)
    {
        using (MySqlCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = query;

            // Agregar los par�metros a la consulta
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            // Ejecutar la consulta y devolver true si se actualiz� al menos una fila
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }


    public bool UpdateScore(int userId, int scoreToAdd)
    {
        string query = "UPDATE users SET Score = Score + @score WHERE id = @userId";

        using (MySqlCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@score", scoreToAdd);
            cmd.Parameters.AddWithValue("@userId", userId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0; // Devuelve true si se actualiz� al menos una fila
        }
    }







    public bool Delete(string query, Dictionary<string, object> parameters)
    {
        using (MySqlCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = query;

            // Agregar los par�metros a la consulta
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            // Ejecutar la consulta y devolver true si se elimin� al menos una fila
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }





}
