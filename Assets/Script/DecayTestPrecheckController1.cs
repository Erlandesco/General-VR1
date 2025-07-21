// DecayTestPrecheckController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DecayTestPrecheckController1 : MonoBehaviour
{
    [System.Serializable]
    public class Prerequisite
    {
        public string name;
        public TextMeshProUGUI statusText;
        public Image statusIcon;
        public Sprite checkmarkIcon;
        public Sprite crossIcon;

        public void UpdateVisualStatus(bool isMet)
        {
            if (statusIcon != null)
            {
                statusIcon.sprite = isMet ? checkmarkIcon : crossIcon;
                statusIcon.color = isMet ? Color.green : Color.red;
            }
        }
    }

    [Header("Main UI Sections (Parent GameObjects)")]
    public GameObject precheckBusyPanel;
    public GameObject infoDetailPanel;
    public Button readyToDecayButton;

    [Header("Busy Panel Elements")]
    public Button pressForMoreInfoButton;

    [Header("Info Detail Panel Elements")]
    public Button closeInfoPanelButton;
    public List<Prerequisite> prerequisites;

    // --- DETEKSI PINTU BARU DENGAN COLLIDER ---
    [Header("Door Collider Sensors")]
    public DoorSensorTrigger doorClosedSensor; // Seret GameObject DoorClosedSensor ke sini
    public DoorSensorTrigger doorOpenSensor;   // Seret GameObject DoorOpenSensor ke sini
    // --- AKHIR DETEKSI COLLIDER ---

    [Header("Chamber Specifics")]
    public HingeJoint chamberDoorHingeJoint; // Masih diperlukan untuk mengontrol pintu
    public float doorClosedAngleThreshold = 0.5f; // Masih bisa digunakan untuk internal logic motor jika mau

    [Header("Door Initial State (Motor Controlled)")]
    public bool openDoorOnStart = true;
    public float initialOpenTargetAngle = -60f;
    public float initialOpenSpeed = 50f;
    public float initialOpenForce = 100f;

    private Rigidbody doorRigidbody;
    private bool isInfoPanelActive = false;

    void Awake()
    {
        if (chamberDoorHingeJoint != null)
        {
            doorRigidbody = chamberDoorHingeJoint.GetComponent<Rigidbody>();
            if (doorRigidbody == null)
            {
                Debug.LogError("Door GameObject with HingeJoint does not have a Rigidbody component! HingeJoints require a Rigidbody for physics control.");
            }
        }
    }

    void Start()
    {
        Debug.Log("DecayTestPrecheckController Start() called.");

        if (precheckBusyPanel == null || infoDetailPanel == null || readyToDecayButton == null ||
            pressForMoreInfoButton == null || closeInfoPanelButton == null || chamberDoorHingeJoint == null ||
            doorClosedSensor == null || doorOpenSensor == null) // Tambahkan pengecekan sensor
        {
            Debug.LogError("DecayTestPrecheckController: One or more UI or HingeJoint/Sensor references are NOT SET in the Inspector!");
            this.enabled = false;
            return;
        }

        infoDetailPanel.SetActive(false);
        precheckBusyPanel.SetActive(false);
        readyToDecayButton.gameObject.SetActive(false);

        pressForMoreInfoButton.onClick.AddListener(OnPressForMoreInfoButtonClicked);
        closeInfoPanelButton.onClick.AddListener(OnCloseInfoPanelButtonClicked);

        // Mengatur posisi awal pintu menggunakan motor HingeJoint
        if (doorRigidbody != null && openDoorOnStart)
        {
            JointMotor motor = chamberDoorHingeJoint.motor;
            motor.targetVelocity = (initialOpenTargetAngle > chamberDoorHingeJoint.angle) ? initialOpenSpeed : -initialOpenSpeed;
            motor.force = initialOpenForce;
            chamberDoorHingeJoint.motor = motor;
            chamberDoorHingeJoint.useMotor = true;

            Debug.Log($"Door motor activated to open to {initialOpenTargetAngle} degrees on Start. Current HingeJoint angle: {chamberDoorHingeJoint.angle}.");

            // Karena deteksi sekarang menggunakan collider, kita tidak perlu menunggu HingeJoint.angle stabil
            // Coroutine ini mungkin masih berguna jika Anda ingin motor berhenti otomatis
            // StartCoroutine(StopMotorAfterDelay(initialOpenTargetAngle, initialOpenSpeed / initialOpenForce * 2f));
        }
        else if (doorRigidbody == null && openDoorOnStart)
        {
            Debug.LogError("Door Rigidbody is NULL! Cannot activate HingeJoint motor. Ensure door has a Rigidbody.");
        }
        else // Jika openDoorOnStart false, pastikan motor nonaktif
        {
            chamberDoorHingeJoint.useMotor = false;
        }

        // Panggil update status awal UI.
        // Karena deteksi sekarang dari collider, ini akan lebih cepat akurat.
        UpdateDecayTestUIState();
        Debug.Log("DecayTestPrecheckController Start() finished. Initial UI state set.");
    }

    void Update()
    {
        // Terus periksa status pintu dan update UI
        // Sekarang update bisa lebih sering atau hanya saat panel terlihat
        // UpdateDecayTestUIState(); // Bisa dipanggil lebih sering jika diperlukan
        if (isInfoPanelActive || precheckBusyPanel.activeSelf)
        {
            CheckAllPrerequisitesAndRefreshUI();
        }
    }
    // ISI DARI FUNGSI INI:
    private void OnPressForMoreInfoButtonClicked()
    {
        Debug.Log("Press For More Info button clicked.");
        infoDetailPanel.SetActive(true); // Aktifkan panel informasi detail
        isInfoPanelActive = true;
        // Langsung refresh status visual saat panel dibuka
        CheckAllPrerequisitesAndRefreshUI();
    }

    // ISI DARI FUNGSI INI:
    private void OnCloseInfoPanelButtonClicked()
    {
        Debug.Log("Close Info Panel button clicked.");
        infoDetailPanel.SetActive(false); // Sembunyikan panel informasi detail
        isInfoPanelActive = false;
        // Setelah menutup panel detail, perbarui status UI utama (Busy/Ready)
        UpdateDecayTestUIState();
    }


    // Metode utama untuk menentukan panel mana yang harus aktif (Busy atau Ready)
    public void UpdateDecayTestUIState()
    {
        Debug.Log("UpdateDecayTestUIState() called.");
        bool allPrerequisitesMet = CheckAllPrerequisites();
        Debug.Log($"All prerequisites met (based on collider): {allPrerequisitesMet}");

        if (allPrerequisitesMet)
        {
            precheckBusyPanel.SetActive(false);
            readyToDecayButton.gameObject.SetActive(true);
            Debug.Log("Decay Test UI State: Set to READY (Button to Decay active).");
        }
        else
        {
            precheckBusyPanel.SetActive(true);
            readyToDecayButton.gameObject.SetActive(false);
            Debug.Log("Decay Test UI State: Set to BUSY (Precheck Busy Panel active).");
        }
        Debug.Log($"Current precheckBusyPanel active: {precheckBusyPanel.activeSelf}");
        Debug.Log($"Current readyToDecayButton active: {readyToDecayButton.gameObject.activeSelf}");
    }

    // Metode untuk mengecek semua prasyarat (sekarang menggunakan sensor collider)
    private bool CheckAllPrerequisites()
    {
        // Prasyarat "pintu tertutup" terpenuhi jika doorClosedSensor mendeteksi pintu
        bool doorClosed = doorClosedSensor.IsDoorInside;

        // Debugging sensor status
        Debug.Log($"DoorClosedSensor.IsDoorInside: {doorClosedSensor.IsDoorInside}");
        Debug.Log($"DoorOpenSensor.IsDoorInside: {doorOpenSensor.IsDoorInside}");

        // Tambahkan pengecekan untuk prasyarat lain di masa depan
        // Contoh: bool tankFilled = CheckTankStatus();
        // return doorClosed && tankFilled;

        return doorClosed; // Untuk sekarang, hanya tergantung pada pintu tertutup
    }

    // Metode ini tidak lagi digunakan untuk menentukan status tertutup
    // Namun bisa tetap ada untuk debugging atau internal logic lain jika perlu.
    private bool CheckChamberDoorStatus()
    {
        if (chamberDoorHingeJoint == null)
        {
            Debug.LogError("Chamber Door HingeJoint is NULL. Cannot check door status!");
            return false;
        }
        // Ini adalah cara lama mendeteksi tertutup, sekarang diganti oleh collider
        bool isClosedViaAngle = Mathf.Abs(chamberDoorHingeJoint.angle) < doorClosedAngleThreshold;
        Debug.Log($"Chamber Door Hinge Angle (for reference): {chamberDoorHingeJoint.angle}. Is Closed via Angle: {isClosedViaAngle}");
        return isClosedViaAngle; // Mengembalikan nilai berdasarkan sudut HingeJoint
    }

    private void CheckAllPrerequisitesAndRefreshUI()
    {
        foreach (var prereq in prerequisites)
        {
            if (prereq.name == "Chamber Door Closed")
            {
                // Gunakan status dari collider sensor
                bool isDoorClosedViaCollider = doorClosedSensor.IsDoorInside;
                prereq.UpdateVisualStatus(isDoorClosedViaCollider);
                Debug.Log($"Updating UI for 'Chamber Door Closed'. Status: {isDoorClosedViaCollider}");
            }
            // Tambahkan else if untuk prasyarat lain di masa depan
        }
    }

    // ... (Button Click Handlers dan OnDestroy tetap sama) ...

    // Coroutine untuk mematikan motor setelah mencapai target (jika tidak pakai limit)
    System.Collections.IEnumerator StopMotorAfterDelay(float targetAngle, float delay)
    {
        yield return new WaitForSeconds(delay); // Beri waktu motor untuk bergerak
        if (chamberDoorHingeJoint != null && chamberDoorHingeJoint.useMotor)
        {
            // Cek apakah sudah mendekati target, atau apakah sensor sudah mendeteksi posisi terbuka
            if (doorOpenSensor.IsDoorInside || Mathf.Abs(chamberDoorHingeJoint.angle - targetAngle) < doorClosedAngleThreshold * 2f)
            {
                chamberDoorHingeJoint.useMotor = false;
                Debug.Log($"Door motor stopped after reaching target/sensor detected open. Current angle: {chamberDoorHingeJoint.angle}");
            }
            else
            {
                Debug.LogWarning("Motor did not reach target angle or open sensor not triggered, keeping motor active. Check limits or force.");
            }
        }
    }

    // Manual Door Control methods (OpenDoorManually, CloseDoorManually) tetap sama
    public float manualOpenSpeed = 50f;
    public float manualCloseSpeed = -50f;
    public float motorForce = 100f;

    public void CloseDoorManually()
    {
        if (chamberDoorHingeJoint != null)
        {
            JointMotor motor = chamberDoorHingeJoint.motor;
            motor.targetVelocity = manualCloseSpeed;
            motor.force = motorForce;
            chamberDoorHingeJoint.motor = motor;
            chamberDoorHingeJoint.useMotor = true;
            Debug.Log("Attempting to close door manually.");
        }
    }

    public void OpenDoorManually()
    {
        if (chamberDoorHingeJoint != null)
        {
            JointMotor motor = chamberDoorHingeJoint.motor;
            motor.targetVelocity = manualOpenSpeed;
            motor.force = motorForce;
            chamberDoorHingeJoint.motor = motor;
            chamberDoorHingeJoint.useMotor = true;
            Debug.Log("Attempting to open door manually.");
        }
    }
}