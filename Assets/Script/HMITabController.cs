// HMITabController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HMITabController : MonoBehaviour
{
    [System.Serializable]
    public class TabPanel
    {
        public Button tabButton;
        public GameObject panelContent;
        public string requiredAccessPermission; // Nama permission dari AuthorizationProfile
    }

    [Header("Panels")]
    public GameObject initialScreenPanel;
    public GameObject loginPanel;
    public GameObject mainHmiPanel;

    [Header("Tabs")]
    public List<TabPanel> tabPanels;

    [Header("Other UI")]
    public Button shutdownFunctionKeyButton;
    public Button logoutButtonHeader;

    private GameObject currentActivePanel;
    private bool wasLoggedInBeforeLoginScreen = false;
    private AuthorizationProfile previousProfileOnLoginAttempt = null; // BARU: Simpan profil sebelumnya

    private void Start()
    {
        if (AuthorizationManager.Instance == null)
        {
            Debug.LogError("AuthorizationManager instance not found. Please ensure it is in the scene.");
            return;
        }

        AuthorizationManager.Instance.OnAuthorizationStatusChanged += UpdateTabPermissions;
        AuthorizationManager.Instance.OnAuthorizationStatusChanged += UpdateLogoutButtonVisibility;

        initialScreenPanel?.SetActive(true);
        loginPanel?.SetActive(false);
        mainHmiPanel?.SetActive(false);

        foreach (var tab in tabPanels)
        {
            tab.panelContent?.SetActive(false);
            if (tab.tabButton != null)
            {
                tab.tabButton.onClick.AddListener(() => OnTabButtonClicked(tab));
            }
        }

        Button activateButton = initialScreenPanel?.GetComponentInChildren<Button>();
        if (activateButton != null && activateButton.name == "Button_PressToActivate")
        {
            activateButton.onClick.AddListener(ShowLoginScreen);
        }
        else
        {
            Debug.LogWarning("Button_PressToActivate not found in InitialScreenPanel. Please ensure it exists and is named correctly.");
        }

        if (logoutButtonHeader != null)
        {
            logoutButtonHeader.onClick.AddListener(OnLogoutButtonClicked);
        }

        UpdateTabPermissions();
        UpdateLogoutButtonVisibility();
    }

    private void OnDestroy()
    {
        if (AuthorizationManager.Instance != null)
        {
            AuthorizationManager.Instance.OnAuthorizationStatusChanged -= UpdateTabPermissions;
            AuthorizationManager.Instance.OnAuthorizationStatusChanged -= UpdateLogoutButtonVisibility;
        }
        foreach (var tab in tabPanels)
        {
            if (tab.tabButton != null)
            {
                tab.tabButton.onClick.RemoveAllListeners();
            }
        }
        Button activateButton = initialScreenPanel?.GetComponentInChildren<Button>();
        if (activateButton != null && activateButton.name == "Button_PressToActivate")
        {
            activateButton.onClick.RemoveListener(ShowLoginScreen);
        }
        if (logoutButtonHeader != null)
        {
            logoutButtonHeader.onClick.RemoveListener(OnLogoutButtonClicked);
        }
    }

    public void ShowLoginScreen()
    {
        // Simpan profil yang sedang aktif SAAT INI (sebelum login attempt)
        // Ini akan digunakan jika user menekan Cancel
        previousProfileOnLoginAttempt = AuthorizationManager.Instance.currentProfile;

        initialScreenPanel?.SetActive(false);
        mainHmiPanel?.SetActive(false);
        loginPanel?.SetActive(true);

        // Pastikan tidak ada panel konten tab yang aktif saat login popup muncul
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
        }
    }

    public void OnLoginSuccess()
    {
        loginPanel?.SetActive(false);
        initialScreenPanel?.SetActive(false); // Pastikan ini juga disembunyikan
        mainHmiPanel?.SetActive(true); // Tampilkan Main HMI Panel

        // Setelah login sukses, atur previousProfileOnLoginAttempt ke null
        // karena statusnya sudah berubah secara sengaja
        previousProfileOnLoginAttempt = null;

        // Tampilkan tab pertama yang dapat diakses oleh user baru
        ShowFirstAccessibleTabContent();
    }

    public void ReturnFromLoginScreen()
    {
        loginPanel?.SetActive(false);

        // Jika ada previousProfileOnLoginAttempt, berarti user sebelumnya sudah login
        // dan kita harus kembali ke Main HMI Panel dengan profil tersebut
        if (previousProfileOnLoginAttempt != null)
        {
            // Penting: Jangan lakukan AuthorizationManager.Instance.currentProfile = previousProfileOnLoginAttempt; di sini.
            // AuthorizationManager.Instance.currentProfile sudah seharusnya tidak berubah
            // jika login gagal atau cancel. Kita hanya perlu mengembalikan UI ke state sebelumnya.

            mainHmiPanel?.SetActive(true);
            ShowFirstAccessibleTabContent(); // Tampilkan konten tab yang relevan untuk user sebelumnya
            Debug.Log($"Returned to previous authority: {previousProfileOnLoginAttempt.roleName}");
        }
        else
        {
            // Jika previousProfileOnLoginAttempt null, berarti sebelumnya tidak ada yang login
            // atau sudah logout, jadi kembali ke Initial Screen
            initialScreenPanel?.SetActive(true);
            Debug.Log("Returned to initial screen (no previous login).");
        }
        // Panggil event untuk update header dan tombol logout
        AuthorizationManager.Instance.TriggerAuthorizationStatusChanged();
    }

    private void OnTabButtonClicked(TabPanel clickedTab)
    {
        if (AuthorizationManager.Instance.HasPermission(clickedTab.requiredAccessPermission))
        {
            ActivateTabContent(clickedTab.panelContent);
            Debug.Log($"Accessed tab: {clickedTab.requiredAccessPermission} with role: {AuthorizationManager.Instance.currentProfile?.roleName}");
        }
        else
        {
            Debug.LogWarning($"Access denied for {clickedTab.requiredAccessPermission} for user: {AuthorizationManager.Instance.currentProfile?.roleName}. Showing login screen.");
            ShowLoginScreen();
        }
    }

    private void ActivateTabContent(GameObject panelContentToActivate)
    {
        if (currentActivePanel != panelContentToActivate)
        {
            if (currentActivePanel != null)
            {
                currentActivePanel.SetActive(false);
            }
            panelContentToActivate?.SetActive(true);
            currentActivePanel = panelContentToActivate;
        }
    }

    private void ShowFirstAccessibleTabContent()
    {
        if (AuthorizationManager.Instance.currentProfile == null)
        {
            if (currentActivePanel != null)
            {
                currentActivePanel.SetActive(false);
                currentActivePanel = null;
            }
            return;
        }

        foreach (var tab in tabPanels)
        {
            if (AuthorizationManager.Instance.HasPermission(tab.requiredAccessPermission))
            {
                ActivateTabContent(tab.panelContent);
                return;
            }
        }
        Debug.LogWarning("No accessible tabs found for the current user after login.");
    }

    private void UpdateTabPermissions()
    {
        foreach (var tab in tabPanels)
        {
            if (tab.tabButton != null)
            {
                tab.tabButton.gameObject.SetActive(true);
                tab.tabButton.interactable = true;
            }
        }

        if (shutdownFunctionKeyButton != null)
        {
            bool canShutdown = AuthorizationManager.Instance.CanShutdownIsolatorSystem();
            shutdownFunctionKeyButton.interactable = canShutdown;
            shutdownFunctionKeyButton.gameObject.SetActive(canShutdown);
        }

        if (AuthorizationManager.Instance.currentProfile == null)
        {
            if (currentActivePanel != null)
            {
                currentActivePanel.SetActive(false);
                currentActivePanel = null;
            }
            mainHmiPanel?.SetActive(false);
        }
    }

    public void OnLogoutButtonClicked()
    {
        AuthorizationManager.Instance.Logout();
        // Setelah logout, kita ingin kembali ke login screen yang bersih, bukan initial screen
        ShowLoginScreen();
    }

    private void UpdateLogoutButtonVisibility()
    {
        if (logoutButtonHeader != null)
        {
            logoutButtonHeader.gameObject.SetActive(AuthorizationManager.Instance.currentProfile != null);
        }
    }
}