using UnityEngine;

public class UIFOVClampAndFade : MonoBehaviour
{
    public Transform head;                  // Main Camera
    [Range(5f, 80f)] public float maxAngle = 35f;   // UI tidak keluar dari +/- sudut ini
    public float minDistance = 1.0f;
    public float maxDistance = 3.0f;

    [Header("Fade (butuh CanvasGroup)")]
    public CanvasGroup cg;
    public float fadeSmooth = 10f;

    void Reset()
    {
        if (!head && Camera.main) head = Camera.main.transform;
        if (!cg) cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
    }

    void LateUpdate()
    {
        if (!head) return;

        // Clamp sudut relatif ke arah tatap head (yaw/pitch ringan)
        Vector3 to = transform.position - head.position;
        float dist = to.magnitude;
        if (dist < 0.0001f) return;

        Vector3 fwd = head.forward;
        float ang = Vector3.Angle(fwd, to);

        if (ang > maxAngle)
        {
            // Proyeksikan ke kerucut dengan sudut maxAngle
            Quaternion rotTo = Quaternion.FromToRotation(to, Quaternion.AngleAxis(maxAngle, Vector3.Cross(fwd, to)) * fwd);
            Vector3 clamped = rotTo * to.normalized * dist;
            transform.position = head.position + clamped;
        }

        // Fade by distance (1 di minDistance, 0 di >maxDistance)
        if (cg)
        {
            float a = 1f;
            if (dist <= minDistance) a = 1f;
            else if (dist >= maxDistance) a = 0f;
            else a = 1f - (dist - minDistance) / (maxDistance - minDistance);

            cg.alpha = Mathf.Lerp(cg.alpha, a, 1f - Mathf.Exp(-fadeSmooth * Time.deltaTime));
            cg.blocksRaycasts = cg.alpha > 0.1f;
        }
    }
}
