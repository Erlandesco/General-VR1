using System;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;

    [Header("Follow/Anchor Options")]
    public Transform keyboardAnchor;          // Optional: biar keyboard nempel ke transform ini
    public bool useFixedPose = false;         // Kalau true, pakai posisi/rotasi tetap di bawah
    public Vector3 keyboardWorldPosition;     // Posisi absolut (world)
    public Vector3 keyboardWorldEuler;        // Rotasi absolut (world, dalam derajat)

    [Header("Legacy offset mode (opsional)")]
    public float distance = 0.5f;
    public float verticalOffset = -0.5f;
    public Transform positionSource;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(_ => OpenKeyboard());
    }

    public void OpenKeyboard()
    {
        var kb = NonNativeKeyboard.Instance;
        kb.InputField = inputField;
        kb.PresentKeyboard(inputField.text);

        // --- PILIH SATU MODE DI BAWAH INI ---

        if (keyboardAnchor != null)
        {
            // Mode A: ikuti Transform anchor
            SetKeyboardPose(kb, keyboardAnchor.position, keyboardAnchor.rotation);
        }
        else if (useFixedPose)
        {
            // Mode B: pakai posisi/rotasi absolut
            SetKeyboardPose(kb, keyboardWorldPosition, Quaternion.Euler(keyboardWorldEuler));
        }
        else if (positionSource != null)
        {
            // Mode C: pakai perhitungan arah seperti sebelumnya
            Vector3 dir = positionSource.forward;
            dir.y = 0f;
            dir.Normalize();
            Vector3 targetPos = positionSource.position + dir * distance + Vector3.up * verticalOffset;

            // Kalau kamu ingin tetap pakai API bawaan:
            // kb.RepositionKeyboard(targetPos);

            // Atau langsung set transform:
            SetKeyboardPose(kb, targetPos, Quaternion.LookRotation(dir, Vector3.up));
        }

        SetCaretColorAlpha(1f);
        kb.OnClosed += Instance_OnClosed;
    }

    private void SetKeyboardPose(NonNativeKeyboard kb, Vector3 pos, Quaternion rot)
    {
        // Banyak versi MRTK punya RepositionKeyboard(Vector3),
        // tapi set transform langsung juga aman.
        kb.transform.SetPositionAndRotation(pos, rot);
    }

    private void Instance_OnClosed(object sender, EventArgs e)
    {
        SetCaretColorAlpha(0f);
        NonNativeKeyboard.Instance.OnClosed -= Instance_OnClosed;
    }

    private void SetCaretColorAlpha(float value)
    {
        inputField.customCaretColor = true;
        var caret = inputField.caretColor;
        caret.a = value;
        inputField.caretColor = caret;
    }
}
