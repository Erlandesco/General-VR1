using UnityEngine;

public class FadeInMaterial : MonoBehaviour
{
    [Tooltip("Renderer yang punya material (MeshRenderer/SkinnedMeshRenderer/SpriteRenderer).")]
    public Renderer targetRenderer;

    [Tooltip("Durasi menuju alpha penuh (detik).")]
    public float duration = 1.5f;

    Material mat;
    string colorProp = "_Color";   // fallback
    float t;

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        // Pakai instance material agar tidak mengubah material global
        mat = targetRenderer.material;

        // Deteksi properti warna: URP/HDRP biasanya _BaseColor
        if (mat.HasProperty("_BaseColor")) colorProp = "_BaseColor";
        else if (mat.HasProperty("_Color")) colorProp = "_Color";

        // Pastikan material dalam mode Transparent agar alpha berpengaruh
        // (Set di Inspector: Standard -> Rendering Mode = Transparent / URP Lit -> Surface Type = Transparent)
        // Atau kalau mau paksa via kode, tinggal set blending & renderQueue.
    }

    void Update()
    {
        if (mat == null) return;

        Color c = mat.GetColor(colorProp);
        // Gerak halus ke 1.0 (255)
        float step = (duration > 0f) ? Time.deltaTime / duration : 1f;
        c.a = Mathf.MoveTowards(c.a, 1f, step);
        mat.SetColor(colorProp, c);

        // (Opsional) hentikan script saat sudah penuh
        if (Mathf.Approximately(c.a, 1f)) enabled = false;
    }

    // Utility kalau kamu mau set alpha awal dalam skala 0–255
    public void SetAlpha255(int a255)
    {
        a255 = Mathf.Clamp(a255, 0, 255);
        Color c = mat.GetColor(colorProp);
        c.a = a255 / 255f;
        mat.SetColor(colorProp, c);
    }
}
