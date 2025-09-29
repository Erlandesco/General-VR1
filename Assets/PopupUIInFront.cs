using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupUIInFront : MonoBehaviour
{
    [Header("References")]
    public Camera xrCamera;                       // drag Main Camera (XR)
    public Canvas popupCanvas;                    // Canvas (World Space)
    public CanvasGroup canvasGroup;               // untuk fade (opsional tapi disarankan)
    public InputActionReference toggleAction;     // binding ke tombol X (Left / North)

    [Header("Placement")]
    [Tooltip("Jarak dari user ke UI (meter).")]
    public float spawnDistance = 1.5f;
    [Tooltip("Offset tinggi dari posisi mata.")]
    public float heightOffset = -0.05f;
    [Tooltip("Clamp kemiringan supaya selalu sejajar lantai.")]
    public bool flattenForward = true;

    [Header("Auto Hide")]
    [Tooltip("UI akan hilang jika idle selama detik ini.")]
    public float autoHideDelay = 4.0f;
    public bool fadeWhenHiding = true;
    public float fadeDuration = 0.15f;

    bool visible;
    float lastInteractionTime;
    float fadeVel;

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

        // (Opsional) Jaga selalu menghadap user tanpa geser posisi
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

        if (canvasGroup)
        {
            StopAllCoroutines();
            canvasGroup.alpha = 1f; // cepat tampil; kalau mau fade-in, buat coroutine sendiri
        }
    }

    public void Hide()
    {
        if (!popupCanvas) return;

        visible = false;
        lastInteractionTime = 0f;

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

    // ==== Panggil ini dari event UI (OnClick / OnPointerEnter / OnValueChanged) ====
    public void NotifyInteracted()
    {
        lastInteractionTime = Time.time;
    }
}
