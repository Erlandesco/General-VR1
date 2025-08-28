using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Transform playerHead;       // Biasanya ini Camera Offset/Center Eye
    public CapsuleCollider bodyCollider;
    public Transform rigOrigin;        // Root XR Origin / Camera Offset (referensi untuk posisi relatif)

    public float bodyHeightMin = 0.5f;
    public float bodyHeightMax = 2f;
    public float skinBuffer = 0.1f;    // Supaya collider gak mentok ke bawah

    void FixedUpdate()
    {
        Vector3 headLocalPos = rigOrigin.InverseTransformPoint(playerHead.position);
        float headHeight = Mathf.Clamp(headLocalPos.y, bodyHeightMin, bodyHeightMax);

        bodyCollider.height = headHeight;
        bodyCollider.center = new Vector3(headLocalPos.x, headHeight / 2 + skinBuffer, headLocalPos.z);
    }
}
