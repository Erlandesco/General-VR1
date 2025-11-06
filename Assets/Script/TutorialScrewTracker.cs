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
    public GameObject bolt1;
    public GameObject bolt2;
    public GameObject bolt3;
    public GameObject bolt4;
    public TextMeshProUGUI labelMisionUpdate; // “Screws removed: X / N”
    public GameObject warningPanel;         // panel/teks peringatan (inactive default)
    public GameObject checkMarkMission;
    public float warningDuration = 2f;      // detik

    [Header("Ouput")]
    public int unscrewedCount = 0;
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
            Debug.Log("All BOLTS HAVE BEEN LOOSENED");
            checkMarkMission.SetActive(true);
            StartCoroutine(NonactiveBolt());

            // Optional: bisa nyalakan highlight cover, sfx, dsb
        }
        if (unscrewedCount == totalScrews)
        {
            StartCoroutine(NonactiveBolt());

            bolt1.SetActive(false);
            bolt2.SetActive(false);
            bolt3.SetActive(false);
            bolt4.SetActive(false);


        }
    }

    public bool AllScrewsRemoved()
    {
        return unscrewedCount >= totalScrews;
    }

    private void UpdateCounterUI()
    {
        if (labelMisionUpdate != null)
            labelMisionUpdate.text = $"Unscrew the bolt glove cover {unscrewedCount} / {totalScrews}";
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

    private System.Collections.IEnumerator NonactiveBolt()
    {
        yield return new WaitForSeconds(2f);
        bolt1.SetActive(false);
        bolt2.SetActive(false);
        bolt3.SetActive(false);
        bolt4.SetActive(false);

    }
}
