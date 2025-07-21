// DecayTestPrecheckController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Untuk TextMeshPro
using System.Collections.Generic; // Untuk List
using System.Collections; //

public class DecayTestPrecheckController : MonoBehaviour
{
    [System.Serializable]
    public class Prerequisite
    {
        public string name; // Contoh: "Chamber Door Closed"
        public TextMeshProUGUI statusText; // TextMeshProUGUI untuk menampilkan deskripsi prasyarat
        public Image statusIcon; // Image untuk menampilkan centang/silang
        public Sprite checkmarkIcon; // Sprite untuk centang
        public Sprite crossIcon; // Sprite untuk silang

        // Metode untuk mengupdate status visual
        public void UpdateVisualStatus(bool isMet)
        {
            if (statusIcon != null)
            {
                statusIcon.sprite = isMet ? checkmarkIcon : crossIcon;
                statusIcon.color = isMet ? Color.green : Color.red; // Contoh: warna hijau untuk centang, merah untuk silang
            }
        }
    }
    private Rigidbody doorRigidbody;

    [Header("Main UI Sections (Parent GameObjects)")]
    public GameObject precheckBusyPanel; // Referensi ke "if decaytes not ready"
    public GameObject infoDetailPanel;   // Referensi ke "Information"
    public Button readyToDecayButton;  // Referensi ke "Button to Decay"

    [Header("Busy Panel Elements")]
    public Button pressForMoreInfoButton; // Referensi ke "Press For More Information"

    [Header("Info Detail Panel Elements")]
    public Button closeInfoPanelButton; // Referensi ke "Close" Button
    public List<Prerequisite> prerequisites; // Daftar semua prasyarat

    [Header("Chamber Specifics")]
    public HingeJoint chamberDoorHingeJoint; // Referensi ke HingeJoint pintu
    public float doorClosedAngleThreshold = 0.5f; // Sudut maksimum agar pintu dianggap tertutup (mendekati 0)

    private bool isInfoPanelActive = false; // Status apakah panel informasi detail sedang aktif

    [Header("Door Initial State (Motor Controlled)")]
    public bool openDoorOnStart = true;    // Apakah pintu harus otomatis terbuka di Start?
    public float initialOpenTargetAngle = -60f; // Sudut target terbuka (misal -60)
    public float initialOpenSpeed = 50f;   // Kecepatan motor untuk membuka
    public float initialOpenForce = 100f;  // Kekuatan motor


    void Start()
    {
        Debug.Log("DecayTestPrecheckController Start() called.");

        if (precheckBusyPanel == null || infoDetailPanel == null || readyToDecayButton == null ||
            pressForMoreInfoButton == null || closeInfoPanelButton == null || chamberDoorHingeJoint == null)
        {
            Debug.LogError("DecayTestPrecheckController: One or more UI or HingeJoint references are NOT SET in the Inspector!");
            this.enabled = false;
            return;
        }

        infoDetailPanel.SetActive(false);
        precheckBusyPanel.SetActive(false);
        readyToDecayButton.gameObject.SetActive(false);

        pressForMoreInfoButton.onClick.AddListener(OnPressForMoreInfoButtonClicked);
        closeInfoPanelButton.onClick.AddListener(OnCloseInfoPanelButtonClicked);

        // --- PENTING: Mengatur posisi awal pintu ---
        if (doorRigidbody != null && openDoorOnStart)
        {
            JointMotor motor = chamberDoorHingeJoint.motor;
            motor.targetVelocity = (initialOpenTargetAngle > chamberDoorHingeJoint.angle) ? initialOpenSpeed : -initialOpenSpeed;
            motor.force = initialOpenForce;
            chamberDoorHingeJoint.motor = motor;
            chamberDoorHingeJoint.useMotor = true;

            Debug.Log($"Door motor activated to open to {initialOpenTargetAngle} degrees on Start. Current angle: {chamberDoorHingeJoint.angle} (at Start).");

            // BARU: Panggil Coroutine untuk menunda update UI awal
            StartCoroutine(DelayedInitialUIUpdate());
        }
        else if (doorRigidbody == null && openDoorOnStart)
        {
            Debug.LogError("Door Rigidbody is NULL! Cannot activate HingeJoint motor. Ensure door has a Rigidbody.");
            // Fallback jika Rigidbody hilang dan tidak ada motor
            // Ini akan membuat pintu terbuka secara instan tapi mungkin tidak sinkron dengan HingeJoint.angle
            // chamberDoorHingeJoint.transform.localRotation = Quaternion.Euler(chamberDoorHingeJoint.transform.localEulerAngles.x, initialOpenTargetAngle, chamberDoorHingeJoint.transform.localEulerAngles.z);

            // Masih perlu panggil update UI, mungkin perlu penundaan juga
            StartCoroutine(DelayedInitialUIUpdate());
        }
        else // Jika openDoorOnStart false, langsung update UI
        {
            chamberDoorHingeJoint.useMotor = false; // Pastikan motor nonaktif jika tidak dibuka
            UpdateDecayTestUIState();
            Debug.Log("DecayTestPrecheckController Start() finished. Initial UI state set (no door open animation).");
        }
    }

