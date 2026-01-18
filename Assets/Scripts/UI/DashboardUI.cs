using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DashboardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text welcomeText;
    [SerializeField] private Button logoutButton;

    private void Start()
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Usuario");
        welcomeText.text = "Welcome, " + currentUser;
        logoutButton.onClick.AddListener(OnLogoutButtonClicked);
    }

    private void OnLogoutButtonClicked()
    {
        PlayerPrefs.DeleteKey("CurrentUser");
        SceneManager.LoadScene("LoginScene");
    }
}
