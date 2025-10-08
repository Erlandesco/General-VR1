using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

/// Toggle UI Mission Panel + sembunyi/tampilkan hint di model controller
/// Contoh untuk LEFT controller: Y = SecondaryButton, X = PrimaryButton
public class ControllerHintAndPanelToggler : MonoBehaviour
{
    [Header("Inputs (Left Controller)")]
    // X = Primary, Y = Secondary (Left Controller)
    [SerializeField] XRInputValueReader<float> m_XButtonInput = new XRInputValueReader<float>("PrimaryButton");
    [SerializeField] XRInputValueReader<float> m_YButtonInput = new XRInputValueReader<float>("SecondaryButton");

    [Header("Panels to Toggle")]
    [SerializeField] GameObject m_MainMenuPanel; // dipicu tombol X
    [SerializeField] GameObject m_MissionPanel;  // dipicu tombol Y

    [Header("Hint UI Objects")]
    [SerializeField] GameObject m_XHintUI;  // text/ikon: "X  Open MAIN MENU" (misal)
    [SerializeField] GameObject m_YHintUI;  // text/ikon: "Y  Open MISSION PANEL"

    [Header("Behavior")]
    [Range(0f, 1f)]
    [SerializeField] float m_PressThreshold = 0.55f;
    [SerializeField] bool m_ForcePanelsOffOnStart = true;  // matikan panel di awal
    [SerializeField] bool m_StartHintsActive = true;       // tampilkan hint di awal

    bool m_XPrevPressed;
    bool m_YPrevPressed;

    void OnEnable()
    {
        m_XButtonInput?.EnableDirectActionIfModeUsed();
        m_YButtonInput?.EnableDirectActionIfModeUsed();
    }

    void OnDisable()
    {
        m_XButtonInput?.DisableDirectActionIfModeUsed();
        m_YButtonInput?.DisableDirectActionIfModeUsed();
    }

    void Start()
    {
        if (m_ForcePanelsOffOnStart)
        {
            if (m_MainMenuPanel) m_MainMenuPanel.SetActive(false);
            if (m_MissionPanel) m_MissionPanel.SetActive(false);
        }

        if (m_StartHintsActive)
        {
            if (m_XHintUI) m_XHintUI.SetActive(true);
            if (m_YHintUI) m_YHintUI.SetActive(true);
        }

        // sinkron awal: hint tampil hanya jika panelnya off
        SyncHintsWithPanels();
    }

    void Update()
    {
        // ----- X (Primary) -> Main Menu -----
        float xVal = m_XButtonInput != null ? Mathf.Clamp01(m_XButtonInput.ReadValue()) : 0f;
        bool xPressed = xVal >= m_PressThreshold;

        if (xPressed && !m_XPrevPressed)
        {
            // toggle panel
            if (m_MainMenuPanel)
            {
                bool newState = !m_MainMenuPanel.activeSelf;
                m_MainMenuPanel.SetActive(newState);

                // hint X mengikuti state panel (hide saat panel on, show saat panel off)
                if (m_XHintUI) m_XHintUI.SetActive(!newState);
            }
        }
        m_XPrevPressed = xPressed;

        // ----- Y (Secondary) -> Mission -----
        float yVal = m_YButtonInput != null ? Mathf.Clamp01(m_YButtonInput.ReadValue()) : 0f;
        bool yPressed = yVal >= m_PressThreshold;

        if (yPressed && !m_YPrevPressed)
        {
            if (m_MissionPanel)
            {
                bool newState = !m_MissionPanel.activeSelf;
                m_MissionPanel.SetActive(newState);

                // hint Y mengikuti state panel
                if (m_YHintUI) m_YHintUI.SetActive(!newState);
            }
        }
        m_YPrevPressed = yPressed;

        // (opsional tapi aman) sync terus-menerus jika panel ditutup lewat cara lain (button close di UI, dsb.)
        SyncHintsWithPanels();
    }

    void SyncHintsWithPanels()
    {
        if (m_XHintUI && m_MainMenuPanel)
            m_XHintUI.SetActive(!m_MainMenuPanel.activeSelf);

        if (m_YHintUI && m_MissionPanel)
            m_YHintUI.SetActive(!m_MissionPanel.activeSelf);
    }
}
