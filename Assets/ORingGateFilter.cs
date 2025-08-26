using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ORingGateFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
{
    [Header("Refs")]
    public CoverStorageWatcher coverWatcher;            // drag watcher dari scene
    [SerializeField] XRGrabInteractable interactable;   // drag XRGrabInteractable (O-Ring)

    [Header("Options")]
    public bool blockHoverUntilReady = true;
    public float hapticAmplitude = 0.25f;
    public float hapticDuration = 0.10f;

    // Wajib untuk IXR*Filter
    public bool canProcess { get; set; } = true;

    void Reset() => TryGetComponent(out interactable);

    void OnEnable()
    {
        if (!interactable) TryGetComponent(out interactable);
        if (!interactable) return;

        interactable.selectFilters.Add(this);
        if (blockHoverUntilReady) interactable.hoverFilters.Add(this);
    }

    void OnDisable()
    {
        if (!interactable) return;
        interactable.selectFilters.Remove(this);
        interactable.hoverFilters.Remove(this);
    }

    // HOVER: blokir sampai cover tersimpan
    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable target)
    {
        if (!blockHoverUntilReady) return true;
        return coverWatcher == null || coverWatcher.coverStored;
    }

    // SELECT: tolak grab + warning + (opsional) haptic
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable target)
    {
        bool ready = coverWatcher == null || coverWatcher.coverStored;
        if (ready) return true;

        // tampilkan warning kalau kamu punya sistem warning
        // contoh (opsional):
        // TutorialScrewTracker.instance?.ShowWarning("Simpan cover di socket dulu!");

        if (interactor is XRBaseInputInteractor c && c.xrController != null)
            c.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);

        return false; // tolak grab O-Ring
    }
}
