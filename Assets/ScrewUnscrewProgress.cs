using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ScrewUnscrewProgress : MonoBehaviour
{
    [Header("Refs")]
    public XRSocketInteractor socket;        // drag socket di kepala baut (allowSelect=false)
    public Transform screwAxisRef;           // pivot di kepala baut
    public Transform screwMesh;              // opsional: untuk anim keluar
    public AudioSource sfx;
    public AudioClip tickSfx, releaseSfx;

    [Header("Logic")]
    public float requiredTurns = 3f;                 // berapa putaran penuh utk lepas
    public Vector3 localAxis = Vector3.forward;      // sumbu baut
    public bool requireActivatePress = true;         // wajib tekan Trigger saat dihitung
    public bool countBothDirections = false;         // true: arah balik tetap dihitung

    [Header("Sensitivity")]
    public float rotationSensitivity = 3f;           // pengali delta derajat (≥1)
    public float minDegToIgnore = 0.0f;              // deadzone; 0 = hitung sekecil apapun
    public float maxDegPerFrame = 25f;               // clamp spike per-frame (derajat)

    [Header("Feedback")]
    public bool hapticEachQuarterTurn = true;
    public float hapticAmp = 0.2f;
    public float hapticDur = 0.05f;
    public InputActionProperty triggerAction; // drag dari XRI Left/Right Interaction/Activate

    float totalAngle;                  // akumulasi derajat (signed)
    int lastQuarterTicks;
    Quaternion lastRot;
    Transform tool;
    XRBaseInputInteractor hand;
    bool finished;

    void Reset()
    {
        socket = GetComponent<XRSocketInteractor>();
        if (!screwAxisRef) screwAxisRef = transform;
    }

    void OnEnable()
    {
        if (socket != null)
        {
            socket.allowSelect = false; // penting: zona snap saja
            socket.hoverEntered.AddListener(OnToolEnterZone);
            socket.hoverExited.AddListener(OnToolExitZone);
        }
    }

    void OnDisable()
    {
        if (socket != null)
        {
            socket.hoverEntered.RemoveListener(OnToolEnterZone);
            socket.hoverExited.RemoveListener(OnToolExitZone);
        }
    }

    void OnToolEnterZone(HoverEnterEventArgs args)
    {
        tool = args.interactableObject.transform;                 // alat tetap dipegang tangan
        hand = args.interactorObject as XRBaseInputInteractor; // untuk haptic
        lastRot = tool.rotation;
    }

    void OnToolExitZone(HoverExitEventArgs args)
    {
        if (tool == args.interactableObject.transform)
        {
            tool = null;
            hand = null;
        }
    }

    void Update()
    {
        if (finished || tool == null) return;

        // opsional: wajib trigger ditekan saat menghitung
        if (requireActivatePress && triggerAction.action != null && triggerAction.action.ReadValue<float>() < 0.5f)
            return;

        // hitung perubahan rotasi pergelangan
        Quaternion curr = tool.rotation;
        Quaternion delta = curr * Quaternion.Inverse(lastRot);
        delta.ToAngleAxis(out float deg, out Vector3 axis);
        if (deg > 180f) deg -= 360f;

        // arah relatif sumbu baut
        Vector3 screwAxisW = screwAxisRef.TransformDirection(localAxis.normalized);
        float signed = Mathf.Sign(Vector3.Dot(axis.normalized, screwAxisW)) * deg;

        // 🔧 sensitivitas & sanitasi
        float d = signed;
        d = Mathf.Clamp(d, -maxDegPerFrame, maxDegPerFrame);      // cegah spike
        if (Mathf.Abs(d) < minDegToIgnore) d = 0f;                // deadzone
        d *= Mathf.Max(1f, rotationSensitivity);                  // perbesar efek putaran

        // kalau mau dua arah sama-sama valid:
        if (countBothDirections) totalAngle += Mathf.Sign(d) * Mathf.Abs(d);
        else totalAngle += Mathf.Max(0f, d);  // hanya arah lepas

        lastRot = curr;

        // haptic tiap 1/4 putaran (hanya progres positif)
        if (hapticEachQuarterTurn)
        {
            int quarters = Mathf.FloorToInt(Mathf.Max(0f, totalAngle) / 90f);
            if (quarters > lastQuarterTicks && hand != null)
            {
                hand.SendHapticImpulse(hapticAmp, hapticDur);
                if (sfx && tickSfx) sfx.PlayOneShot(tickSfx);
                lastQuarterTicks = quarters;
            }
        }

        // selesai?
        float turns = Mathf.Abs(totalAngle) / 360f;
        if (turns >= requiredTurns) FinishUnscrew();
    }

    void FinishUnscrew()
    {
        finished = true;
        if (sfx && releaseSfx) sfx.PlayOneShot(releaseSfx);

        // anim keluarkan baut sedikit (opsional)
        if (screwMesh != null)
        {
            // contoh: geser 1 cm sepanjang sumbu lokal baut
            screwMesh.localPosition += localAxis.normalized * 0.01f;
            screwMesh.gameObject.SetActive(false); // atau anim penuh
        }

        // kabari tracker step tutorial, dll.
        if (TutorialScrewTracker.instance != null)
            TutorialScrewTracker.instance.UnscrewOne();

        // matikan collider kepala baut biar gak ke-trigger lagi
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }
}
