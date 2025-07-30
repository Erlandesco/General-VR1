using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStepManager : MonoBehaviour
{
    public Image tutorialImage; // UI Image tempat gambar
    public AudioSource audioSource; // Sumber audio
    public TutorialStep[] steps; // List semua step tutorial

    void Start()
    {
        StartCoroutine(PlayTutorialSteps());
    }

    IEnumerator PlayTutorialSteps()
    {
        foreach (var step in steps)
        {
            // Ganti gambar
            tutorialImage.sprite = step.image;

            // Putar audio
            audioSource.clip = step.audioClip;
            audioSource.Play();

            // Tunggu sampai audio selesai
            yield return new WaitUntil(() => !audioSource.isPlaying);

            // Optional: jeda antar step
            yield return new WaitForSeconds(0.5f);
        }
    }
}
