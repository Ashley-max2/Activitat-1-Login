using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private GameObject errorPopupObject;

    private DatabaseManager dbManager;
    private ErrorPopup errorPopup;

    private void Start()
    {
        dbManager = DatabaseManager.Instance;
        errorPopup = errorPopupObject.GetComponent<ErrorPopup>();
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorPopup.ShowError("Please fill in all fields");
            return;
        }

        DatabaseManager.LoginResult result = dbManager.LoginUser(username, password);

        switch (result)
        {
            case DatabaseManager.LoginResult.Success:
                PlayerPrefs.SetString("CurrentUser", username);
                SceneManager.LoadScene("DashboardScene");
                break;
            case DatabaseManager.LoginResult.UserNotFound:
                errorPopup.ShowError("User not found");
                break;
            case DatabaseManager.LoginResult.WrongPassword:
                errorPopup.ShowError("Incorrect password");
                passwordInput.text = "";
                break;
            case DatabaseManager.LoginResult.GenericError:
            default:
                errorPopup.ShowError("An error occurred during login");
                break;
        }
    }

    private void OnRegisterButtonClicked()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    private void ClearFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
    }
}
