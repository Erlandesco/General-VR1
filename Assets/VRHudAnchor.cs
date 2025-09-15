using UnityEngine;

[DefaultExecutionOrder(100)]
public class VRHudAnchor : MonoBehaviour
{
    public Camera targetCam;
    [Tooltip("Posisi target di viewport (0..1). Contoh: kiri-atas = (0.12, 0.88)")]
    public Vector2 viewportAnchor = new Vector2(0.12f, 0.88f);
    [Tooltip("Jarak dari kamera (meter)")]
    public float distanceMeters = 1.1f;
    [Tooltip("Offset kecil dalam meter setelah ditempatkan via viewport")]
    public Vector3 localNudge = Vector3.zero; // mis. (0, 0, 0)

    [Header("Gerak Halus")]
    public float positionSmooth = 16f;  // lebih besar = lebih responsif
    public float rotationSmooth = 16f;

    [Header("Kenyamanan")]
    [Tooltip("Kunci roll supaya UI tidak ikut miring saat kepala miring")]
    public bool lockRoll = true;
    [Tooltip("Sedikit turunkan pitch agar panel tidak terlalu 'nempel' ke hidung")]
    public float pitchDownDegrees = 0f; // mis. 5–7 derajat kalau perlu

    Transform tf;
    Vector3 vel;

    void Reset()
    {
        targetCam = Camera.main;
    }

    void Awake()
    {
        tf = transform;
        if (!targetCam) targetCam = Camera.main;
        // Scale kecil biar world space Canvas enak
        if (Mathf.Approximately(tf.localScale.x, 1f))
            tf.localScale = Vector3.one * 0.001f;
    }

    void LateUpdate()
    {
        if (!targetCam) return;

        // 1) Hitung titik target di depan kamera pada jarak tertentu
        Vector3 vp = new Vector3(Mathf.Clamp01(viewportAnchor.x), Mathf.Clamp01(viewportAnchor.y), 0f);
        // Viewport (x,y) ke world di jarak 'distanceMeters' sepanjang forward kamera:
        Vector3 centerWorld = targetCam.transform.position + targetCam.transform.forward * distanceMeters;

        // Konversi viewport ke world di near plane dan proyeksikan ke jarak target:
        Vector3 nearWorld = targetCam.ViewportToWorldPoint(new Vector3(vp.x, vp.y, targetCam.nearClipPlane + 0.05f));
        // Arah dari kamera ke titik viewport
        Vector3 dir = (nearWorld - targetCam.transform.position).normalized;
        Vector3 targetPos = targetCam.transform.position + dir * distanceMeters;

        // Nudge sedikit di ruang kamera (mis. geser 2cm naik)
        targetPos += targetCam.transform.TransformVector(localNudge);

        // 2) Rotasi: menghadap ke depan kamera, optional lock roll + pitch down
        Quaternion look = Quaternion.LookRotation(targetCam.transform.forward, Vector3.up);
        if (lockRoll)
        {
            // Hilangkan roll: ambil yaw/pitch kamera terhadap up dunia
            Vector3 fwd = targetCam.transform.forward;
            Vector3 rightNoRoll = Vector3.Cross(Vector3.up, fwd).normalized;
            Vector3 upNoRoll = Vector3.Cross(fwd, rightNoRoll).normalized;
            look = Quaternion.LookRotation(fwd, upNoRoll);
        }
        if (!Mathf.Approximately(pitchDownDegrees, 0f))
            look = Quaternion.AngleAxis(-pitchDownDegrees, targetCam.transform.right) * look;

        // 3) Smooth damp
        tf.position = Vector3.Lerp(tf.position, targetPos, 1f - Mathf.Exp(-positionSmooth * Time.deltaTime));
        tf.rotation = Quaternion.Slerp(tf.rotation, look, 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
    }
}
