using UnityEngine;
using UnityEngine.Events;

public class SwitchRotator : MonoBehaviour
{
    // Rotasi untuk posisi "OFF"
    public Vector3 offRotation = new Vector3(0, 0, 0);

    // Rotasi untuk posisi "ON"
    public Vector3 onRotation = new Vector3(0, 90, 0);

    // Kecepatan putaran
    public float rotationSpeed = 5f;

    // Status sakelar saat ini
    private bool isOn = false;

    // Rotasi target
    private Quaternion targetRotation;

    void Start()
    {
        // Atur rotasi awal sesuai dengan posisi OFF
        targetRotation = Quaternion.Euler(offRotation);
        transform.localRotation = targetRotation;
    }

    void Update()
    {
        // Gerakkan rotasi objek secara bertahap menuju target
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Fungsi ini dipanggil dari controller VR untuk memutar sakelar
    public void ToggleSwitch()
    {
        isOn = !isOn; // Balik status on/off

        // Jika ON, atur rotasi target ke posisi ON
        if (isOn)
        {
            targetRotation = Quaternion.Euler(onRotation);
            Debug.Log("Sakelar ON"); // Anda bisa ganti dengan kode untuk menyalakan mesin
        }
        // Jika OFF, atur rotasi target ke posisi OFF
        else
        {
            targetRotation = Quaternion.Euler(offRotation);
            Debug.Log("Sakelar OFF"); // Anda bisa ganti dengan kode untuk mematikan mesin
        }
    }
}