// Rotasi UV 90 derajat (sekitar pusat 0.5,0.5)
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RotateUV90 : MonoBehaviour
{
    public bool clockwise = true;

    void Reset() { Apply(); }
    [ContextMenu("Apply UV 90°")]
    public void Apply()
    {
        var mf = GetComponent<MeshFilter>();
        var mesh = mf.sharedMesh;
        var uvs = mesh.uv;
        for (int i = 0; i < uvs.Length; i++)
        {
            // geser ke pusat
            Vector2 uv = uvs[i] - new Vector2(0.5f, 0.5f);
            // rotasi 90° (cw/ccw)
            uv = clockwise ? new Vector2(uv.y, -uv.x) : new Vector2(-uv.y, uv.x);
            // geser balik
            uvs[i] = uv + new Vector2(0.5f, 0.5f);
        }
        mesh.uv = uvs;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(mesh);
#endif
    }
}
