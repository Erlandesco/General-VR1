using UnityEngine;
using UnityEngine.UI; // Penting untuk mengakses komponen UI
using UnityEditor; // Penting untuk mengakses EditorGUI dan Editor

public class UIDebugButton : MonoBehaviour
{
    // Referensi ke tombol di Canvas Worldspace Anda
    public Button targetButton;

    // Fungsi yang akan dipanggil ketika tombol di Inspector diklik
    public void SimulateButtonClick()
    {
        if (targetButton != null)
        {
            // Memanggil event onClick dari tombol target
            targetButton.onClick.Invoke();
            Debug.Log($"Tombol '{targetButton.name}' di UI Worldspace telah diklik dari Inspector.");
        }
        else
        {
            Debug.LogWarning("Target Button belum diatur di UIDebugButton.");
        }
    }
}

// Editor kustom untuk menambahkan tombol di Inspector
#if UNITY_EDITOR
[CustomEditor(typeof(UIDebugButton))]
public class UIDebugButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Gambar Inspector default (untuk referensi targetButton)
        DrawDefaultInspector();

        UIDebugButton myScript = (UIDebugButton)target;

        // Tambahkan tombol kustom di Inspector
        if (GUILayout.Button("Klik Tombol UI"))
        {
            myScript.SimulateButtonClick();
        }
    }
}
#endif