    void Awake() // Gunakan Awake untuk mendapatkan Rigidbody
    {
        if (chamberDoorHingeJoint != null)
        {
            doorRigidbody = chamberDoorHingeJoint.GetComponent<Rigidbody>();
            if (doorRigidbody == null)
            {
                Debug.LogError("Door GameObject with HingeJoint does not have a Rigidbody component! HingeJoints require a Rigidbody.");
            }
        }
    }

    // BARU: Coroutine untuk menunda UpdateDecayTestUIState()
    private IEnumerator DelayedInitialUIUpdate()
    {
        // Tunggu satu FixedUpdate frame (di mana fisika dihitung)
        yield return new WaitForFixedUpdate();

        // Atau tunggu beberapa frame, tergantung seberapa cepat pintu Anda berputar
        yield return new WaitForSeconds(3f); // Tunggu 0.1 detik

        // Setelah penundaan, panggil update status UI
        UpdateDecayTestUIState();
        Debug.Log("DecayTestPrecheckController: Delayed initial UI update performed.");

        // Opsional: Matikan motor setelah pintu terbuka jika Anda yakin sudah mencapai target
        // Ini jika Anda TIDAK menggunakan HingeJoint.limits
        // if (chamberDoorHingeJoint != null && chamberDoorHingeJoint.useMotor)
        // {
        //     if (Mathf.Abs(chamberDoorHingeJoint.angle - initialOpenTargetAngle) < doorClosedAngleThreshold * 2f)
        //     {
        //         chamberDoorHingeJoint.useMotor = false;
        //         Debug.Log("Door motor stopped after initial open.");
        //     }
        // }
    }


    System.Collections.IEnumerator StopMotorAfterDelay(float targetAngle, float delay)
    {
        yield return new WaitForSeconds(delay); // Beri waktu motor untuk bergerak
        if (chamberDoorHingeJoint != null && chamberDoorHingeJoint.useMotor)
        {
            // Cek apakah sudah mendekati target
            if (Mathf.Abs(chamberDoorHingeJoint.angle - targetAngle) < doorClosedAngleThreshold * 2f) // Gunakan threshold lebih besar untuk "dekat"
            {
                chamberDoorHingeJoint.useMotor = false;
                Debug.Log($"Door motor stopped after reaching target angle {chamberDoorHingeJoint.angle}");
            }
            else
            {
                Debug.LogWarning("Motor did not reach target angle, keeping motor active. Check limits or force.");
            }
        }
    }



    void Update()
    {
        // Terus periksa status pintu dan update UI jika panel informasi detail sedang aktif
        // atau jika kita berada di precheckBusyPanel (untuk mengantisipasi transisi otomatis)
        if (isInfoPanelActive || precheckBusyPanel.activeSelf)
        {
            CheckAllPrerequisitesAndRefreshUI();
        }
    }

