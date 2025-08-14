using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class TrigerToLowerDownGloves : MonoBehaviour
{
    [SerializeField] private bool isPlayerInZone = false;
    [SerializeField] private bool isLowerDownGloves = false;
    public InputActionReference trigerAction;
    public GameObject step10HelpUI;
    public GameObject step11HelpUI;
    public GameObject triggerSelfObject;
    public XRGrabInteractable grabNewGloves;

    void Update()
    {
        if(isPlayerInZone == true && trigerAction.action.IsPressed())
        {
            isLowerDownGloves = true;
            Debug.Log("Old Gloves Has Been Lower Down ");
            grabNewGloves.enabled = true;
            step10HelpUI.SetActive(false);
            step11HelpUI.SetActive(true);
            triggerSelfObject.SetActive(true);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            isPlayerInZone = true;
            Debug.Log("Player masuk area interaksi glove.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            isPlayerInZone = false;
            Debug.Log("Player keluar area interaksi glove.");
        }
    }


}
