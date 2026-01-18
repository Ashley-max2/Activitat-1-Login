using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager instance;
    private string connectionString;
    private const string DATABASE_NAME = "users.db";

    public enum LoginResult
    {
        Success,
        UserNotFound,
        WrongPassword,
        GenericError
    }

    public enum RegisterResult
    {
        Success,
        UserAlreadyExists,
        PasswordTooShort,
        GenericError
    }

    public static DatabaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject dbObject = new GameObject("DatabaseManager");
                instance = dbObject.AddComponent<DatabaseManager>();
                DontDestroyOnLoad(dbObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    public void InitializeDatabase()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, DATABASE_NAME);
        connectionString = "URI=file:" + dbPath;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL
                    )";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        Debug.Log("Database initialized at: " + dbPath);
    }

    public RegisterResult RegisterUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username and password cannot be empty");
            return RegisterResult.GenericError;
        }

        if (password.Length < 8)
        {
            Debug.LogError("Password must be at least 8 characters long");
            return RegisterResult.PasswordTooShort;
        }

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            Debug.Log("User registered successfully: " + username);
            return RegisterResult.Success;
        }
        catch (SqliteException ex)
        {
            if (ex.Message.Contains("UNIQUE constraint failed"))
            {
                Debug.LogError("User already exists");
                return RegisterResult.UserAlreadyExists;
            }
            Debug.LogError("Error registering user: " + ex.Message);
            return RegisterResult.GenericError;
        }
    }

    public LoginResult LoginUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return LoginResult.GenericError;
        }

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Password FROM Users WHERE Username = @username";
                    command.Parameters.AddWithValue("@username", username);

                    object result = command.ExecuteScalar();

                    if (result == null)
                    {
                        return LoginResult.UserNotFound;
                    }

                    string storedPassword = result.ToString();

                    if (storedPassword == password)
                    {
                        return LoginResult.Success;
                    }
                    else
                    {
                        return LoginResult.WrongPassword;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during login: " + ex.Message);
            return LoginResult.GenericError;
        }
    }

    public bool UserExists(string username)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    command.Parameters.AddWithValue("@username", username);

                    int count = 0;
                    object res = command.ExecuteScalar();
                    if (res != null)
                    {
                        // SQLite count returns Int64 usually
                        count = System.Convert.ToInt32(res);
                    }
                    return count > 0;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error checking user: " + ex.Message);
            return false;
        }
    }
}
