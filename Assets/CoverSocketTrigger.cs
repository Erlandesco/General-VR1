using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CoverSocketTrigger : MonoBehaviour
{
    public XRSocketInteractor socket;
    public GameObject step9HelpUI;
    public GameObject gloveCoverObject; // opsional, untuk disable
    public XRGrabInteractable karetGlove;

    private bool hasTriggered = false;

    private void Start()
    {
        karetGlove.enabled = false;
    }

    void Update()
    {
        if (hasTriggered) return;

        if (socket.hasSelection)
        {
            hasTriggered = true;

            Debug.Log("Cover placed on socket. Step 9 starts.");
            if (step9HelpUI != null) step9HelpUI.SetActive(true);

            // Optional: lock object, disable grab
            var interactable = socket.GetOldestInteractableSelected();
            if (interactable != null)
                //interactable.transform.GetComponent<XRGrabInteractable>().enabled = false;
                karetGlove.enabled = true;
                step9HelpUI.GetComponent<GloveHelpAnimation>().PlayHelpAnimation("Left Pinch");

        }
    }
}
