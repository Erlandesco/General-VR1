// UserAdminScreenUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UserAdminScreenUIController : MonoBehaviour
{
    [Header("User Admin Panel")]
    public GameObject userAdminPanel;

    [Header("UI Elements")]
    public Button changePasswordButton;
    public TMP_InputField targetUsernameInputField;
    public TMP_InputField newPasswordInputField;
    public TextMeshProUGUI adminStatusText;

    void Start()
    {
        if (AuthorizationManager.Instance == null)
        {
            Debug.LogError("AuthorizationManager not found!");
            return;
        }

        // Daftarkan metode ke event AuthorizationManager
        AuthorizationManager.Instance.OnAuthorizationStatusChanged += OnAuthorizationChanged;
        OnAuthorizationChanged(); // Panggil sekali di awal

        if (changePasswordButton != null)
        {
            changePasswordButton.onClick.AddListener(OnChangePasswordButtonClicked);
        }
    }

    void OnDestroy() // Pastikan untuk menghapus listener saat objek dihancurkan
    {
        if (AuthorizationManager.Instance != null)
        {
            AuthorizationManager.Instance.OnAuthorizationStatusChanged -= OnAuthorizationChanged;
        }
    }

    // Metode ini akan dipanggil oleh event
    private void OnAuthorizationChanged()
    {
        CheckAndSetAdminPanelVisibility();
        UpdatePermissionDependentUI();
    }

    private void CheckAndSetAdminPanelVisibility()
    {
        if (userAdminPanel != null)
        {
            bool canAccess = AuthorizationManager.Instance.CanAccessNavigateUserAdminScreen();
            userAdminPanel.SetActive(canAccess);

            if (!canAccess && AuthorizationManager.Instance.currentProfile != null)
            {
                Debug.LogWarning($"Current user ({AuthorizationManager.Instance.currentProfile.roleName}) does not have permission to access User Admin Screen.");
            }
        }
    }

    private void UpdatePermissionDependentUI()
    {
        if (changePasswordButton != null)
        {
            bool canChangePass = AuthorizationManager.Instance.CanUserPasswordChange();
            changePasswordButton.interactable = canChangePass;
            if (!canChangePass && adminStatusText != null && AuthorizationManager.Instance.currentProfile != null)
            {
                adminStatusText.text = "You don't have permission to change passwords.";
                adminStatusText.color = Color.yellow;
            }
            else if (adminStatusText != null)
            {
                adminStatusText.text = ""; // Bersihkan pesan jika ada izin
                adminStatusText.color = Color.white;
            }
        }
        // Tambahkan logika serupa untuk tombol/element lain seperti canCreationOrDeletionAccounts, canChangeAuthorizationLevel
        // Contoh:
        // if (createDeleteAccountButton != null)
        // {
        //     createDeleteAccountButton.interactable = AuthorizationManager.Instance.CanCreationOrDeletionAccounts();
        // }
    }

    public void OnChangePasswordButtonClicked()
    {
        if (!AuthorizationManager.Instance.CanUserPasswordChange())
        {
            if (adminStatusText != null)
            {
                adminStatusText.color = Color.red;
                adminStatusText.text = "Permission denied: You cannot change user passwords.";
            }
            Debug.LogWarning("Permission denied: Cannot change user password.");
            return;
        }

        string targetUsername = targetUsernameInputField.text;
        string newPassword = newPasswordInputField.text;

        if (string.IsNullOrEmpty(targetUsername) || string.IsNullOrEmpty(newPassword))
        {
            if (adminStatusText != null)
            {
                adminStatusText.color = Color.red;
                adminStatusText.text = "Username and New Password cannot be empty.";
            }
            Debug.LogWarning("Username or New Password is empty.");
            return;
        }

        if (AuthorizationManager.Instance.ChangeUserPassword(targetUsername, newPassword))
        {
            if (adminStatusText != null)
            {
                adminStatusText.color = Color.green;
                adminStatusText.text = $"Password for '{targetUsername}' changed successfully.";
            }
        }
        else
        {
            if (adminStatusText != null)
            {
                adminStatusText.color = Color.red;
                adminStatusText.text = $"Failed to change password for '{targetUsername}'. Check console for details.";
            }
        }
    }
}