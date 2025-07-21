// AuthorizationProfile.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAuthorizationProfile", menuName = "HMI/Authorization Profile", order = 1)]
public class AuthorizationProfile : ScriptableObject
{
    [Header("Role Information")]
    public string roleName; // e.g., "Operator", "Supervisor", "Maintenance", "Admin"
    public int accessLevel; // e.g., 0 for Operator, 1 for Supervisor, etc. (Optional, useful for hierarchical access)
    public string username; // Untuk sistem login
    public string passwordHash; // Menggunakan hash password (lebih aman dari plaintext)

    [Header("General Operation Permissions")]
    public bool canLogin = false;
    public bool canActivateIsolator = false;
    public bool canOnOrOffElectricalPowerViableAirSampler = false;
    public bool canOnOrOffElectricalPowerSterilityTestPump = false;
    public bool canOpenOrCloceDrainValveLine = false;
    public bool canActivatePowerChamberLighting = false;
    public bool canRequestOpenChamberMainDoor = false;
    public bool canAlarmsAcknowledgement = false;
    public bool canMuteAlarm = false;
    public bool canViewsAlarmLogHistory = false;
    public bool canShutdownIsolatorSystem = false;
    public bool canResetEmergencyStopHMIScreen = false;

    [Header("Operation Screen Permissions")]
    public bool canAccessNavigateOperationScreen = false;
    public bool canStartStopChamberPressureDecayTest = false;
    public bool canBypassChamberPressureDecayTest = false;
    public bool canStartStopChamberBiodecontaminationOperation = false;
    public bool canBypassChamberBiodecontaminationOperation = false;
    public bool canAbortTheAeration = false;
    public bool canStartStopGloveLeakTest = false; // Corrected typo
    public bool canClearGloveLeakTest = false;
    public bool canStartStopIsolatorNormalRun = false;

    [Header("Settings Screen Permissions")]
    public bool canAccessSettingsScreen = false;
    public bool canAdjustIsolatorOperatingSetpointsAndTimer = false;
    public bool canEnableDisableAlarms = false;
    public bool canChangeAlarmSetpoints = false;
    public bool canChangeBiodeconCycleDev = false;
    public bool canChangeOrStartCleanScreenAndTimer = false;
    public bool canOpenHMIControlPanel = false;
    public bool canPerformCalibrateTouch = false;
    public bool canAdjustScreenBrightness = false;
    public bool canEnableThermalPrinterFunction = false;
    public bool canAccessPDFViewer = false;
    public bool canTurnOnOrOffElectronicsLogging = false;
    public bool canChangeDurationForContinuousDataLoggingInterval = false;
    public bool canChangesettingsOnlineParticleCounter = false;
    public bool canOpenfilebrowser = false;
    public bool canTransferAllLogstoArchivePath = false;
    public bool canChangeDateTimeInSettingsScreen = false;

    [Header("Service Screen Permissions")]
    public bool canAccessNavigateServiceScreen = false;
    public bool canForceOverrideIOs = false; // Corrected typo: caForceOverridel/O's -> canForceOverrideIOs
    public bool canAccessChangeScalingValues = false;
    public bool canAccessChangeClosedLoopControlsTuning = false;
    public bool canAccessSystemMaintenance = false;
    public bool canAccessDiagnosticVverview = false; // Corrected typo: DiagnosticVverview -> DiagnosticOverview
    public bool canAccessSystemAlarmsEvents = false;
    public bool canAccessRestoreFactoryDefaultSettings = false;

    // --- NEW PERMISSIONS ---
    [Header("User Admin Screen Permissions")]
    public bool canAccessNavigateUserAdminScreen = false;
    public bool canUserPasswordChange = false; // Untuk pengguna mengubah password mereka sendiri
    public bool canCreationOrDeletionAccounts = false;
    public bool canChangeAuthorizationLevel = false;
    public bool canUserChangeLogoffTime = false;

    [Header("Contact Us Screen Permissions")]
    public bool canAccessContactUsScreen = false;

    // Helper untuk memverifikasi password (akan digunakan oleh LoginManager)
    public bool VerifyPassword(string inputPassword)
    {
        // Untuk demo, kita akan melakukan perbandingan langsung.
        // DALAM APLIKASI PRODUKSI NYATA, ANDA HARUS MENGGUNAKAN HASHING PASSWORD (misalnya BCrypt atau PBKDF2)
        // Jangan pernah menyimpan atau membandingkan password dalam bentuk plaintext!
        return passwordHash == inputPassword;
    }
}