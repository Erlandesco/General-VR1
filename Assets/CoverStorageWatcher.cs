using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CoverStorageWatcher : MonoBehaviour
{
    [Header("Socket penyimpanan cover")]
    public XRSocketInteractor coverStorageSocket;

    [Header("State")]
    public bool coverStored = false;   // akan true ketika cover sudah disocket

    void OnEnable()
    {
        if (coverStorageSocket != null)
        {
            coverStorageSocket.selectEntered.AddListener(OnCoverStored);
            coverStorageSocket.selectExited.AddListener(OnCoverRemoved);
        }
    }

    void OnDisable()
    {
        if (coverStorageSocket != null)
        {
            coverStorageSocket.selectEntered.RemoveListener(OnCoverStored);
            coverStorageSocket.selectExited.RemoveListener(OnCoverRemoved);
        }
    }

    void OnCoverStored(SelectEnterEventArgs _)
    {
        coverStored = true;
        // Debug.Log("Cover disimpan di socket.");
    }

    void OnCoverRemoved(SelectExitEventArgs _)
    {
        coverStored = false;
        // kalau mau: kunci lagi O-Ring ketika cover diambil kembali
    }
}
