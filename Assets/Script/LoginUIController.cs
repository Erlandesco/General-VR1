// LoginUIController.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;
    public Button logoutButton; // Akan dipindahkan, tapi tetap di sini untuk referensi kode
    public Button cancelButton;
    public TextMeshProUGUI statusText;

    [Header("References")]
    public HMITabController hmiTabController; // Pastikan ini terhubung di Inspector!

    private void OnEnable() // Dipanggil setiap kali GameObject aktif
    {
        // Kosongkan input field setiap kali panel login diaktifkan
        if (usernameInputField != null) usernameInputField.text = "";
        if (passwordInputField != null) passwordInputField.text = "";
        if (statusText != null) statusText.text = "Please login to proceed."; // Pesan awal yang lebih relevan
    }

    private void Start()
    {
        if (AuthorizationManager.Instance != null)
        {
            AuthorizationManager.Instance.OnAuthorizationStatusChanged += UpdateLoginUI;
        }

        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        else
        {
            Debug.LogWarning("Cancel Button is not assigned in LoginUIController.");
        }

        UpdateLoginUI(); // Panggil sekali di awal untuk mengatur UI
    }

    private void OnDestroy()
    {
        if (AuthorizationManager.Instance != null)
        {
            AuthorizationManager.Instance.OnAuthorizationStatusChanged -= UpdateLoginUI;
        }
        if (loginButton != null)
        {
            loginButton.onClick.RemoveListener(OnLoginButtonClicked);
        }
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }
    }

    public void OnLoginButtonClicked()
    {
        if (usernameInputField == null || passwordInputField == null)
        {
            Debug.LogError("Username or Password Input Field is not assigned in LoginUIController.");
            if (statusText != null) statusText.text = "Error: Input fields not set.";
            return;
        }

        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (AuthorizationManager.Instance.TryLogin(username, password))
        {
            // PENTING: JANGAN tampilkan "Login Successful!" di sini.
            // Biarkan HMITabController.OnLoginSuccess() yang menangani transisi UI.
            // Debug.Log($"Login successful! Welcome, {username}. Role: {AuthorizationManager.Instance.currentProfile.roleName}"); // Debug log tetap OK

            if (hmiTabController != null)
            {
                hmiTabController.OnLoginSuccess();
            }
            else
            {
                Debug.LogError("HMITabController reference is missing in LoginUIController.");
            }
        }
        else
        {
            // Tetap tampilkan pesan gagal login di statusText
            if (statusText != null) statusText.text = "Invalid Username or Password";
            passwordInputField.text = ""; // Kosongkan password setelah gagal login
        }
    }

    private void UpdateLoginUI()
    {
        bool isLoggedIn = AuthorizationManager.Instance.currentProfile != null;

        // Kontrol visibilitas elemen login
        if (usernameInputField != null) usernameInputField.gameObject.SetActive(true); // Selalu aktif di panel login
        if (passwordInputField != null) passwordInputField.gameObject.SetActive(true); // Selalu aktif di panel login
        if (loginButton != null) loginButton.gameObject.SetActive(true); // Selalu aktif di panel login
        if (cancelButton != null) cancelButton.gameObject.SetActive(true); // Selalu aktif di panel login

        // Pesan status (bisa disesuaikan agar tidak terlalu mengganggu jika panel login selalu diisi)
        // Saat ini, OnEnable() sudah mengatur pesan awal.
        // If (isLoggedIn) { statusText.text = "Already logged in as " + AuthorizationManager.Instance.currentProfile.roleName; }
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Login cancelled. Returning to previous user authority.");
        if (hmiTabController != null)
        {
            hmiTabController.ReturnFromLoginScreen();
        }
        else
        {
            Debug.LogError("HMITabController reference is missing in LoginUIController.");
        }
    }

    public void OnLogoutButtonClicked()
    {
        AuthorizationManager.Instance.Logout();
        // Setelah logout, kita selalu ingin menampilkan layar login bersih (dengan opsi login/cancel)
        if (hmiTabController != null)
        {
            hmiTabController.ShowLoginScreen();
        }
    }
}