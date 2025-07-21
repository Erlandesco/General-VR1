// AuthorizationManager.cs
using System;
using System.Collections.Generic;
using System.Security.Cryptography; // Untuk hashing sederhana (opsional, sangat disarankan untuk riil)
using System.Text; // Untuk mengubah string ke byte array
using UnityEngine; // Tambahkan ini
using System.Linq;

public class AuthorizationManager : MonoBehaviour
{
    public static AuthorizationManager Instance { get; private set; }


    // Dictionary untuk menyimpan semua profil yang tersedia berdasarkan username
    private Dictionary<string, AuthorizationProfile> userProfiles = new Dictionary<string, AuthorizationProfile>();
    // Event yang akan dipanggil ketika status otorisasi berubah
  
    public event Action OnAuthorizationStatusChanged;
    public AuthorizationProfile currentProfile;

    private AuthorizationProfile[] availableProfiles;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        LoadAuthorizationProfiles();
        // currentProfile akan tetap null di awal, sesuai keinginan.
        // Jika Anda ingin profil default (misal Guest) saat tidak ada login,
        // Anda bisa tambahkan: currentProfile = availableProfiles.FirstOrDefault(p => p.roleName == "Guest");
        // Tapi karena Anda ingin kosong, biarkan saja null.

        OnAuthorizationStatusChanged?.Invoke(); // Panggil ini untuk mengupdate UI awal
    }
    private void LoadAuthorizationProfiles()
    {
        availableProfiles = Resources.LoadAll<AuthorizationProfile>("AuthorizationProfiles");
        if (availableProfiles.Length == 0)
        {
            Debug.LogError("No Authorization Profiles found in Resources/AuthorizationProfiles. Please create some.");
        }
        // Tambahkan ini untuk memastikan passwordHash terisi jika kosong (untuk demo)
        foreach (var profile in availableProfiles)
        {
            if (string.IsNullOrEmpty(profile.passwordHash))
            {
                profile.passwordHash = profile.username; // Untuk demo, password sama dengan username
                Debug.LogWarning($"Authorization Profile '{profile.roleName}' had empty passwordHash. Set to username for demo purposes.");
            }
        }
    }


    private void LoadAllAuthorizationProfiles()
    {   
        AuthorizationProfile[] profiles = Resources.LoadAll<AuthorizationProfile>("AuthorizationProfiles"); // Pastikan ini ada di folder Resources/AuthorizationProfiles
        userProfiles.Clear(); // Bersihkan dictionary jika ada data sebelumnya

        foreach (var profile in profiles)
        {
            if (!string.IsNullOrEmpty(profile.username) && !userProfiles.ContainsKey(profile.username))
            {
                userProfiles.Add(profile.username, profile);
                Debug.Log($"Loaded Authorization Profile for user: {profile.username} ({profile.roleName})");

                // Untuk pengembangan: Jika passwordHash kosong, set agar mudah di-debug.
                // HANYA UNTUK KEPERLUAN DEMO/DEVELOPMENT. Hapus ini di produksi.
                if (string.IsNullOrEmpty(profile.passwordHash))
                {
                    Debug.LogWarning($"Profile '{profile.username}' has no password. Setting password to its username for dev purposes.");
                    profile.passwordHash = profile.username; // Misalnya, password default adalah username
                }
            }
            else
            {
                Debug.LogWarning($"Duplicate or invalid username found for Authorization Profile: {profile.roleName}. Skipping.");
            }
        }

        // Set default profile jika belum diatur atau jika tidak ditemukan
        if (currentProfile == null && userProfiles.Count > 0)
        {
            Debug.LogWarning("No default Authorization Profile assigned. Assigning the first loaded profile as current (for initial state).");
            foreach (var profile in userProfiles.Values)
            {
                currentProfile = profile;
                break;
            }
        }
        else if (currentProfile != null && !userProfiles.ContainsValue(currentProfile))
        {
            Debug.LogWarning($"Current assigned profile '{currentProfile.roleName}' (username: {currentProfile.username}) not found in loaded profiles. Please check your Resources folder.");
        }
    }

    /// <summary>
    /// Mencoba login dengan username dan password yang diberikan.
    /// Mengatur profil otorisasi saat ini jika login berhasil.
    /// </summary>
    /// <param name="username">Username yang dimasukkan.</param>
    /// <param name="password">Password yang dimasukkan.</param>
    /// <returns>True jika login berhasil, False jika gagal (username tidak ditemukan atau password salah).</returns>
    public bool TryLogin(string username, string password)
    {
        AuthorizationProfile foundProfile = availableProfiles.FirstOrDefault(p => p.username == username && p.passwordHash == password);

        if (foundProfile != null)
        {
            currentProfile = foundProfile;
            OnAuthorizationStatusChanged?.Invoke();
            Debug.Log($"Login successful for user: {username} with role: {currentProfile.roleName}");
            return true;
        }
        else
        {
            Debug.LogWarning($"Login failed for user: {username}. Incorrect username or password.");
            return false;
        }
    }

    /// <summary>
    /// Mengatur profil saat ini menjadi null (log out).
    /// </summary>
    public void Logout()
    {
        currentProfile = null;
        Debug.Log("User logged out.");
        OnAuthorizationStatusChanged.Invoke();
        // Anda mungkin ingin mengatur profil default tamu/anonim di sini jika ada
    }
    // --- Metode untuk memeriksa Izin ---

    public bool CanLogin() => currentProfile != null && currentProfile.canLogin;
    public bool CanActivateIsolator() => currentProfile != null && currentProfile.canActivateIsolator;
    public bool CanOnOrOffElectricalPowerViableAirSampler() => currentProfile != null && currentProfile.canOnOrOffElectricalPowerViableAirSampler;
    public bool CanOnOrOffElectricalPowerSterilityTestPump() => currentProfile != null && currentProfile.canOnOrOffElectricalPowerSterilityTestPump;
    public bool CanOpenOrCloceDrainValveLine() => currentProfile != null && currentProfile.canOpenOrCloceDrainValveLine;
    public bool CanActivatePowerChamberLighting() => currentProfile != null && currentProfile.canActivatePowerChamberLighting;
    public bool CanRequestOpenChamberMainDoor() => currentProfile != null && currentProfile.canRequestOpenChamberMainDoor;
    public bool CanAlarmsAcknowledgement() => currentProfile != null && currentProfile.canAlarmsAcknowledgement;
    public bool CanMuteAlarm() => currentProfile != null && currentProfile.canMuteAlarm;
    public bool CanViewsAlarmLogHistory() => currentProfile != null && currentProfile.canViewsAlarmLogHistory;
    public bool CanShutdownIsolatorSystem() => currentProfile != null && currentProfile.canShutdownIsolatorSystem;
    public bool CanResetEmergencyStopHMIScreen() => currentProfile != null && currentProfile.canResetEmergencyStopHMIScreen;

    public bool CanAccessNavigateOperationScreen() => currentProfile != null && currentProfile.canAccessNavigateOperationScreen;
    public bool CanStartStopChamberPressureDecayTest() => currentProfile != null && currentProfile.canStartStopChamberPressureDecayTest;
    public bool CanBypassChamberPressureDecayTest() => currentProfile != null && currentProfile.canBypassChamberPressureDecayTest;
    public bool CanStartStopChamberBiodecontaminationOperation() => currentProfile != null && currentProfile.canStartStopChamberBiodecontaminationOperation;
    public bool CanBypassChamberBiodecontaminationOperation() => currentProfile != null && currentProfile.canBypassChamberBiodecontaminationOperation;
    public bool CanAbortTheAeration() => currentProfile != null && currentProfile.canAbortTheAeration;
    public bool CanStartStopGloveLeakTest() => currentProfile != null && currentProfile.canStartStopGloveLeakTest;
    public bool CanClearGloveLeakTest() => currentProfile != null && currentProfile.canClearGloveLeakTest;
    public bool CanStartStopIsolatorNormalRun() => currentProfile != null && currentProfile.canStartStopIsolatorNormalRun;

    public bool CanAccessSettingsScreen() => currentProfile != null && currentProfile.canAccessSettingsScreen;
    public bool CanAdjustIsolatorOperatingSetpointsAndTimer() => currentProfile != null && currentProfile.canAdjustIsolatorOperatingSetpointsAndTimer;
    public bool CanEnableDisableAlarms() => currentProfile != null && currentProfile.canEnableDisableAlarms;
    public bool CanChangeAlarmSetpoints() => currentProfile != null && currentProfile.canChangeAlarmSetpoints;
    public bool CanChangeBiodeconCycleDev() => currentProfile != null && currentProfile.canChangeBiodeconCycleDev;
    public bool CanChangeOrStartCleanScreenAndTimer() => currentProfile != null && currentProfile.canChangeOrStartCleanScreenAndTimer;
    public bool CanOpenHMIControlPanel() => currentProfile != null && currentProfile.canOpenHMIControlPanel;
    public bool CanPerformCalibrateTouch() => currentProfile != null && currentProfile.canPerformCalibrateTouch;
    public bool CanAdjustScreenBrightness() => currentProfile != null && currentProfile.canAdjustScreenBrightness;
    public bool CanEnableThermalPrinterFunction() => currentProfile != null && currentProfile.canEnableThermalPrinterFunction;
    public bool CanAccessPDFViewer() => currentProfile != null && currentProfile.canAccessPDFViewer;
    public bool CanTurnOnOrOffElectronicsLogging() => currentProfile != null && currentProfile.canTurnOnOrOffElectronicsLogging;
    public bool CanChangeDurationForContinuousDataLoggingInterval() => currentProfile != null && currentProfile.canChangeDurationForContinuousDataLoggingInterval;
    public bool CanChangesettingsOnlineParticleCounter() => currentProfile != null && currentProfile.canChangesettingsOnlineParticleCounter;
    public bool CanOpenfilebrowser() => currentProfile != null && currentProfile.canOpenfilebrowser;
    public bool CanTransferAllLogstoArchivePath() => currentProfile != null && currentProfile.canTransferAllLogstoArchivePath;
    public bool CanChangeDateTimeInSettingsScreen() => currentProfile != null && currentProfile.canChangeDateTimeInSettingsScreen;

    public bool CanAccessNavigateServiceScreen() => currentProfile != null && currentProfile.canAccessNavigateServiceScreen;
    public bool CanForceOverrideIOs() => currentProfile != null && currentProfile.canForceOverrideIOs; // Using the corrected name
    public bool CanAccessChangeScalingValues() => currentProfile != null && currentProfile.canAccessChangeScalingValues;
    public bool CanAccessChangeClosedLoopControlsTuning() => currentProfile != null && currentProfile.canAccessChangeClosedLoopControlsTuning;
    public bool CanAccessSystemMaintenance() => currentProfile != null && currentProfile.canAccessSystemMaintenance;
    public bool CanAccessDiagnosticVverview() => currentProfile != null && currentProfile.canAccessDiagnosticVverview; // Using the corrected name
    public bool CanAccessSystemAlarmsEvents() => currentProfile != null && currentProfile.canAccessSystemAlarmsEvents;
    public bool CanAccessRestoreFactoryDefaultSettings() => currentProfile != null && currentProfile.canAccessRestoreFactoryDefaultSettings;


    // --- NEW PERMISSION CHECKS ---
    public bool CanAccessNavigateUserAdminScreen() => currentProfile != null && currentProfile.canAccessNavigateUserAdminScreen;
    public bool CanCreationOrDeletionAccounts() => currentProfile != null && currentProfile.canCreationOrDeletionAccounts;
    public bool CanChangeAuthorizationLevel() => currentProfile != null && currentProfile.canChangeAuthorizationLevel;
    public bool CanUserChangeLogoffTime() => currentProfile != null && currentProfile.canUserChangeLogoffTime;

    public bool CanAccessContactUsScreen() => currentProfile != null && currentProfile.canAccessContactUsScreen;
    public bool CanUserPasswordChange() => currentProfile != null && currentProfile.canUserPasswordChange; // <<< NEW METHOD

    // Generic check for a permission (less performant than direct calls, but flexible)
    public bool HasPermission(string permissionName)
    {
        if (currentProfile == null)
        {
            return false; // Tidak ada yang login, tidak ada izin
        }

        // Menggunakan refleksi untuk mendapatkan nilai boolean dari nama permission
        var field = typeof(AuthorizationProfile).GetField(permissionName);
        if (field != null && field.FieldType == typeof(bool))
        {
            return (bool)field.GetValue(currentProfile);
        }
        Debug.LogWarning($"Permission '{permissionName}' not found or is not a boolean field in AuthorizationProfile.");
        return false;
    }

    /// <summary>
    /// Mengubah password untuk user tertentu (memerlukan izin `canUserPasswordChange`).
    /// </summary>
    /// <param name="targetUsername">Username yang passwordnya akan diubah.</param>
    /// <param name="newPassword">Password baru.</param>
    /// <returns>True jika password berhasil diubah, False jika tidak ada izin atau user tidak ditemukan.</returns>
    public bool ChangeUserPassword(string targetUsername, string newPassword)
    {
        if (currentProfile == null || !currentProfile.canUserPasswordChange)
        {
            Debug.LogWarning($"Permission denied: Current user ({currentProfile?.username ?? "N/A"}) cannot change user passwords.");
            return false;
        }

        if (userProfiles.TryGetValue(targetUsername, out AuthorizationProfile targetProfile))
        {
            // DALAM APLIKASI PRODUKSI NYATA, GUNAKAN FUNGSI HASHING UNTUK newPassword!
            targetProfile.passwordHash = newPassword; // Untuk demo, langsung set string
            Debug.Log($"Password for user '{targetUsername}' changed successfully by '{currentProfile.username}'.");
            // Anda mungkin perlu menyimpan perubahan ini jika menggunakan sistem penyimpanan non-Resources
            // Misalnya: EditorUtility.SetDirty(targetProfile); AssetDatabase.SaveAssets(); jika di Editor
            return true;
        }
        else
        {
            Debug.LogWarning($"Failed to change password: User '{targetUsername}' not found.");
            return false;
        }
    }
    public void TriggerAuthorizationStatusChanged()
    {
        OnAuthorizationStatusChanged?.Invoke();
    }
}