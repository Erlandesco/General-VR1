using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PinchGrabInteractable : XRGrabInteractable
{
    // Corrected method signature to match the base class
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // Kalau interactor adalah controller
        if (interactor is XRBaseInputInteractor controllerInteractor)
        {
            // Check apakah activateInput is performed
            if (controllerInteractor.activateInput?.ReadIsPerformed() ?? false)
                return base.IsSelectableBy(interactor);
            else
                return false;
        }

        return false;
    }

}
