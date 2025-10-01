// Rotasi UV 90 derajat (sekitar pusat 0.5,0.5)
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RotateUV90 : MonoBehaviour
{
    public bool clockwise = true; // true = searah jarum jam

    void Awake()
    {
        var mf = GetComponent<MeshFilter>();
        var mesh = mf.mesh;                // instance, bukan asset
        if (!mesh || mesh.uv == null) { Destroy(this); return; }

        var uvs = mesh.uv;
        for (int i = 0; i < uvs.Length; i++)
        {
            var uv = uvs[i] - new Vector2(0.5f, 0.5f);
            uvs[i] = clockwise
                ? new Vector2(uv.y, -uv.x) + new Vector2(0.5f, 0.5f)
                : new Vector2(-uv.y, uv.x) + new Vector2(0.5f, 0.5f);
        }
        mesh.uv = uvs;

        Destroy(this); // sudah apply, komponen dibuang
    }
}
