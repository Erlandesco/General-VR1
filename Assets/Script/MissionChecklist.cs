using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MissionChecklist_XRIT : MonoBehaviour
{
    [Header("UI")]
    public GameObject missionUI;
    public GameObject yHintUI; // canvas hint tombol Y
    public GameObject xHintUI; // canvas hint tombol X
    public Transform leftHandAnchor;

    [Header("XR")]
    public NearFarInteractor leftHandGroup;   // assign Interaction Group tangan kiri
    public HapticImpulsePlayer leftHandHaptic;    // XR Controller kiri untuk haptics

    [Header("Input")]
    public InputActionReference toggleMissionAction; // referensi input (button Y)

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sfxOpen;
    public AudioClip sfxClose;

    [Header("Haptics")]
    [Range(0f, 1f)] public float hapticAmplitudeOn = 0.5f;
    public float hapticDurationOn = 0.08f;
    [Range(0f, 1f)] public float hapticAmplitudeOff = 0.3f;
    public float hapticDurationOff = 0.05f;

    [Header("Hand Pose Params (Animator)")]
    public Animator leftHandAnimator;
    [Range(0f, 20f)] public float poseLerpSpeed = 8f; // makin besar makin cepat
    public string thumbParam = "Thumb";
    public string indexParam = "Index";
    public string middleParam = "Middle";
    public string ringParam = "Ring";
    public string pinkyParam = "Pinky";

    float targetThumb, targetIndex, targetMiddle, targetRing, targetPinky;

    private bool uiActive;

    void Update()
    {
        // update lerp setiap frame (halus)
        if (leftHandAnimator)
        {
            LerpParam(thumbParam, targetThumb);
            LerpParam(indexParam, targetIndex);
            LerpParam(middleParam, targetMiddle);
            LerpParam(ringParam, targetRing);
            LerpParam(pinkyParam, targetPinky);
        }
    }

    void Awake()
    {
        if (missionUI != null)
        {
            missionUI.SetActive(false);
            yHintUI.SetActive(true);
            xHintUI.SetActive(true);
        }
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
    void LerpParam(string name, float target)
    {
        float cur = leftHandAnimator.GetFloat(name);
        float next = Mathf.MoveTowards(cur, target, poseLerpSpeed * Time.deltaTime);
        leftHandAnimator.SetFloat(name, next);
    }

    void ApplyHandTargets(bool active)
    {
        if (active)
        {
            // Pose genggam dengan ibu jari tetap 0
            targetThumb = 0f;   // <- sesuai permintaanmu
            targetIndex = 1f;
            targetMiddle = 1f;
            targetRing = 1f;
            targetPinky = 1f;
        }
        else
        {
            // Kembali ke tangan rileks (semua 0, sesuaikan kalau punya default lain)
            targetThumb = 0f;
            targetIndex = 0f;
            targetMiddle = 0f;
            targetRing = 0f;
            targetPinky = 0f;
        }
    }

    private void OnToggle(InputAction.CallbackContext ctx)
    {
        ToggleUI();
    }

    private void ToggleUI()
    {
        uiActive = !uiActive;
        missionUI.SetActive(uiActive);
        yHintUI.SetActive(!uiActive);
        xHintUI.SetActive(!uiActive);

        if (uiActive)
        {
            missionUI.transform.SetParent(leftHandAnchor);
            missionUI.transform.localPosition = Vector3.zero;
            missionUI.transform.localRotation = Quaternion.identity;

            // matikan interaksi tangan kiri
            leftHandGroup.enabled = false;
            //if (leftHandGroup) leftHandGroup.enabled = false;

            // pose genggam
            if (leftHandAnimator) leftHandAnimator.SetBool("HoldChecklist", true);

            // efek
            PlaySFX(sfxOpen);
            PulseHaptics(hapticAmplitudeOn, hapticDurationOn);
            ApplyHandTargets(true);
        }
        else
        {
            missionUI.transform.SetParent(null);

            leftHandGroup.enabled = true;
            if (leftHandGroup) leftHandGroup.enabled = true;

            if (leftHandAnimator) leftHandAnimator.SetBool("HoldChecklist", false);

            PlaySFX(sfxClose);
            PulseHaptics(hapticAmplitudeOff, hapticDurationOff);
            ApplyHandTargets(false);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }

    private void PulseHaptics(float amplitude, float duration)
    {
        if (leftHandHaptic != null)
            leftHandHaptic.SendHapticImpulse(amplitude, duration);
    }
}