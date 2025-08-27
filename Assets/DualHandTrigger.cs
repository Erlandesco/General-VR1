using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DualHandTrigger : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string openTriggerName = "Open";
    public string closeTriggerName = "Close";

    [Header("Tags")]
    public string rightHandTag = "RightHand";
    public string leftHandTag = "LeftHand";

    [Header("Glove")]
    public GameObject gloveOpen;
    public GameObject gloveClose;

    // state sensor
    bool rightInside = false;
    bool leftInside = false;

    // state pintu
    bool isOpen = false;

    // edge/latch
    bool bothInsideLastFrame = false; // untuk deteksi rising-edge
    bool armed = true;                // toggle hanya boleh saat sudah "armed"

    // dipanggil dari HandTrigger (sensor kanan/kiri)
    public void RightHandEnter() { rightInside = true; TryUpdate(); }
    public void RightHandExit() { rightInside = false; TryUpdate(); }
    public void LeftHandEnter() { leftInside = true; TryUpdate(); }
    public void LeftHandExit() { leftInside = false; TryUpdate(); }

    void TryUpdate()
    {
        bool bothInsideNow = rightInside && leftInside;

        // Detect "rising edge": sebelumnya tidak lengkap -> sekarang lengkap
        if (!bothInsideLastFrame && bothInsideNow)
        {
            if (armed)
            {
                ToggleDoor();
                armed = false; // jangan toggle lagi selama masih lengkap
            }
        }

        // Re-arm ketika tidak lengkap (salah satu keluar)
        if (!bothInsideNow)
            armed = true;

        bothInsideLastFrame = bothInsideNow;
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            //animator.ResetTrigger(closeTriggerName);
            animator.Play(openTriggerName);
            gloveOpen.SetActive(true);
            gloveClose.SetActive(false);
            Debug.Log("TOGGLE: OPEN");
        }
        else
        {
            //animator.ResetTrigger(openTriggerName);
            animator.Play(closeTriggerName);
            gloveOpen.SetActive(false);
            gloveClose.SetActive(true);
            Debug.Log("TOGGLE: CLOSE");
        }
    }
}
