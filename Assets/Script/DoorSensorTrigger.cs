// DoorSensorTrigger.cs
using UnityEngine;
using UnityEngine.Events; // Untuk event UnityEvent

public class DoorSensorTrigger : MonoBehaviour
{
    // Event yang akan dipanggil ketika pintu memasuki/meninggalkan trigger ini
    public UnityEvent OnDoorEntered;
    public UnityEvent OnDoorExited;

    // Untuk melacak apakah pintu saat ini ada di dalam trigger
    public bool IsDoorInside { get; private set; } = false;

    // Pastikan collider ini di set Is Trigger = true
    private void OnTriggerEnter(Collider other)
    {
        // Pastikan GameObject yang masuk adalah pintu kita dengan tag "DoorSensor"
        if (other.CompareTag("DoorSensor"))
        {
            IsDoorInside = true;
            OnDoorEntered?.Invoke();
            Debug.Log($"{gameObject.name}: Door Entered.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DoorSensor"))
        {
            IsDoorInside = false;
            OnDoorExited?.Invoke();
            Debug.Log($"{gameObject.name}: Door Exited.");
        }
    }
}