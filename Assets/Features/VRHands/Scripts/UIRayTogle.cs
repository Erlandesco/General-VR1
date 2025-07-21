using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class UIRayTogle : MonoBehaviour
{
    public GameObject lineVisualObject;
    public LayerMask uiLayerMask;
    public float maxDistance = 10f;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        bool hitUI = Physics.Raycast(ray, out RaycastHit hit, maxDistance, uiLayerMask);

        if (lineVisualObject != null)
        {
            lineVisualObject.SetActive(hitUI);
        }
    }

   
}
