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
    public GameObject stepManager;

    void Start()
    {
        StartCoroutine(PlayTutorialSequence());
    }

    IEnumerator PlayTutorialSequence()
    {
        stepManager.SetActive(false);
        // Play voiceover
        tutorialAudio.Play();

        // Tunggu audio selesai
        yield return new WaitUntil(() => !tutorialAudio.isPlaying);

        // Step 1: Getarkan controller kanan
        rightHaptic.SendHapticImpulse(0.7f, 0.3f);
        yield return new WaitForSeconds(1f);

        // Step 2: Getarkan controller kiri
        leftHaptic.SendHapticImpulse(0.7f, 0.3f);

        yield return new WaitForSeconds(1f);
        stepManager.SetActive(false);





    }
}