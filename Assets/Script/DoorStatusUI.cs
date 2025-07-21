using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorStatusUI : MonoBehaviour
{
    [Header("UI Elements")]  
    [Tooltip("Drag the UI Image/GameObject for the 'X' (door open) here.")]
    [SerializeField] private GameObject xMarkUI; // GameObject untuk tanda silang
    [Tooltip("Drag the UI Image/GameObject for the 'Checkmark' (door closed) here.")]
    [SerializeField] private GameObject checkMarkUI; // GameObject untuk tanda centang

    [Header("Door Reference")]
    [Tooltip("Drag the GameObject representing the door here. It should have a script with an 'IsDoorClosed' property.")]
    [SerializeField] private DoorController doorController; // Referensi ke script kontrol pintu Anda

    private void Start()
    {
        // Pastikan UI elements tidak null
        if (xMarkUI == null || checkMarkUI == null)
        {
            Debug.LogError("X Mark UI or Check Mark UI is not assigned in the Inspector!", this);
            enabled = false; // Menonaktifkan script jika UI tidak diatur
            return;
        }

        // Pastikan referensi pintu tidak null
        if (doorController == null)
        {
            Debug.LogError("Door Controller is not assigned in the Inspector! Please assign the door GameObject with its script.", this);
            enabled = false; // Menonaktifkan script jika pintu tidak diatur
            return;
        }

        // Setel status awal UI berdasarkan status pintu saat Start
        UpdateUI(doorController.IsDoorClosed);
    }

    private void Update()
    {
        // Perbarui UI setiap frame berdasarkan status pintu
        // Anda bisa memodifikasi ini agar hanya diperbarui saat status pintu berubah
        UpdateUI(doorController.IsDoorClosed);
    }

    /// <summary>
    /// Memperbarui tampilan UI berdasarkan apakah pintu tertutup atau tidak.
    /// </summary>
    /// <param name="isClosed">True jika pintu tertutup, False jika terbuka.</param>
    private void UpdateUI(bool isClosed)
    {
        if (isClosed)
        {
            // Pintu tertutup: tampilkan centang, sembunyikan silang
            checkMarkUI.SetActive(true);
            xMarkUI.SetActive(false);
        }
        else
        {
            // Pintu terbuka: tampilkan silang, sembunyikan centang
            checkMarkUI.SetActive(false);
            xMarkUI.SetActive(true);
        }
    }
}