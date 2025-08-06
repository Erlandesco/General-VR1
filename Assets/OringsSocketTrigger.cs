using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;

public class OringsSocketTrigger : MonoBehaviour
{
    public XRSocketInteractor socketOrings;
    public GameObject step10HelpUI;
    //public GameObject gloveCoverObject; // opsional, untuk disable
    //public XRGrabInteractable karetGlove;
    public CoverSocketTrigger CoverSocketTrigger;
    public TrigerToLowerDownGloves TrigerToLowerDownGloves;
    private bool hasTriggered = false;


    void Update()
    {
        if (hasTriggered) return;

        if (socketOrings.hasSelection)
        {
            hasTriggered = true;

            Debug.Log("O Rings placed on socket. Step 10 starts.");
            if (step10HelpUI != null)
            {
                step10HelpUI.SetActive(true);
                CoverSocketTrigger.step9HelpUI.SetActive(false);
                TrigerToLowerDownGloves.enabled = true;
             }

            // Optional: lock object, disable grab
            var interactable = socketOrings.GetOldestInteractableSelected();
            if (interactable != null)
                //interactable.transform.GetComponent<XRGrabInteractable>().enabled = false;
                step10HelpUI.GetComponent<GloveHelpAnimation>().PlayHelpAnimation("Left Pinch");
           

        }
    }
}
