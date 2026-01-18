using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ErrorPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideError);
        }
        popupPanel.SetActive(false);
    }

    public void ShowError(string message)
    {
        errorText.text = message;
        popupPanel.SetActive(true);
    }

    public void HideError()
    {
        popupPanel.SetActive(false);
    }
}
