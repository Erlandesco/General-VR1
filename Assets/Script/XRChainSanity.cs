using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRChainSanity : MonoBehaviour
{
    void Start()
    {
        var t = transform;
        while (t != null)
        {
            var s = t.localScale;
            if (s.x <= 0 || s.y <= 0 || s.z <= 0)
                Debug.LogError($"[XRChainSanity] WARNING: Negative/zero scale on {t.name}: {s}");
            var r = t.localEulerAngles;
            Debug.Log($"[XRChainSanity] {t.name} pos={t.localPosition} rot={r} scale={s}");
            t = t.parent;
        }
    }
}
