using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class DualHandLiftDoor_Multiple : MonoBehaviour
{

    public float minAngle = 0f;   // tertutup
    public float maxAngle = 75f;  // terbuka maksimum
    public float lockedSlack = 2f; // toleransi kecil saat terkunci

    HingeJoint joint;
    XRGrabInteractable xri;
    Rigidbody rb;

    void Awake()
    {
        joint = GetComponent<HingeJoint>();
        xri = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Pastikan awal terkunci (limit nyaris nol)
        LockDoor();
        // Pastikan rigidbody non-kinematic untuk joint
        rb.isKinematic = false;

        xri.selectEntered.AddListener(_ => OnGrabChanged());
        xri.selectExited.AddListener(_ => OnGrabChanged());
    }

    void OnDestroy()
    {
        xri.selectEntered.RemoveAllListeners();
        xri.selectExited.RemoveAllListeners();
    }

    void OnGrabChanged()
    {
        if (xri.interactorsSelecting.Count >= 2)
            UnlockDoor();
        else
            LockDoor();
    }

    void LockDoor()
    {
        var lim = joint.limits;
        // kunci di sekitar sudut saat ini supaya nggak nyentak
        float current = joint.angle; // ini di space joint (bisa negatif/positif)
        lim.min = current - lockedSlack;
        lim.max = current + lockedSlack;
        joint.limits = lim;
        joint.useLimits = true;

        // Redam gerak
        rb.angularVelocity = Vector3.zero;
    }

    void UnlockDoor()
    {
        var lim = joint.limits;
        lim.min = minAngle;
        lim.max = maxAngle;
        joint.limits = lim;
        joint.useLimits = true;
    }
}
