using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class HandPoseDrive : MonoBehaviour
{
    [Header("Refs")]
    public XRDirectInteractor directInteractor; // drag interactor tangan
    public Animator handAnimator;               // drag animator tangan

    [Header("Animator Params")]
    public string pinchBoolParam = "UsePinch";  // bool di Animator untuk switch pose

    [Header("Auto Size Fallback")]
    public bool enableAutoSize = true;
    public float smallObjectMaxExtent = 0.03f; // ~3 cm: extents threshold

    // state
    IXRInteractable currentHover;
    IXRSelectInteractable currentSelect;

    void Reset()
    {
        if (!directInteractor) TryGetComponent(out directInteractor);
        if (!handAnimator) handAnimator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        if (!directInteractor) return;
        directInteractor.hoverEntered.AddListener(OnHoverEntered);
        directInteractor.hoverExited.AddListener(OnHoverExited);
        directInteractor.selectEntered.AddListener(OnSelectEntered);
        directInteractor.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        if (!directInteractor) return;
        directInteractor.hoverEntered.RemoveListener(OnHoverEntered);
        directInteractor.hoverExited.RemoveListener(OnHoverExited);
        directInteractor.selectEntered.RemoveListener(OnSelectEntered);
        directInteractor.selectExited.RemoveListener(OnSelectExited);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        currentHover = args.interactableObject;
        UpdatePose();
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (currentHover == args.interactableObject)
            currentHover = null;
        UpdatePose();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentSelect = args.interactableObject;
        UpdatePose();
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        if (currentSelect == args.interactableObject)
            currentSelect = null;
        UpdatePose();
    }

    void UpdatePose()
    {
        // Prioritas: yang sedang di-SELECT → kalau tidak ada, pakai HOVER → kalau tidak ada, normal
        var target = (IXRInteractable)currentSelect ?? currentHover;
        bool usePinch = ShouldUsePinch(target);
        if (handAnimator) handAnimator.SetBool(pinchBoolParam, usePinch);
    }

    bool ShouldUsePinch(IXRInteractable target)
    {
        if (target == null) return false;

        // Coba cari HandPoseHint
        if (target.transform is Transform t)
        {
            var hint = t.GetComponentInParent<HandPoseHint>();
            if (hint && hint.overrideAutoSize)
                return hint.usePinch;

            if (hint && hint.usePinch)
                return true;

            // Fallback: deteksi ukuran collider (jika diizinkan)
            if (enableAutoSize)
            {
                var cols = t.GetComponentsInParent<Collider>();
                if (cols != null && cols.Length > 0)
                {
                    // Ambil bounds gabungan
                    var b = cols[0].bounds;
                    foreach (var c in cols.Skip(1)) b.Encapsulate(c.bounds);
                    // Pakai extents paling besar sebagai perkiraan “besar objek”
                    float maxExtent = Mathf.Max(b.extents.x, Mathf.Max(b.extents.y, b.extents.z));
                    return maxExtent <= smallObjectMaxExtent;
                }
            }
        }
        return false;
    }
}
