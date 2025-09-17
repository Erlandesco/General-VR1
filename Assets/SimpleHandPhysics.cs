using UnityEngine;

public class SimpleHandPhysics : MonoBehaviour
{
    public float pushMultiplier = 1.0f;
    public float torqueMultiplier = 0.5f;

    Vector3 prevPos;
    Quaternion prevRot;
    Vector3 vel;
    Vector3 angVel;

    void Start()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // linear velocity
        Vector3 pos = transform.position;
        vel = (pos - prevPos) / dt;

        // angular velocity (aproksimasi dari delta quaternion)
        Quaternion dq = transform.rotation * Quaternion.Inverse(prevRot);
        dq.ToAngleAxis(out float angleDeg, out Vector3 axis);
        if (angleDeg > 180f) angleDeg -= 360f;
        if (float.IsNaN(axis.x)) { axis = Vector3.zero; angleDeg = 0f; }
        angVel = axis * (angleDeg * Mathf.Deg2Rad) / dt;

        prevPos = pos;
        prevRot = transform.rotation;
    }

    void OnCollisionStay(Collision c)
    {
        var rb = c.rigidbody;
        if (!rb || rb.isKinematic) return;

        var p = c.GetContact(0).point;
        rb.AddForceAtPosition(vel * pushMultiplier, p, ForceMode.VelocityChange);
        rb.AddTorque(angVel * torqueMultiplier, ForceMode.VelocityChange);
    }
}
