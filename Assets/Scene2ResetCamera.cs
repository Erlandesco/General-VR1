using UnityEngine;
using UnityEngine.Rendering;
#if USING_URP
using UnityEngine.Rendering.Universal;
#endif

[DefaultExecutionOrder(-1000)]
public class Scene2ResetCamera : MonoBehaviour
{
    void Awake()
    {
        var cam = Camera.main;
        if (!cam) cam = FindObjectOfType<Camera>(true);
        if (!cam) return;

        cam.clearFlags = CameraClearFlags.Skybox; // atau SolidColor
        cam.cullingMask = ~0; // Everything

#if USING_URP
        var data = cam.GetUniversalAdditionalCameraData();
        if (data != null){
            data.renderPostProcessing = false;     // matikan dulu untuk tes
            data.cameraStack.Clear();              // buang overlay warisan
        }
#endif
    }
}
