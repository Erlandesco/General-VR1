// XR Origin Height Normalizer – XRI 3.x
// - Coba paksa Floor. Jika gagal, normalisasi ke targetHeight.
// - Tambah public method CalibrateNow() untuk dipanggil dari tombol/aksi input.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils; // for XROrigin

[RequireComponent(typeof(XROrigin))]
public class XROriginHeightNormalizer : MonoBehaviour
{
    public float targetHeight = 1.70f;   // tinggi desain (meter)
    public bool autoCalibrateOnStart = true;

    XROrigin xrOrigin;
    Transform floorOffset; // Camera Floor Offset Object
    List<XRInputSubsystem> subsystems = new();

    void Awake()
    {
        xrOrigin = GetComponent<XROrigin>();
        floorOffset = xrOrigin.CameraFloorOffsetObject != null
            ? xrOrigin.CameraFloorOffsetObject.transform
            : xrOrigin.transform;
        SubsystemManager.GetInstances(subsystems);
    }

    void Start()
    {
        if (TrySetFloor())
            return;

        if (autoCalibrateOnStart)
            CalibrateNow();
    }

    bool TrySetFloor()
    {
        bool anySet = false;
        foreach (var s in subsystems)
        {
            if (!s.running) continue;
            if (s.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor))
                anySet = true;
        }
        if (anySet)
        {
            // pastikan offset 0 bila di Floor
            var lp = floorOffset.localPosition;
            floorOffset.localPosition = new Vector3(lp.x, 0f, lp.z);
        }
        return anySet;
    }

    // Panggil ini dari tombol "Calibrate" (mis. menu button)
    public void CalibrateNow()
    {
        // posisi HMD relatif rig
        var cam = xrOrigin.Camera;
        if (cam == null) return;

        // di mode Device, cam.localPosition.y ≈ tinggi kepala user
        float hmdY = cam.transform.localPosition.y;
        float neededOffsetY = targetHeight - hmdY;

        var lp = floorOffset.localPosition;
        floorOffset.localPosition = new Vector3(lp.x, neededOffsetY, lp.z);
    }
}
