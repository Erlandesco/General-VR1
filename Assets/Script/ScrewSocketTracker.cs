using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ScrewSocketTracker : MonoBehaviour
{
    public XRSocketInteractor[] screwSockets; // isi dengan 4 socket
    public GameObject onAllScrewsPlacedObject; // opsional: misalnya UI/help/audio

    private bool isTriggered = false;

    void Update()
    {
        if (isTriggered) return;

        int placed = 0;
        foreach (var socket in screwSockets)
        {
            if (socket.hasSelection)
                placed++;
        }

        if (placed == screwSockets.Length)
        {
            isTriggered = true;

            Debug.Log("✅ Semua baut telah dipasang!");

            if (onAllScrewsPlacedObject != null)
                onAllScrewsPlacedObject.SetActive(true);

            // Bisa juga: panggil fungsi, play audio, animasi, dll
        }
    }
}
