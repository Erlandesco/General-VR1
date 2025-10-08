using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// Drives thumbstick, trigger, grip, and X/Y button animations.
    public class ControllerAnimator : MonoBehaviour
    {
        [Header("Thumbstick")]
        [SerializeField] Transform m_ThumbstickTransform;
        [SerializeField] Vector2 m_StickRotationRange = new Vector2(30f, 30f);
        [SerializeField] XRInputValueReader<Vector2> m_StickInput = new XRInputValueReader<Vector2>("Thumbstick");

        [Header("Trigger")]
        [SerializeField] Transform m_TriggerTransform;
        [SerializeField] Vector2 m_TriggerXAxisRotationRange = new Vector2(0f, -15f);
        [SerializeField] XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");

        [Header("Grip")]
        [SerializeField] Transform m_GripTransform;
        [SerializeField] Vector2 m_GripRightRange = new Vector2(-0.0125f, -0.011f);
        [SerializeField] XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");

        [Header("Y Button (Left Secondary)")]
        [SerializeField] Transform m_YButtonTransform;
        [SerializeField] Vector2 m_YButtonRange = new Vector2(0.001f, 0.00035f);
        // NOTE: Y di left controller = SecondaryButton
        [SerializeField] XRInputValueReader<float> m_YButtonInput = new XRInputValueReader<float>("SecondaryButton");

        [Header("X Button (Left Primary)")]
        [SerializeField] Transform m_XButtonTransform;
        [SerializeField] Vector2 m_XButtonRange = new Vector2(0.001f, 0.00035f);
        // NOTE: X di left controller = PrimaryButton
        [SerializeField] XRInputValueReader<float> m_XButtonInput = new XRInputValueReader<float>("PrimaryButton");

        // Cache posisi awal tombol supaya offset-nya relatif & aman saat Reset/Prefab
        Vector3 m_XButtonStartLocalPos;
        Vector3 m_YButtonStartLocalPos;

        void Awake()
        {
            if (m_XButtonTransform != null) m_XButtonStartLocalPos = m_XButtonTransform.localPosition;
            if (m_YButtonTransform != null) m_YButtonStartLocalPos = m_YButtonTransform.localPosition;
        }

        void OnEnable()
        {
            if (m_ThumbstickTransform == null || m_GripTransform == null || m_TriggerTransform == null)
            {
                enabled = false;
                Debug.LogWarning($"Controller Animator component missing references on {gameObject.name}", this);
                return;
            }

            m_StickInput?.EnableDirectActionIfModeUsed();
            m_TriggerInput?.EnableDirectActionIfModeUsed();
            m_GripInput?.EnableDirectActionIfModeUsed();

            // Enable X/Y
            m_XButtonInput?.EnableDirectActionIfModeUsed();
            m_YButtonInput?.EnableDirectActionIfModeUsed();
        }

        void OnDisable()
        {
            m_StickInput?.DisableDirectActionIfModeUsed();
            m_TriggerInput?.DisableDirectActionIfModeUsed();
            m_GripInput?.DisableDirectActionIfModeUsed();

            // Disable X/Y
            m_XButtonInput?.DisableDirectActionIfModeUsed();
            m_YButtonInput?.DisableDirectActionIfModeUsed();
        }

        void Update()
        {
            // Thumbstick tilt
            if (m_StickInput != null)
            {
                var stickVal = m_StickInput.ReadValue();
                m_ThumbstickTransform.localRotation =
                    Quaternion.Euler(-stickVal.y * m_StickRotationRange.x, 0f, -stickVal.x * m_StickRotationRange.y);
            }

            // Trigger squeeze
            if (m_TriggerInput != null)
            {
                var triggerVal = Mathf.Clamp01(m_TriggerInput.ReadValue());
                m_TriggerTransform.localRotation =
                    Quaternion.Euler(Mathf.Lerp(m_TriggerXAxisRotationRange.x, m_TriggerXAxisRotationRange.y, triggerVal), 0f, 0f);
            }

            // Grip slide
            if (m_GripInput != null)
            {
                var gripVal = Mathf.Clamp01(m_GripInput.ReadValue());
                var currentPos = m_GripTransform.localPosition;
                m_GripTransform.localPosition = new Vector3(
                    Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, gripVal),
                    currentPos.y,
                    currentPos.z
                );
            }

            // X Button press (move sedikit masuk ke body)
            if (m_XButtonTransform != null && m_XButtonInput != null)
            {
                var xVal = Mathf.Clamp01(m_XButtonInput.ReadValue()); // 0 (up) -> 1 (down)
                // Geser pada sumbu Y lokal tombol (atau sesuaikan dengan arah normal mesh tombolmu)
                m_XButtonTransform.localPosition = new Vector3(
                    m_XButtonStartLocalPos.x,
                    Mathf.Lerp(m_XButtonStartLocalPos.y + m_XButtonRange.x, m_XButtonStartLocalPos.y + m_XButtonRange.y, xVal),
                    m_XButtonStartLocalPos.z
                );
            }

            // Y Button press
            if (m_YButtonTransform != null && m_YButtonInput != null)
            {
                var yVal = Mathf.Clamp01(m_YButtonInput.ReadValue());
                m_YButtonTransform.localPosition = new Vector3(
                    m_YButtonStartLocalPos.x,
                    Mathf.Lerp(m_YButtonStartLocalPos.y + m_YButtonRange.x, m_YButtonStartLocalPos.y + m_YButtonRange.y, yVal),
                    m_YButtonStartLocalPos.z
                );
            }
        }
    }
}
