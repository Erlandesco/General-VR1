using UnityEngine;

public class TriggerDebug : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var t = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform;
        Debug.Log($"ENTER -> other='{other.name}', tag='{t.tag}', layer='{LayerMask.LayerToName(other.gameObject.layer)}'");
    }
    void OnTriggerExit(Collider other)
    {
        var t = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform;
        Debug.Log($"EXIT -> other='{other.name}', tag='{t.tag}', layer='{LayerMask.LayerToName(other.gameObject.layer)}'");
    }
}