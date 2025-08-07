using UnityEngine;

public class SwitchRotator : MonoBehaviour
{
    public float onThreshold = 30f;   // Rotasi X minimum untuk dianggap ON
    public float offThreshold = 10f;  // Rotasi X maksimum untuk dianggap OFF
    private bool isOn = false;

    void Update()
    {
        float xRotation = transform.localEulerAngles.x;

        // Menyesuaikan nilai rotasi agar bisa deteksi antara 0-180 (hindari 360 wrap)
        if (xRotation > 180) xRotation -= 360;

        if (!isOn && xRotation > onThreshold)
        {
            isOn = true;
            Debug.Log("Switch ON");
            // Tambahkan logika saat ON di sini
        }
        else if (isOn && xRotation < offThreshold)
        {
            isOn = false;
            Debug.Log("Switch OFF");
            // Tambahkan logika saat OFF di sini
        }
    }
}
