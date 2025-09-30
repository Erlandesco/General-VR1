using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs; // Add this for ActionBasedController
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class UIRayFeedback : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Haptics (OpenXR)")]
    public XRNode hand = XRNode.RightHand;  // pilih RightHand / LeftHand
    [Range(0f, 1f)] public float hoverAmplitude = 0.12f;
    public float hoverDuration = 0.03f;
    [Range(0f, 1f)] public float clickAmplitude = 0.35f;
    public float clickDuration = 0.06f;

    // untuk menghindari double-hook
    private readonly HashSet<Button> _hookedButtons = new HashSet<Button>();

    void Awake()
    {
        // Auto siapkan AudioSource kalau belum ada
        if (!audioSource)
        {
            var cam = Camera.main;
            if (cam) audioSource = cam.GetComponent<AudioSource>() ?? cam.gameObject.AddComponent<AudioSource>();
            if (audioSource) { audioSource.playOnAwake = false; audioSource.spatialBlend = 0f; }
        }

        // Hook pertama kali + saat scene berganti
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        RefreshButtons();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _hookedButtons.Clear();
        RefreshButtons();
    }

    /// <summary>
    /// Panggil ini kapan saja (mis. setelah spawn UI) untuk hook button baru.
    /// </summary>
    public void RefreshButtons()
    {
        var buttons = GameObject.FindObjectsOfType<Button>(true);
        foreach (var btn in buttons)
        {
            if (btn == null || _hookedButtons.Contains(btn)) continue;

            // CLICK feedback
            btn.onClick.AddListener(OnAnyButtonClick);

            // HOVER feedback via EventTrigger
            var trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (!trigger) trigger = btn.gameObject.AddComponent<EventTrigger>();

            AddOrReplaceEntry(trigger, EventTriggerType.PointerEnter, OnAnyButtonHover);

            _hookedButtons.Add(btn);
        }
#if UNITY_EDITOR
        Debug.Log($"[UIButtonGlobalFeedback] Hooked {_hookedButtons.Count} button(s).");
#endif
    }

    // ------- Event handlers -------
    private void OnAnyButtonClick()
    {
        if (clickClip && audioSource) audioSource.PlayOneShot(clickClip, volume);
        Pulse(hand, clickAmplitude, clickDuration);
    }

    private void OnAnyButtonHover(BaseEventData _)
    {
        if (hoverClip && audioSource) audioSource.PlayOneShot(hoverClip, volume);
        Pulse(hand, hoverAmplitude, hoverDuration);
    }

    // ------- Helpers -------
    private static void AddOrReplaceEntry(EventTrigger trigger, EventTriggerType type, System.Action<BaseEventData> cb)
    {
        // hapus entry lama tipe yang sama untuk menghindari duplikasi
        if (trigger.triggers == null) trigger.triggers = new List<EventTrigger.Entry>();
        trigger.triggers.RemoveAll(e => e != null && e.eventID == type);

        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(data => cb(data));
        trigger.triggers.Add(entry);
    }

    private static void Pulse(XRNode node, float amp, float dur)
    {
        if (amp <= 0f || dur <= 0f) return;
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(node, devices);
        foreach (var dev in devices)
        {
            if (!dev.isValid) continue;
            if (dev.TryGetHapticCapabilities(out var caps) && caps.supportsImpulse)
                dev.SendHapticImpulse(0u, Mathf.Clamp01(amp), dur);
        }
    }
}
