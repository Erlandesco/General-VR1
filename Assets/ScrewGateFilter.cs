using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ScrewGateFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
{
    [Header("Refs")]
    public TutorialScrewTracker tracker;              // drag tracker
    [SerializeField] XRGrabInteractable interactable; // drag XRGrabInteractable (cover)

    [Header("Options")]
    public bool blockHoverUntilReady = true;          // kalau true, hover juga diblok
    public float hapticAmplitude = 0.25f;
    public float hapticDuration = 0.10f;

    // Wajib untuk IXR*Filter
    public bool canProcess { get; set; } = true;

    void Reset()
    {
        TryGetComponent(out interactable);
    }

    void OnEnable()
    {
        if (!interactable) TryGetComponent(out interactable);
        if (!interactable) return;

        // daftar sebagai filter (tanpa Contains)
        interactable.selectFilters.Add(this);
        if (blockHoverUntilReady)
            interactable.hoverFilters.Add(this);
    }

    void OnDisable()
    {
        if (!interactable) return;

        // lepas dari filter
        interactable.selectFilters.Remove(this);
        interactable.hoverFilters.Remove(this);
    }

    // BLOK HOVER sebelum siap
    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable target)
    {
        if (!blockHoverUntilReady) return true;
        return tracker == null || tracker.AllScrewsRemoved();
    }

    // BLOK GRAB sebelum siap + warning + (opsional) haptic
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable target)
    {
        if (tracker == null || tracker.AllScrewsRemoved()) return true;

        //tracker.ShowWarning("Kamu belum melepas semua baut!");
        if (interactor is XRBaseInputInteractor c && c.xrController != null)
            c.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);

        return false; // tolak grab
    }
}
