using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TutorialScrewTracker : MonoBehaviour
{
    public static TutorialScrewTracker instance;

    public int totalScrews = 4;
    private int unscrewedCount = 0;
    public GameObject gloveCover;
    public GameObject helpStep9;
    public GloveHelpAnimation gloveHelp;

    void Awake()
    {
        instance = this;
        if (gloveCover != null) gloveCover.GetComponent<XRGrabInteractable>().enabled = false;
    }

    public void UnscrewOne()
    {
        unscrewedCount++;

        if (unscrewedCount >= totalScrews)
        {
            Debug.Log("All screws removed.");
            if (gloveCover != null) gloveCover.GetComponent<XRGrabInteractable>().enabled = true;
            if (helpStep9 != null) helpStep9.SetActive(true);
            gloveHelp.PlayHelpAnimation("Left_First");
           
        }
    }
}
