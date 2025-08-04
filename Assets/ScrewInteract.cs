using UnityEngine;

public class ScrewInteract : MonoBehaviour
{
    public bool isUnscrewed = false;
    public GameObject visualScrew;

    private void OnTriggerEnter(Collider other)
    {
        if (isUnscrewed) return;

        if (other.CompareTag("Tools"))
        {
            isUnscrewed = true;
            if (visualScrew) visualScrew.SetActive(false); // sembunyikan baut
            gameObject.GetComponent<Collider>().enabled = false;

            TutorialScrewTracker.instance.UnscrewOne();
        }
    }
}
