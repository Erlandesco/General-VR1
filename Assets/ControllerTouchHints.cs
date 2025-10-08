using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class ControllerTouchHints : MonoBehaviour
{
    [Header("Touch Inputs (0..1)")]
    // primary = X (Left), secondary = Y (Left). Sesuaikan device-mapping kamu.
    [SerializeField] XRInputValueReader<float> m_PrimaryTouched = new XRInputValueReader<float>("PrimaryTouch");
    [SerializeField] XRInputValueReader<float> m_SecondaryTouched = new XRInputValueReader<float>("SecondaryTouch");

    [Header("Hint UI Objects")]
    [SerializeField] GameObject m_XHintUI;   // tooltip/hint untuk tombol X
    [SerializeField] GameObject m_YHintUI;   // tooltip/hint untuk tombol Y

    [Header("Real UI Panels (yang harus ‘menyuspend’ hint)")]
    [SerializeField] Canvas m_MainMenuPanel;   // UI beneran untuk X
    [SerializeField] GameObject m_MissionPanel;    // UI beneran untuk Y

    [Header("Behavior")]
    [Tooltip("Nilai minimal untuk dianggap 'touched'")]
    [Range(0f, 1f)] public float touchThreshold = 0.4f;

    [Tooltip("Jika true, saat UI beneran aktif, hint otomatis dimatikan & input touch diabaikan.")]
    public bool suspendWhenRealUIActive = true;

    // optional: manual override kalau kamu mau suspend via event
    bool m_ManualSuspended;

    void OnEnable()
    {
        m_PrimaryTouched?.EnableDirectActionIfModeUsed();
        m_SecondaryTouched?.EnableDirectActionIfModeUsed();

        // default matikan hint di awal
        if (m_XHintUI) m_XHintUI.SetActive(false);
        if (m_YHintUI) m_YHintUI.SetActive(false);
    }

    void OnDisable()
    {
        m_PrimaryTouched?.DisableDirectActionIfModeUsed();
        m_SecondaryTouched?.DisableDirectActionIfModeUsed();
    }

    void Update()
    {
        // 1) Cek apakah harus ‘suspend’ (UI beneran aktif)
        bool realUIActive = false;
        if (suspendWhenRealUIActive)
        {
            if (m_MainMenuPanel && m_MainMenuPanel.enabled) realUIActive = true;
            if (m_MissionPanel && m_MissionPanel.activeSelf) realUIActive = true;
        }
        bool suspended = realUIActive || m_ManualSuspended;

        if (suspended)
        {
            // Matikan semua hint dan skip baca touch
            if (m_XHintUI && m_XHintUI.activeSelf) m_XHintUI.SetActive(false);
            if (m_YHintUI && m_YHintUI.activeSelf) m_YHintUI.SetActive(false);
            return;
        }

        // 2) Baca nilai touch
        float p = m_PrimaryTouched != null ? Mathf.Clamp01(m_PrimaryTouched.ReadValue()) : 0f;   // X touch
        float s = m_SecondaryTouched != null ? Mathf.Clamp01(m_SecondaryTouched.ReadValue()) : 0f; // Y touch
        bool xTouched = p >= touchThreshold;
        bool yTouched = s >= touchThreshold;

        // 3) Tampilkan / sembunyikan hint sesuai touch
        if (m_XHintUI) m_XHintUI.SetActive(xTouched);
        if (m_YHintUI) m_YHintUI.SetActive(yTouched);
    }

    // ==== API opsional, kalau kamu mau suspend via event ====
    public void SuspendHints(bool suspend)
    {
        m_ManualSuspended = suspend;
        if (suspend)
        {
            if (m_XHintUI) m_XHintUI.SetActive(false);
            if (m_YHintUI) m_YHintUI.SetActive(false);
        }
    }

    // panggil ini dari event saat UI beneran dibuka/dutup (kalau kamu prefer event-berbasis)
    public void OnMainMenuShown() { SuspendHints(true); }
    public void OnMainMenuHidden() { SuspendHints(false); }

    public void OnMissionShown() { SuspendHints(true); }
    public void OnMissionHidden() { SuspendHints(false); }
}
