using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupUIInFront : MonoBehaviour
{
    [Header("References")]
    public Camera xrCamera;                       // drag Main Camera (XR)
    public Canvas popupCanvas;                    // Canvas (World Space)
    public CanvasGroup canvasGroup;               // untuk fade (opsional)
    public InputActionReference toggleAction;     // binding ke tombol X (Left / North)

    [Header("Placement")]
    public float spawnDistance = 1.5f;
    public float heightOffset = -0.05f;
    public bool flattenForward = true;

    [Header("Auto Hide")]
    public float autoHideDelay = 4.0f;
    public bool fadeWhenHiding = true;
    public float fadeDuration = 0.15f;

    [Header("Audio")]
    public AudioSource audioSource;               // TARUH DI MAIN CAMERA / OBJEK YANG TIDAK DI-DISABLE
    public AudioClip sfxOpen;
    public AudioClip sfxClose;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public bool sfx2D = true;                     // 2D biar konsisten di telinga

    bool visible;
    float lastInteractionTime;

    void Reset()
    {
        xrCamera = Camera.main;
        popupCanvas = GetComponentInChildren<Canvas>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    void Awake()
    {
        if (popupCanvas != null) popupCanvas.enabled = false;
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        // Siapkan AudioSource kalau belum di-assign
        if (!audioSource)
        {
            var cam = Camera.main;
            if (cam)
                audioSource = cam.GetComponent<AudioSource>() ?? cam.gameObject.AddComponent<AudioSource>();
        }
        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = sfx2D ? 0f : 1f;
        }
    }

    void OnEnable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed += OnTogglePerformed;
            toggleAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed -= OnTogglePerformed;
            toggleAction.action.Disable();
        }
    }

    void Update()
    {
        if (!visible) return;

        // Auto-hide bila idle
        if (Time.time - lastInteractionTime > autoHideDelay)
        {
            Hide();
        }

        // Jaga selalu menghadap user
        if (xrCamera && popupCanvas)
        {
            Vector3 toCam = xrCamera.transform.position - popupCanvas.transform.position;
            if (flattenForward) toCam.y = 0f;
            if (toCam.sqrMagnitude > 0.0001f)
                popupCanvas.transform.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
        }
    }

    void OnTogglePerformed(InputAction.CallbackContext _)
    {
        if (!visible) Show();
        else Hide();
    }

    public void Show()
    {
        if (!xrCamera || !popupCanvas) return;

        // Posisi di depan kamera pada jarak tertentu
        Vector3 fwd = xrCamera.transform.forward;
        if (flattenForward) fwd = Vector3.ProjectOnPlane(fwd, Vector3.up).normalized;
        if (fwd.sqrMagnitude < 0.0001f) fwd = xrCamera.transform.forward;

        Vector3 pos = xrCamera.transform.position + fwd * spawnDistance;
        pos.y = xrCamera.transform.position.y + heightOffset;

        popupCanvas.transform.position = pos;
        popupCanvas.transform.rotation = Quaternion.LookRotation(-fwd, Vector3.up);

        // Tampilkan
        visible = true;
        lastInteractionTime = Time.time;
        popupCanvas.enabled = true;

        if (canvasGroup) { StopAllCoroutines(); canvasGroup.alpha = 1f; }

        // >>> PLAY SFX OPEN
        PlaySFX(sfxOpen);
    }

    public void Hide()
    {
        if (!popupCanvas) return;

        visible = false;
        lastInteractionTime = 0f;

        // >>> PLAY SFX CLOSE (sebelum Canvas dimatikan)
        PlaySFX(sfxClose);

        if (canvasGroup && fadeWhenHiding && fadeDuration > 0f)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutThenDisable());
        }
        else
        {
            popupCanvas.enabled = false;
            if (canvasGroup) canvasGroup.alpha = 0f;
        }
    }

    System.Collections.IEnumerator FadeOutThenDisable()
    {
        float t = 0f;
        float start = canvasGroup.alpha;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        popupCanvas.enabled = false;
    }

    // Panggil dari event UI (OnClick / OnPointerEnter / OnValueChanged) agar idle reset
    public void NotifyInteracted() => lastInteractionTime = Time.time;

    // ---------- AUDIO HELPER ----------
    void PlaySFX(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip, sfxVolume);
    }
}
