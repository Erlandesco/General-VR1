using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif

// Pasang di SETIAP scene (1, 2, 3) di GameObject kosong, eksekusi sangat awal.
[DefaultExecutionOrder(-1000)]
public class PerSceneVisualReset : MonoBehaviour
{
    [Header("Camera")]
    public bool fixCamera = true;
    public CameraClearFlags clearFlags = CameraClearFlags.Skybox; // atau SolidColor
    public LayerMask cullingMask = ~0; // Everything
    public bool clearUrpStack = true;
    public bool? postFxEnabled = null; // null = jangan disentuh; true/false = paksa

    [Header("Render Settings (opsional)")]
    public bool overrideLighting = true;
    public Material skybox;              // isi kalau tiap scene punya skybox
    public bool fogEnabled = false;
    [Range(0f, 2f)] public float ambientIntensity = 1f;
    [Range(0f, 2f)] public float reflectionIntensity = 1f;

    void Awake()
    {
        // --- Camera ---
        if (fixCamera)
        {
            var cam = Camera.main;
            if (!cam) cam = FindObjectOfType<Camera>(true);
            if (cam)
            {
                cam.clearFlags = clearFlags;
                cam.cullingMask = cullingMask;

#if UNITY_RENDER_PIPELINE_UNIVERSAL
                var data = cam.GetUniversalAdditionalCameraData();
                if (data)
                {
                    if (clearUrpStack) data.cameraStack.Clear();
                    if (postFxEnabled.HasValue) data.renderPostProcessing = postFxEnabled.Value;
                }
#endif
            }
        }

        // --- Lighting ---
        if (overrideLighting)
        {
            if (skybox) RenderSettings.skybox = skybox;
            RenderSettings.fog = fogEnabled;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            DynamicGI.UpdateEnvironment();
        }

        Time.timeScale = 1f; // jaga-jaga
    }
}
