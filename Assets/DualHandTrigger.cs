using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualHandTrigger : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator; // drag animator yg punya animasi
    public string animationTriggerName = "PlayAnim"; // nama trigger di animator

    [Header("Tags")]
    public string rightHandTag = "RightHand";
    public string leftHandTag = "LeftHand";

    private bool rightInside = false;
    private bool leftInside = false;

    // trigger kanan
    public void RightHandEnter()
    {
        rightInside = true;
        CheckBothHands();
    }

    public void RightHandExit()
    {
        rightInside = false;
    }

    // trigger kiri
    public void LeftHandEnter()
    {
        leftInside = true;
        CheckBothHands();
    }

    public void LeftHandExit()
    {
        leftInside = false;
    }

    void CheckBothHands()
    {
        if (rightInside && leftInside)
        {
            Debug.Log("Kedua tangan masuk, mainkan animasi");
            animator.Play(animationTriggerName);
        }
    }
}
