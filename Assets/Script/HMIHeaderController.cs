// HMIHeaderController.cs
using UnityEngine;
using TMPro; // Jika menggunakan TextMeshPro
using System;
using UnityEngine.UI; // Jika menggunakan UI Image untuk status icon atau logo

public class HMIHeaderController : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI dateTimeText;
    public Image statusIcon; // Jika ada icon status
    public Image logoImage; // Jika ada logo

    void Start()
    {
        if (AuthorizationManager.Instance == null)
        {
            Debug.LogError("AuthorizationManager not found in the scene!");
            return;
        }

        // Daftarkan UpdateHeader ke event AuthorizationManager
        AuthorizationManager.Instance.OnAuthorizationStatusChanged += UpdateHeader;
        UpdateHeader(); // Panggil sekali di awal

        // Jika tanggal/waktu perlu real-time, update setiap frame atau interval tertentu
        InvokeRepeating("UpdateDateTime", 0f, 1f); // Update setiap 1 detik
    }

    void OnDestroy()
    {
        if (AuthorizationManager.Instance != null)
        {
            AuthorizationManager.Instance.OnAuthorizationStatusChanged -= UpdateHeader;
        }
        CancelInvoke("UpdateDateTime");
    }

    public void UpdateHeader()
    {
        if (AuthorizationManager.Instance.currentProfile != null)
        {
            if (usernameText != null) usernameText.text = AuthorizationManager.Instance.currentProfile.username;
            if (roleText != null) roleText.text = AuthorizationManager.Instance.currentProfile.roleName;
            // Perbarui status icon jika ada
            // if (statusIcon != null) statusIcon.sprite = GetStatusIcon(AuthorizationManager.Instance.currentProfile.status);
        }
        else
        {
            if (usernameText != null) usernameText.text = "Guest";
            if (roleText != null) roleText.text = "Guest";
            // Set status icon ke default tidak login
        }
        UpdateDateTime(); // Pastikan tanggal/waktu juga terupdate
    }

    private void UpdateDateTime()
    {
        if (dateTimeText != null)
        {
            dateTimeText.text = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
        }
    }

    // Helper untuk mendapatkan sprite status icon (jika ada)
    // private Sprite GetStatusIcon(UserStatus status) { ... }
}