using UnityEngine;

public class HandPoseHint : MonoBehaviour
{
    [Tooltip("Centang untuk memaksa tangan berpose pinch saat hover/grab objek ini.")]
    public bool usePinch = true;

    [Tooltip("Kalau usePinch tidak diset, gunakan auto deteksi ukuran < threshold.")]
    public bool overrideAutoSize = false;
}
