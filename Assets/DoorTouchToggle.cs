using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTouchToggle : MonoBehaviour
{
    [Header("Door")]
    public Transform door;                 // drag daun pintu (pivot di engsel)
    public Vector3 localAxis = Vector3.up; // sumbu rotasi lokal pintu (Y=vertikal utk pintu biasa)
    [Tooltip("Sudut tertutup relatif base rot (derajat)")]
    public float closedAngle = 0f;
    [Tooltip("Sudut terbuka relatif base rot (derajat)")]
    public float openAngle = 90f;

    [Header("Anim")]
    public float duration = 0.4f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Detection")]
    public string rightHandTag = "RightHand";
    public string leftHandTag = "LeftHand";

    // state
    private Quaternion baseLocalRot;    // referensi pose dasar
    private bool isOpen = false;        // pintu sedang terbuka?
    private bool armed = true;          // harus keluar dulu sebelum boleh toggle lagi
    private Coroutine rotCo;

    void Awake()
    {
        if (door == null) door = transform; // fallback
        baseLocalRot = door.localRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!armed) return;

        if (other.CompareTag(rightHandTag) || other.CompareTag(leftHandTag))
        {
            armed = false;          // lock sampai tangan keluar
            ToggleDoor();           // jalankan toggle
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(rightHandTag) || other.CompareTag(leftHandTag))
        {
            armed = true;           // re-arm: boleh toggle lagi
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        Quaternion target = GetTargetRotation(isOpen);

        if (rotCo != null) StopCoroutine(rotCo);
        rotCo = StartCoroutine(RotateTo(target));
    }

    Quaternion GetTargetRotation(bool open)
    {
        float angle = open ? openAngle : closedAngle;
        // rotasi relatif terhadap baseLocalRot di sumbu lokal yang dipilih
        Quaternion delta = Quaternion.AngleAxis(angle, localAxis.normalized);
        return baseLocalRot * delta;
    }

    IEnumerator RotateTo(Quaternion target)
    {
        Quaternion from = door.localRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            float k = ease.Evaluate(Mathf.Clamp01(t));
            door.localRotation = Quaternion.Slerp(from, target, k);
            yield return null;
        }

        door.localRotation = target;
        rotCo = null;
    }

    // Opsional: fungsi publik kalau mau trigger via script lain
    public void ForceOpen() { if (!isOpen) ToggleDoor(); }
    public void ForceClose() { if (isOpen) ToggleDoor(); }
}
