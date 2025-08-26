 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrigger : MonoBehaviour
{
    public DualHandTrigger dualTrigger;
    public bool isRight; // centang kalau trigger ini buat tangan kanan

    void OnTriggerEnter(Collider other)
    {
        if (isRight && other.CompareTag("RightHand"))
            dualTrigger.RightHandEnter();
        else if (!isRight && other.CompareTag("LeftHand"))
            dualTrigger.LeftHandEnter();
    }

    void OnTriggerExit(Collider other)
    {
        if (isRight && other.CompareTag("RightHand"))
            dualTrigger.RightHandExit();
        else if (!isRight && other.CompareTag("LeftHand"))
            dualTrigger.LeftHandExit();
    }
}