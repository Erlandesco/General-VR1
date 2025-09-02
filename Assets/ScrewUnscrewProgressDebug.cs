using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ScrewUnscrewProgressDebug : MonoBehaviour
{
    [Header("Refs")]
    public XRSocketInteractor socket;             // drag socket di kepala baut
    public Transform screwAxisRef;                // pivot tepat di kepala baut
    public Transform screwMesh;                   // opsional: mesh baut
    public InputActionProperty triggerAction;     // drag XRI <Right/Left> Interaction/Activate (TRIGGER)
    public InputActionProperty triggerRight;   // drag: XRI Right Interaction/Activate
    public InputActionProperty triggerLeft;    // drag: XRI Left Interaction/Activate
    public Transform toolTip;                     // opsional: ujung kunci L (untuk cek jarak)

    [Header("Logic")]
    public Vector3 localAxis = Vector3.forward;   // sumbu lokal baut (arah ulir)
    public float requiredTurns = 3f;              // putaran penuh untuk lepas
    public bool requireActivatePress = true;      // wajib sambil tekan trigger
    public bool countBothDirections = false;      // true: bolak-balik tetap maju
    public bool requireCloseDistance = false;     // true: wajib dekat ke kepala
    public float maxHeadDistance = 0.015f;        // jarak max (meter)

    [Header("Input Actions")]
    //public InputActionProperty triggerRight;   // drag: XRI Right Interaction/Activate
    //public InputActionProperty triggerLeft;    // drag: XRI Left Interaction/Activate

    private InputAction _activeTrigger;        // action yang dipakai saat ini

    [Header("Sensitivity")]
    public float rotationSensitivity = 3f;        // pengali delta derajat
    public float minDegToIgnore = 0f;             // deadzone
    public float maxDegPerFrame = 25f;            // clamp spike

    [Header("Debug")]
    public bool showLogs = true;
    public bool showOverlay = true;
    public bool drawAxisGizmo = true;

    // runtime
    Transform tool;                     // transform alat (dipegang tangan)

    XRBaseInputInteractor hand;
        // interactor tangan
    Quaternion lastRot;
    float totalAngle;                   // akumulasi (derajat)
    int lastQuarter;
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
            socket.allowSelect = false; // JANGAN ambil alat; biar tangan yg pegang
            socket.hoverEntered.AddListener(OnEnterZone);
            socket.hoverExited.AddListener(OnExitZone);
        }

        // pastikan action di-enable
        if (triggerRight.action != null) triggerRight.action.Enable();
        if (triggerLeft.action != null) triggerLeft.action.Enable();
    }

    void OnDisable()
    {
        if (socket != null)
        {
            socket.hoverEntered.RemoveListener(OnEnterZone);
            socket.hoverExited.RemoveListener(OnExitZone);
        }
        if (triggerRight.action != null) triggerRight.action.Disable();
        if (triggerLeft.action != null) triggerLeft.action.Disable();
    }

    void OnEnterZone(HoverEnterEventArgs args)
    {
        tool = args.interactableObject.transform;

        // ✔ cast ke XRBaseControllerInteractor (bukan XRBaseInputInteractor)
        hand = args.interactorObject as XRBaseInputInteractor;
        lastRot = tool.rotation;

        // Tentukan tangan (kiri/kanan) lewat ActionBasedController di parent
        _activeTrigger = null;
        if (hand is Component comp)
        {
            var ctrl = comp.GetComponentInParent<ActionBasedController>(); // ada di tangan
            XRNode node = XRNode.RightHand; // Default to RightHand

            // Determine the XRNode based on the GameObject name or other logic
            if (ctrl != null && ctrl.gameObject.name.Contains("Left", System.StringComparison.OrdinalIgnoreCase))
            {
                node = XRNode.LeftHand;
            }

            // pilih action yg benar (drag ini di Inspector)
            _activeTrigger = (node == XRNode.RightHand) ? triggerRight.action : triggerLeft.action;
            _activeTrigger?.Enable();
        }

        if (showLogs) Debug.Log($"[Screw] ENTER zone, hand={(hand ? hand.name : "null")}, activeTrigger={_activeTrigger?.name}");
    }

    void OnExitZone(HoverExitEventArgs args)
    {
        if (tool == args.interactableObject.transform)
        {
            tool = null; hand = null; _activeTrigger = null;
            if (showLogs) Debug.Log("[Screw] EXIT zone.");
        }
    }

    void Update()
    {
        if (finished || tool == null) return;

        // wajib trigger ditekan?
        if (requireActivatePress)
        {
            float tv = (_activeTrigger != null) ? _activeTrigger.ReadValue<float>() : 0f;
            if (tv < 0.5f)
            {
                if (showLogs) Debug.Log($"[Screw] Blocked: Trigger<0.5 ({tv:F2})");
                return;
            }
        }

        // 0b) optional gate: jarak ujung alat ke kepala
        if (requireCloseDistance && toolTip != null)
        {
            float d = Vector3.Distance(toolTip.position, socket.attachTransform.position);
            if (d > maxHeadDistance)
            {
                if (showLogs) Debug.Log($"[Screw] Blocked: distance {d:F3} > {maxHeadDistance}");
                return;
            }
        }

        // 1) delta rotasi alat
        Quaternion curr = tool.rotation;
        Quaternion delta = curr * Quaternion.Inverse(lastRot);
        delta.ToAngleAxis(out float deg, out Vector3 axis);
        if (deg > 180f) deg -= 360f;

        // 2) proyeksi arah terhadap sumbu baut
        Vector3 screwAxisW = screwAxisRef.TransformDirection(localAxis.normalized);
        float signed = Mathf.Sign(Vector3.Dot(axis.normalized, screwAxisW)) * deg;

        // 3) sanitasi & sensitivitas
        float dDeg = Mathf.Clamp(signed, -maxDegPerFrame, maxDegPerFrame);
        if (Mathf.Abs(dDeg) < minDegToIgnore) dDeg = 0f;
        dDeg *= Mathf.Max(1f, rotationSensitivity);

        // 4) akumulasi
        if (countBothDirections) totalAngle += Mathf.Sign(dDeg) * Mathf.Abs(dDeg);
        else totalAngle += Mathf.Max(0f, dDeg);

        lastRot = curr;

        // 5) debug log utama
   
        if (showLogs)
            {
                float turnsNow = Mathf.Abs(totalAngle) / 360f;
                float triggerVal = (_activeTrigger != null) ? _activeTrigger.ReadValue<float>() : 0f; // Define triggerVal here
                Debug.Log($"[Screw] d={dDeg:F2}deg | totalAngle={totalAngle:F1} | turns={turnsNow:F2} | trig={triggerVal:F2}");
            }
        

        // 6) selesai?
        float turns = Mathf.Abs(totalAngle) / 360f;
        if (turns >= requiredTurns)
        {
            FinishUnscrew();
        }
    }

    void FinishUnscrew()
    {
        finished = true;
        Debug.Log("[Screw] COMPLETE! Screw released.");

        // contoh: anim kecil lalu disable
        if (screwMesh)
        {
            screwMesh.localPosition += localAxis.normalized * 0.01f;
            screwMesh.gameObject.SetActive(false);
        }

        // kabari tracker step (kalau ada)
        if (TutorialScrewTracker.instance != null)
            TutorialScrewTracker.instance.UnscrewOne();

        // matikan collider agar tidak retrigger
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawAxisGizmo || screwAxisRef == null) return;
        Gizmos.color = Color.cyan;
        Vector3 p = screwAxisRef.position;
        Vector3 dir = screwAxisRef.TransformDirection(localAxis.normalized);
        Gizmos.DrawLine(p - dir * 0.1f, p + dir * 0.1f);
        Gizmos.DrawSphere(p + dir * 0.1f, 0.005f);
        Gizmos.DrawSphere(p - dir * 0.1f, 0.005f);
    }

    void OnGUI()
    {
        if (!showOverlay) return;
        var style = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = 14 };
        float turns = Mathf.Abs(totalAngle) / 360f;
        string txt =
            $"Screw DEBUG\n" +
            $"- tool: {(tool ? tool.name : "null")}\n" +
            $"- axis local: {localAxis}\n" +
            $"- totalAngle: {totalAngle:F1} deg\n" +
            $"- turns: {turns:F2} / {requiredTurns}\n" +
            $"- needTrigger: {requireActivatePress}\n" +
            $"- trigger val: {(triggerAction.action != null ? triggerAction.action.ReadValue<float>() : -1f):F2}\n";
        GUI.Box(new Rect(10, 10, 300, 130), txt, style);
    }
}
