using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject errorPopupObject;

    private DatabaseManager dbManager;
    private ErrorPopup errorPopup;

    private void Start()
    {
        dbManager = DatabaseManager.Instance;
        errorPopup = errorPopupObject.GetComponent<ErrorPopup>();
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Validaciones
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorPopup.ShowError("Please fill in all fields");
            return;
        }

        if (password != confirmPassword)
        {
            errorPopup.ShowError("Passwords do not match");
            return;
        }

        // Password length check is also done in DB manager, but good to have here for UI feedback before calling DB
        if (password.Length < 8)
        {
            errorPopup.ShowError("Password must be at least 8 characters long");
            return;
        }

        DatabaseManager.RegisterResult result = dbManager.RegisterUser(username, password);

        switch (result)
        {
            case DatabaseManager.RegisterResult.Success:
                errorPopup.ShowError("Registration successful. Returning to login...");
                Invoke("BackToLogin", 2f);
                break;
            case DatabaseManager.RegisterResult.UserAlreadyExists:
                errorPopup.ShowError("User already exists");
                break;
            case DatabaseManager.RegisterResult.PasswordTooShort:
                errorPopup.ShowError("Password must be at least 8 characters long");
                break;
            case DatabaseManager.RegisterResult.GenericError:
            default:
                errorPopup.ShowError("Error during registration");
                break;
        }
    }

    private void OnBackButtonClicked()
    {
        BackToLogin();
    }

    private void BackToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }

    private void ClearFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
    }
}
