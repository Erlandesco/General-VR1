using UnityEngine;
using System.Collections; // Dibutuhkan untuk IEnumerator
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class ControllerVibration : MonoBehaviour
{
    public HapticImpulsePlayer rightHaptic;
    public HapticImpulsePlayer leftHaptic;
    public AudioSource tutorialAudio;
    public TutorialStepManager tutorialStepManager; // Tambahkan ini!

    void Start()
    {
        StartCoroutine(PlayTutorialSequence());
    }

    IEnumerator PlayTutorialSequence()
    {
        tutorialAudio.Play();
        yield return new WaitUntil(() => !tutorialAudio.isPlaying);

        rightHaptic.SendHapticImpulse(0.7f, 0.3f);
        yield return new WaitForSeconds(1f);

        leftHaptic.SendHapticImpulse(0.7f, 0.3f);
        yield return new WaitForSeconds(1f);

        // Baru mulai step tutorial
        yield return StartCoroutine(tutorialStepManager.PlayTutorialSteps());
    }
}