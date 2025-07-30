using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStepManager : MonoBehaviour
{
    public Image tutorialImage;
    public AudioSource audioSource;
    public TutorialStep[] steps;
    public GameObject audioStep;
    public GameObject hologramScreen;
    public GameObject iMGController;
    public Button skipButton;
    public GameObject mainMenu;

    private Coroutine tutorialCoroutine;
    private bool isSkipped = false;

    void Start()
    {
        audioStep.SetActive(false);
        mainMenu.SetActive(false);

        skipButton.onClick.AddListener(SkipTutorial); // Tambahkan event listener
    }

    public IEnumerator PlayTutorialSteps()
    {
        isSkipped = false;
        tutorialCoroutine = StartCoroutine(TutorialRoutine());
        yield return tutorialCoroutine;
    }
    private IEnumerator TutorialRoutine()
    {
        hologramScreen.SetActive(true);
        iMGController.SetActive(true);

        foreach (var step in steps)
        {
            if (isSkipped) yield break;

            tutorialImage.sprite = step.image;
            audioStep.SetActive(true);
            audioSource.clip = step.audioClip;
            audioSource.Play();

            yield return new WaitUntil(() => !audioSource.isPlaying);

            if (isSkipped) yield break;

            yield return new WaitForSeconds(1f);
        }
        mainMenu.SetActive(true);
    }
    public void SkipTutorial()
    {
        isSkipped = true;

        if (tutorialCoroutine != null)
            StopCoroutine(tutorialCoroutine);

        audioSource.Stop();
        audioStep.SetActive(false);

        // Arahkan ke langkah berikutnya, misalnya:
        gameObject.SetActive(false);// Hilangkan UI tutorial
        hologramScreen.SetActive(false);
        iMGController.SetActive(false);
        mainMenu.SetActive(true);

    }

}
