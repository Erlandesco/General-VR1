using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class HapticOnScrewPlaced : MonoBehaviour
{
    private XRSocketInteractor socket;

    public float hapticAmplitude = 0.5f;
    public float hapticDuration = 0.2f;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnScrewInserted);
    }

    void OnScrewInserted(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject as XRBaseInputInteractor;

        if (interactor != null && interactor.xrController != null)
        {
            interactor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);
        }
    }
}