    // Metode utama untuk menentukan panel mana yang harus aktif (Busy atau Ready)
    private void UpdateDecayTestUIState()
    {
        Debug.Log("UpdateDecayTestUIState() called."); // Debug 4
        bool allPrerequisitesMet = CheckAllPrerequisites();
        Debug.Log($"All prerequisites met: {allPrerequisitesMet}"); // Debug 5

        if (allPrerequisitesMet)
        {
            precheckBusyPanel.SetActive(false);
            readyToDecayButton.gameObject.SetActive(true); // Aktifkan Button to Decay
            Debug.Log("Decay Test UI State: Set to READY (Button to Decay active)."); // Debug 6
        }
        else
        {
            precheckBusyPanel.SetActive(true);
            readyToDecayButton.gameObject.SetActive(false); // Nonaktifkan Button to Decay
            Debug.Log("Decay Test UI State: Set to BUSY (Precheck Busy Panel active)."); // Debug 7
        }
        Debug.Log($"Current precheckBusyPanel active: {precheckBusyPanel.activeSelf}"); // Debug 8
        Debug.Log($"Current readyToDecayButton active: {readyToDecayButton.gameObject.activeSelf}"); // Debug 9
    }


    // Metode untuk mengecek semua prasyarat (saat ini hanya pintu)
    private bool CheckAllPrerequisites()
    {
        bool doorClosed = CheckChamberDoorStatus();
        // Di sini Anda akan menambahkan pengecekan untuk prasyarat lain di masa depan
        // Contoh: bool tankFilled = CheckTankStatus();
        // return doorClosed && tankFilled;
        return doorClosed; // Untuk sekarang, hanya tergantung pada pintu
    }

    // Metode spesifik untuk mengecek status pintu
    private bool CheckChamberDoorStatus()
    {
        if (chamberDoorHingeJoint == null)
        {
            Debug.LogError("Chamber Door HingeJoint is NULL. Cannot check door status!"); // Debug 10
            return false;
        }
        bool isClosed = Mathf.Abs(chamberDoorHingeJoint.angle) < doorClosedAngleThreshold;
        Debug.Log($"Chamber Door Hinge Angle: {chamberDoorHingeJoint.angle}. Is Closed: {isClosed}"); // Debug 11
        return isClosed;
    }


    // Metode untuk mengupdate status visual semua prasyarat di infoDetailPanel
    private void CheckAllPrerequisitesAndRefreshUI()
    {
        // Untuk setiap prasyarat dalam list, update visualnya
        foreach (var prereq in prerequisites)
        {
            // Perlu logika untuk menentukan apakah prasyarat ini terpenuhi
            // Contoh: Jika prereq.name adalah "Chamber Door Closed"
            if (prereq.name == "Chamber Door Closed") // Sesuaikan nama yang Anda berikan di Inspector
            {
                bool isDoorClosed = CheckChamberDoorStatus();
                prereq.UpdateVisualStatus(isDoorClosed);
            }
            // Tambahkan else if untuk prasyarat lain di masa depan
        }
    }

    // --- Button Click Handlers ---

    private void OnPressForMoreInfoButtonClicked()
    {
        Debug.Log("Press For More Info button clicked.");
        // Panel "if decaytes not ready" tetap aktif, hanya info detail yang muncul di atasnya
        // precheckBusyPanel.SetActive(false); // Hapus baris ini sesuai revisi Anda

        infoDetailPanel.SetActive(true); // Aktifkan panel informasi detail
        isInfoPanelActive = true;
        CheckAllPrerequisitesAndRefreshUI(); // Update status visual segera setelah dibuka
    }

    private void OnCloseInfoPanelButtonClicked()
    {
        Debug.Log("Close Info Panel button clicked.");
        infoDetailPanel.SetActive(false); // Sembunyikan panel informasi detail
        isInfoPanelActive = false;
        UpdateDecayTestUIState(); // Panggil ini untuk mengecek ulang dan mengupdate UI utama
    }

    void OnDestroy()
    {
        // Pastikan untuk menghapus listener saat GameObject dihancurkan
        if (pressForMoreInfoButton != null)
        {
            pressForMoreInfoButton.onClick.RemoveListener(OnPressForMoreInfoButtonClicked);
        }
        if (closeInfoPanelButton != null)
        {
            closeInfoPanelButton.onClick.RemoveListener(OnCloseInfoPanelButtonClicked);
        }
    }
}