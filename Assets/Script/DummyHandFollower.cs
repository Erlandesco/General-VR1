using UnityEngine;

public class DummyHandFollower : MonoBehaviour
{
    public Transform targetController; // Controller kanan/kiri (XR controller pose)
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    void LateUpdate()
    {
        if (targetController == null) return;

        // Ikuti posisi + offset
        transform.position = targetController.position + targetController.TransformVector(positionOffset);

        // Ikuti rotasi + offset
        transform.rotation = targetController.rotation * Quaternion.Euler(rotationOffset);
    }
}
