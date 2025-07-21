// HMIButtonController.cs
using UnityEngine;
using UnityEngine.UI;

public class HMIButtonController : MonoBehaviour
{
    public Button activateIsolatorButton;
    // Tambahkan string untuk mengidentifikasi izin yang dibutuhkan
    //public string requiredPermissionName; // Contoh: "canActivateIsolator"
    public HMITabController hMITabController; // Referensi ke HMITabController jika diperlukan
    void Start()
    {
        // Tidak perlu pengecekan AuthorizationManager di sini jika tombol selalu aktif
        // if (AuthorizationManager.Instance == null)
        // {
        //     Debug.LogError("AuthorizationManager not found in the scene!");
        //     return;
        // }

        // Tidak perlu berlangganan event jika tombol selalu aktif
        // AuthorizationManager.Instance.OnAuthorizationStatusChanged += CheckAndSetButtonPermission;
        // CheckAndSetButtonPermission(); // Tidak perlu panggil ini di awal

        if (activateIsolatorButton != null)
        {
            activateIsolatorButton.onClick.AddListener(OnActivateIsolatorButtonClicked);
            activateIsolatorButton.interactable = true; // Set eksplisit aktif
        }
        else
        {
            Debug.LogError("Activate Isolator Button is not assigned in HMIButtonController!");
        }
    }

    void OnDestroy() // Pastikan untuk menghapus listener saat objek dihancurkan
    {
        // Tidak perlu unsubscribe jika tidak ada subscription di Start()
        // if (AuthorizationManager.Instance != null)
        // {
        //     AuthorizationManager.Instance.OnAuthorizationStatusChanged -= CheckAndSetButtonPermission;
        // }
        if (activateIsolatorButton != null)
        {
            activateIsolatorButton.onClick.RemoveListener(OnActivateIsolatorButtonClicked);
        }
    }


    // Jadikan metode ini publik atau internal agar bisa dipanggil oleh event
    //public void CheckAndSetButtonPermission()
    //{
    //    if (activateIsolatorButton != null)
    //    {
    //        bool hasPermission = false;
    //        // Gunakan HasPermission generik jika requiredPermissionName diset
    //        if (!string.IsNullOrEmpty(requiredPermissionName))
    //        {
    //            hasPermission = AuthorizationManager.Instance.HasPermission(requiredPermissionName);
    //        }
    //        else // Jika tidak diset, fallback ke canActivateIsolator (sesuai tujuan awal script ini)
    //        {
    //            hasPermission = AuthorizationManager.Instance.CanActivateIsolator();
    //        }

    //        activateIsolatorButton.interactable = hasPermission;
    //        Debug.Log($"Button ({gameObject.name}) - Permission '{requiredPermissionName}': {(hasPermission ? "Enabled" : "Disabled")} for current role.");
    //    }
    //}

    public void OnActivateIsolatorButtonClicked()
    {
        Debug.Log("Activate Isolator Button Clicked. Showing Login Screen.");
        if (hMITabController != null)
        {
            hMITabController.ShowLoginScreen();
        }
        else
        {
            Debug.LogError("HMITabController reference is missing in HMIButtonController!");
        }
    }
}