using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private bool _isDoorClosed = false;

    // Properti publik untuk mendapatkan status pintu
    public bool IsDoorClosed
    {
        get { return _isDoorClosed; }
        private set { _isDoorClosed = value; } // Set bisa dari internal atau script lain yang mengontrol pintu
    }

    // Metode contoh untuk membuka/menutup pintu (Anda akan menggantinya dengan logika pintu Anda)
    public void OpenDoor()
    {
        Debug.Log("Door is now open!");
        IsDoorClosed = false;
        // Tambahkan logika animasi pintu terbuka di sini
    }

    public void CloseDoor()
    {
        Debug.Log("Door is now closed!");
        IsDoorClosed = true;
        // Tambahkan logika animasi pintu tertutup di sini
    }

    // Contoh penggunaan (opsional, untuk pengujian)
    void Update()
    {
        // Tekan 'O' untuk membuka, 'C' untuk menutup (hanya untuk testing)
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenDoor();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CloseDoor();
        }
    }
}