using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;           // untuk XROrigin
using UnityEngine.XR;

public class XRStartupFix : MonoBehaviour
{
    public XROrigin xrOrigin;
    public bool forceFloorOrigin = true;
    public bool recenterOnStart = true;

    void Awake()
    {
        if (!xrOrigin) xrOrigin = FindObjectOfType<XROrigin>();
    }

    void Start()
    {
        // 1) Paksa Tracking Origin = Floor
        if (forceFloorOrigin)
        {
            var subs = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subs);
            foreach (var s in subs)
            {
                if (s.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor))
                {
                    Debug.Log("[XRStartupFix] Tracking origin set to Floor.");
                }
                else
                {
                    Debug.LogWarning("[XRStartupFix] Failed to set Floor; using device origin.");
                }
            }
        }

        // 2) Recenter (arah & posisi kamera sinkron dengan guardian)
        if (recenterOnStart)
        {
            StartCoroutine(RecenterNextFrame());
        }

        // 3) Pastikan transform chain netral
        if (xrOrigin != null)
        {
            xrOrigin.transform.rotation = Quaternion.identity;
            xrOrigin.transform.localScale = Vector3.one;
        }
    }

    System.Collections.IEnumerator RecenterNextFrame()
    {
        yield return null; // tunggu satu frame sampai XR aktif
        var subs = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subs);
        foreach (var s in subs)
        {
            if (s.TryRecenter())
                Debug.Log("[XRStartupFix] Recentered.");
        }
    }
}