using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.OpenXR;
using UnityEngine.InputSystem.Haptics;


[RequireComponent(typeof(XRGrabInteractable))]
public class PinchOnlyGrabGate : MonoBehaviour
{
    XRGrabInteractable interactable;

    [Header("Options")]
    public bool allowHandTrackingSelect = true; // kalau pakai hand-tracking pinch (Select), biarkan true
    public float hapticAmp = 0.25f;
    public float hapticDur = 0.08f;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Controller interactor?
        if (args.interactorObject is XRBaseInputInteractor ctrl) // Updated from XRBaseControllerInteractor to XRBaseInputInteractor
        {
            // Use the correct property for checking activation state
            bool activatePressed = ctrl.shouldActivate;

            // (opsional) izinkan hand-tracking yang map ke Select (bukan Activate)
            bool selectPressed = ctrl.isSelectActive; // Updated to use isSelectActive

            bool allowed = activatePressed || (allowHandTrackingSelect && selectPressed);

            if (!allowed)
            {
                // haptic kecil sebagai feedback
                if (ctrl is IXRHapticImpulseProvider hapticProvider)
                {
                    // Corrected to use the IXRHapticImpulseChannel interface
                    var channelGroup = hapticProvider.GetChannelGroup();
                    if (channelGroup != null && channelGroup.channelCount > 0)
                    {
                        var channel = channelGroup.GetChannel(0); // Assuming channel 0 is used
                        if (channel != null)
                        {
                            channel.SendHapticImpulse(hapticAmp, hapticDur);
                        }
                    }
                }

                // batalkan grab segera
                if (args.manager != null) // Use the manager property from SelectEnterEventArgs
                    args.manager.SelectExit(args.interactorObject, interactable);
            }
        }
        else
        {
            // Interactor bukan controller (mis. interactor kustom) → tolak untuk aman
            if (args.manager != null) // Use the manager property from SelectEnterEventArgs
                args.manager.SelectExit(args.interactorObject, interactable);
        }
    }
}
