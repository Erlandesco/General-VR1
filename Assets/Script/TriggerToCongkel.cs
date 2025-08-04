using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction;
using UnityEngine.InputSystem;

public class TriggerToCongkel : MonoBehaviour
{
    public GameObject glovesHelp;     // UI bantuan/tutorial
    public GameObject rubberObject;   // Objek yang akan diaktifkan
    public InputActionProperty  triggerAction; // Input dari XR controller
    public bool isPlayerInZone = false;

    private bool isDone = false;

    void Update()
    {
        if (isPlayerInZone && !isDone && triggerAction.action.WasPressedThisFrame())
        {
            // Disable petunjuk
            if (glovesHelp != null) glovesHelp.SetActive(false);

            // Aktifkan objek karet
            if (rubberObject != null) rubberObject.SetActive(true);

            isDone = true;

            Debug.Log("Trigger ditekan, karet aktif.");
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
        }
    }
}
