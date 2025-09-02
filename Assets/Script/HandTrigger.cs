using UnityEngine;

public class HandTrigger : MonoBehaviour
{
    public DualHandTrigger dualTrigger;
    public bool isRight; // centang untuk sensor kanan

    int overlapCount = 0; // banyaknya collider dari tangan yang sedang overlap
    bool reportedInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (isRight && other.CompareTag("RightHand")) Enter();
        else if (!isRight && other.CompareTag("LeftHand")) Enter();
    }

    void OnTriggerExit(Collider other)
    {
        if (isRight && other.CompareTag("RightHand")) Exit();
        else if (!isRight && other.CompareTag("LeftHand")) Exit();
    }

    void Enter()
    {
        overlapCount++;
        if (!reportedInside)
        {
            reportedInside = true;
            if (isRight) dualTrigger.RightHandEnter();
            else dualTrigger.LeftHandEnter();
        }
    }

    void Exit()
    {
        overlapCount = Mathf.Max(0, overlapCount - 1);
        if (reportedInside && overlapCount == 0)
        {
            reportedInside = false;
            if (isRight) dualTrigger.RightHandExit();
            else dualTrigger.LeftHandExit();
        }
    }
}
