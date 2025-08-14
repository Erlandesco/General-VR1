using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class TutorialScrewTracker : MonoBehaviour
{
    public static TutorialScrewTracker instance;

    [Header("Config")]
    public int totalScrews = 4;

    [Header("Refs")]
    public XRGrabInteractable gloveCover;   // drag cover di sini
    public TextMeshProUGUI screwCounterText; // “Screws removed: X / N”
    public GameObject warningPanel;         // panel/teks peringatan (inactive default)
    public float warningDuration = 2f;      // detik

    [HideInInspector]public int unscrewedCount = 0;
    private Coroutine warningCo;

    void Awake()
    {
        instance = this;
        // Cover tetap enabled; seleksi difilter via SelectFilter
        UpdateCounterUI();
        if (warningPanel != null) warningPanel.SetActive(false);
    }

    public void UnscrewOne()
    {
        unscrewedCount = Mathf.Clamp(unscrewedCount + 1, 0, totalScrews);
        UpdateCounterUI();

        if (unscrewedCount >= totalScrews)
        {
            Debug.Log("All screws removed.");
            // Optional: bisa nyalakan highlight cover, sfx, dsb
        }
    }

    public bool AllScrewsRemoved()
    {
        return unscrewedCount >= totalScrews;
    }

    private void UpdateCounterUI()
    {
        if (screwCounterText != null)
            screwCounterText.text = $"Screws removed: {unscrewedCount} / {totalScrews}";
    }

    public void ShowWarning(string msg)
    {
        if (warningPanel == null) return;

        var tmp = warningPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = msg;

        if (warningCo != null) StopCoroutine(warningCo);
        warningCo = StartCoroutine(ShowWarningCo());
    }

    private System.Collections.IEnumerator ShowWarningCo()
    {
        warningPanel.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        warningPanel.SetActive(false);
    }
}
