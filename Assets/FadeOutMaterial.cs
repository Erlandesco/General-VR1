using UnityEngine;

public class FadeOutMaterial : MonoBehaviour
{
    [Header("Tujuan & Durasi")]
    [Range(0, 255)] public int targetAlpha255 = 33; // set sesuka kamu
    public float duration = 1.2f;                  // detik

    Renderer r;
    Material mat;
    string colorProp = "_Color";

    void Awake()
    {
        r = GetComponent<Renderer>();
        mat = r.material; // instance
        if (mat.HasProperty("_BaseColor")) colorProp = "_BaseColor";
        else if (mat.HasProperty("_Color")) colorProp = "_Color";
        // Ingat: material harus Transparent agar alpha berpengaruh.
    }

    void OnEnable()
    {
        StartFadeTo(targetAlpha255, duration);
    }

    public void StartFadeTo(int alpha255, float dur)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(alpha255, dur));
    }

    System.Collections.IEnumerator FadeTo(int alpha255, float dur)
    {
        alpha255 = Mathf.Clamp(alpha255, 0, 255);
        float targetA = alpha255 / 255f;
        if (dur <= 0f) dur = 0.0001f;

        Color c = mat.GetColor(colorProp);
        float startA = c.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            c.a = Mathf.Lerp(startA, targetA, t);
            mat.SetColor(colorProp, c);
            yield return null;
        }
        c.a = targetA; // snap
        mat.SetColor(colorProp, c);
    }
}
