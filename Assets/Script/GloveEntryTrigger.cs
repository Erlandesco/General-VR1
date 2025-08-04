using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveEntryTrigger : MonoBehaviour
{
    public GameObject leftPlayerHand;
    public GameObject rightPlayerHand;

    public GameObject leftDummyHand;
    public GameObject rightDummyHand;

    public float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand"))
        {
            // Hide player hand
            leftPlayerHand.SetActive(false);

            // Show dummy hand with fade
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            leftDummyHand.SetActive(true);
            fadeCoroutine = StartCoroutine(FadeHand(leftDummyHand, 0f, 1f));
        }
        else if (other.CompareTag("RightHand"))
        {
            rightPlayerHand.SetActive(false);

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            rightDummyHand.SetActive(true);
            fadeCoroutine = StartCoroutine(FadeHand(rightDummyHand, 0f, 1f));
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("LeftHand"))
    //    {
    //        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
    //        fadeCoroutine = StartCoroutine(FadeOutAndDeactivate(leftDummyHand));
    //        leftPlayerHand.SetActive(true);
    //    }
    //    else if (other.CompareTag("RightHand"))
    //    {
    //        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
    //        fadeCoroutine = StartCoroutine(FadeOutAndDeactivate(rightDummyHand));
    //        rightPlayerHand.SetActive(true);
    //    }
    //}

    private IEnumerator FadeHand(GameObject hand, float from, float to)
    {
        float elapsed = 0f;
        CanvasGroup cg = hand.GetComponent<CanvasGroup>();
        if (cg == null) cg = hand.AddComponent<CanvasGroup>();

        cg.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = to;
    }

    private IEnumerator FadeOutAndDeactivate(GameObject hand)
    {
        yield return FadeHand(hand, 1f, 0f);
        hand.SetActive(false);
    }
}
