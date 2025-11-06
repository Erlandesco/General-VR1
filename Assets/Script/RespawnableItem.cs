using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RespawnableItem : MonoBehaviour
{
    [Header("Anchor (wajib diisi di scene atau diset via prefab)")]
    public ItemSpawner spawnerAnchor;

    XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grab.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnSelectEntered);
    }

    void Start()
    {
        if (spawnerAnchor != null)
            spawnerAnchor.RegisterIfAtAnchor(gameObject);
        else
            Debug.LogWarning($"[SpawnableOnGrab] spawnerAnchor belum di-assign pada {name}");
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (spawnerAnchor != null)
            spawnerAnchor.RequestImmediateReplacement(gameObject);
    }
}
