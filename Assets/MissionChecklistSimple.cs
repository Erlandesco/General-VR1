using UnityEngine;
using UnityEngine.InputSystem;

public class MissionChecklistSimple : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Root GameObject atau Canvas GameObject dari Mission Panel")]
    public GameObject missionUI;

    [Tooltip("Hint Y (opsional): otomatis disembunyikan saat mission terbuka")]
    public GameObject yHintUI;

    [Header("Input")]
    [Tooltip("Input Action untuk tombol Y (performed = toggle)")]
    public InputActionReference toggleMissionAction;

    public bool IsMissionOpen { get; private set; }

    [Header("Events (opsional)")]
    public UnityEngine.Events.UnityEvent onMissionOpened;
    public UnityEngine.Events.UnityEvent onMissionClosed;

    void Awake()
    {
        // Default: mission tertutup, hint aktif
        SetMission(false, invokeEvents: false);
    }

    void OnEnable()
    {
        if (toggleMissionAction != null)
        {
            toggleMissionAction.action.performed += OnToggle;
            toggleMissionAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (toggleMissionAction != null)
        {
            toggleMissionAction.action.performed -= OnToggle;
            toggleMissionAction.action.Disable();
        }
    }

    void OnToggle(InputAction.CallbackContext _)
    {
        SetMission(!IsMissionOpen, invokeEvents: true);
    }

    /// <summary>Buka/tutup mission + sinkron flag & hint.</summary>
    public void SetMission(bool open, bool invokeEvents)
    {
        IsMissionOpen = open;

        if (missionUI) missionUI.SetActive(open);
        if (yHintUI) yHintUI.SetActive(!open);

        if (invokeEvents)
        {
            if (open) onMissionOpened?.Invoke();
            else onMissionClosed?.Invoke();
        }
    }
}
