using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif  

[DefaultExecutionOrder(-1000)]
public class Scene2ResetCamera : MonoBehaviour
{
    [Header("RenderSettings (optional)")]
    public Material skyboxOverride;      // drag jika punya skybox khusus untuk Scene 2
    public bool overrideLighting = true;
    public bool disableFog = true;
    [Range(0f, 2f)] public float ambientIntensity = 1f;
    [Range(0f, 2f)] public float reflectionIntensity = 1f;

    [Header("Camera Reset")]
    public bool resetCamera = true;
    public CameraClearFlags clearFlags = CameraClearFlags.Skybox; // atau SolidColor
    public LayerMask cullingMask = ~0; // Everything
    public bool turnOffPostFx = true;
    public bool clearUrpCameraStack = true;

    [Header("Volumes")]
    public bool keepOnlyVolumesInThisScene = true;

    void Awake()
    {
        // 1) Reset Camera
        if (resetCamera)
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
                    if (turnOffPostFx) data.renderPostProcessing = false;
                    if (clearUrpCameraStack) data.cameraStack.Clear();
                }
#endif
            }
        }

        // 2) Matikan Volume global yang bukan milik Scene 2
        if (keepOnlyVolumesInThisScene)
        {
            foreach (var v in Resources.FindObjectsOfTypeAll<Volume>())
            {
                if (!v.gameObject.scene.IsValid()) continue;         // skip asset/prefab
                if (v.gameObject.scene != gameObject.scene)          // selain Scene 2 (termasuk DDOL)
                    v.enabled = false;
            }
        }

        // 3) Reset Render Settings
        if (overrideLighting)
        {
            if (skyboxOverride) RenderSettings.skybox = skyboxOverride;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            if (disableFog) RenderSettings.fog = false;
            DynamicGI.UpdateEnvironment();
        }

        // Safety
        Time.timeScale = 1f;
    }
}
