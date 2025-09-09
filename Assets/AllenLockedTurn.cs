using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AllenLockedTurn : MonoBehaviour
{
    [Header("Refs")]
    public XRSocketInteractor boltSocket;
    public XRGrabInteractable allenKey;     // kunci L
    public Transform attachPoint;           // titik snap di kepala baut
    public Transform boltTransform;         // transform baut (untuk sumbu)
    public Transform boltHead;              // bagian kepala baut untuk geser ulir (opsional)

    [Header("Axis & Thread")]
    public Vector3 boltAxisLocal = Vector3.forward; // sumbu baut (local)
    public float degreesPerTurn = 360f;
    public float turnsToRelease = 3f;
    public float threadPitchCm = 0.8f; // jarak maju/mundur per 1 turn (cm)
    public bool invert;                 // balik arah jika kebalik

    bool isSnapped;
    IXRInteractor handInteractor;       // direct/ray interactor yg pegang
    float lastSignedAngle;
    float accumulatedDeg;
    Rigidbody keyRb;

    void OnEnable()
    {
        boltSocket.selectEntered.AddListener(OnSnap);
        boltSocket.selectExited.AddListener(OnUnsnap);

        allenKey.selectEntered.AddListener(OnGrab);
        allenKey.selectExited.AddListener(OnRelease);
    }
    void OnDisable()
    {
        boltSocket.selectEntered.RemoveListener(OnSnap);
        boltSocket.selectExited.RemoveListener(OnUnsnap);

        allenKey.selectEntered.RemoveListener(OnGrab);
        allenKey.selectExited.RemoveListener(OnRelease);
    }

    void OnSnap(SelectEnterEventArgs args)
    {
        // Kunci “menggigit” baut
        isSnapped = true;
        if (!keyRb) keyRb = allenKey.GetComponent<Rigidbody>();

        // Pastikan multiple select
        allenKey.selectMode = InteractableSelectMode.Multiple;

        // Bekukan posisi di attachPoint
        keyRb.isKinematic = true;
        allenKey.transform.SetPositionAndRotation(attachPoint.position, attachPoint.rotation);

        // Reset progress
        accumulatedDeg = 0f;
        lastSignedAngle = 0f;
    }

    void OnUnsnap(SelectExitEventArgs args)
    {
        // Lepas total
        isSnapped = false;
        handInteractor = null;
        if (keyRb) keyRb.isKinematic = false;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Kalau yang grab adalah tangan (direct / ray), simpan sebagai pengendali rotasi
        if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        {
            handInteractor = args.interactorObject;

            // Saat tersnap, objek TIDAK boleh ikut pindah ke tangan
            if (isSnapped)
            {
                // Pastikan kinematic & posisi tetap di attach point
                if (keyRb) keyRb.isKinematic = true;
                allenKey.movementType = XRBaseInteractable.MovementType.Kinematic;
                // “kunci di tempat”—biar XR IT nggak narik ke attach transform tangan
                allenKey.trackPosition = false; // (XRIT 3.x)
                allenKey.trackRotation = false; // rotasi kita atur manual
            }
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject == handInteractor)
            handInteractor = null;

        // Kalau masih tersnap via socket, tetap kinematic & diam.
        if (!isSnapped && keyRb) keyRb.isKinematic = false;

        // Pulihkan tracking default saat tak tersnap
        if (!isSnapped)
        {
            allenKey.trackPosition = true;
            allenKey.trackRotation = true;
        }
    }

    void Update()
    {
        if (!isSnapped) return;

        // 1) KUNCI POSISI di attachPoint (supaya tidak ikut tangan)
        allenKey.transform.position = attachPoint.position;

        // 2) Jika ada tangan yg pegang, ambil rotasi tangan → jadikan sudut di sekitar sumbu baut
        if (handInteractor != null)
        {
            Vector3 worldAxis = boltTransform.TransformDirection(boltAxisLocal.normalized);

            // Ambil dua vektor referensi untuk hitung sudut bertanda
            Vector3 refDir = boltTransform.forward;            // referensi (bebas, penting konsisten)
            Vector3 handDir = handInteractor.transform.forward; // arah tangan

            float signed = SignedAngleAroundAxis(handDir, refDir, worldAxis);
            if (invert) signed = -signed;

            float delta = Mathf.DeltaAngle(lastSignedAngle, signed);
            lastSignedAngle = signed;
            accumulatedDeg += delta;

            // 3) Terapkan rotasi kunci mengelilingi sumbu baut (posisi tetap di attach)
            allenKey.transform.rotation = Quaternion.AngleAxis(signed, worldAxis) * boltTransform.rotation;

            // 4) (Opsional) Progress ulir: geser kepala baut keluar/masuk
            if (boltHead)
            {
                float turns = accumulatedDeg / degreesPerTurn;
                boltHead.position = boltTransform.position + worldAxis * (turns * threadPitchCm * 0.01f);
                if (turns >= turnsToRelease)
                {
                    // Lepas otomatis dari socket jika sudah cukup putaran
                    boltSocket.EndManualInteraction();
                }
            }
        }
        else
        {
            // Tidak ada tangan yg pegang: tetap sejajarkan dengan attachPoint
            allenKey.transform.rotation = attachPoint.rotation;
        }
    }

    static float SignedAngleAroundAxis(Vector3 dir, Vector3 refDir, Vector3 axis)
    {
        Vector3 a = Vector3.ProjectOnPlane(refDir, axis);
        Vector3 b = Vector3.ProjectOnPlane(dir, axis);
        return Vector3.SignedAngle(a, b, axis);
    }
}
