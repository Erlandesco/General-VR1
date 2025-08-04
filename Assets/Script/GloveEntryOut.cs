using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveEntryOut : MonoBehaviour
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
            //leftDummyHand.SetActive(true);

            // Show dummy hand with fade
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            leftPlayerHand.SetActive(true);
            fadeCoroutine = StartCoroutine(FadeHand(leftPlayerHand, 0f, 1f));
        }
        else if (other.CompareTag("RightHand"))
        {
            //rightDummyHand.SetActive(true);

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            rightPlayerHand.SetActive(true);
            fadeCoroutine = StartCoroutine(FadeHand(rightPlayerHand, 0f, 1f));
        }
    }

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
