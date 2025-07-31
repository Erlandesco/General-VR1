using System.Collections;
using UnityEngine;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform target;
    public Renderer nonPhysicalHand;
    public float showNonPhysicalHandDistance = 0.5f;
    public float smoothingFactor = 0.2f;
    public float maxVelocity = 10f;

    private Rigidbody rb;
    private Collider[] handColliders;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        handColliders = GetComponentsInChildren<Collider>();
    }

    public void EnableHandCollider(float delay)
    {
        StartCoroutine(EnableHandColliderDelay(delay));
    }

    private IEnumerator EnableHandColliderDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var item in handColliders)
        {
            item.enabled = true;
        }
    }

    public void DisableHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        nonPhysicalHand.enabled = distance > showNonPhysicalHandDistance;
    }

    void FixedUpdate()
    {
        // --- Smoothed position tracking ---
        Vector3 desiredVelocity = (target.position - transform.position) / Time.fixedDeltaTime;
        Vector3 smoothedVelocity = Vector3.Lerp(rb.velocity, desiredVelocity, smoothingFactor);
        rb.velocity = Vector3.ClampMagnitude(smoothedVelocity, maxVelocity);

        // --- Smoothed rotation tracking ---
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        if (angleInDegrees > 180f)
            angleInDegrees -= 360f;

        Vector3 angularVelocity = (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.fixedDeltaTime;
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, angularVelocity, smoothingFactor);
    }
}
