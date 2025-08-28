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
    [SerializeField] public bool isLowerDownGloves = false;
    public InputActionReference trigerAction;

    void Update()
    {
        if(isPlayerInZone == true && trigerAction.action.IsPressed())
        {
            isLowerDownGloves = true;
            Debug.Log("Old Gloves Has Been Lower Down ");
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
