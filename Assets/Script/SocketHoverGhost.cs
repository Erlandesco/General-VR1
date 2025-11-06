using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketHoverGhost : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostPrefab;
    public Transform ghostAnchor; // kosong = pakai transform socket
    public Vector3 ghostLocalPos;
    public Vector3 ghostLocalEuler;
    public Vector3 ghostLocalScale = Vector3.one;

    [Header("Filter")]
    public InteractionLayerMask validLayers = 4; // default: Everything
    public string requiredTag = ""; // kosong = bebas

    XRSocketInteractor _socket;
    GameObject _ghost;

    void Awake()
    {
        _socket = GetComponent<XRSocketInteractor>();
        _socket.hoverEntered.AddListener(OnHoverEntered);
        _socket.hoverExited.AddListener(OnHoverExited);
    }

    void OnDestroy()
    {
        if (_socket)
        {
            _socket.hoverEntered.RemoveListener(OnHoverEntered);
            _socket.hoverExited.RemoveListener(OnHoverExited);
        }
        if (_ghost) Destroy(_ghost);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        var go = args.interactableObject?.transform?.gameObject;
        if (!go || !Pass(go) || !ghostPrefab) return;

        if (_ghost == null)
        {
            var parent = ghostAnchor ? ghostAnchor : transform;
            _ghost = Instantiate(ghostPrefab, parent);
        }

        _ghost.transform.localPosition = ghostLocalPos;
        _ghost.transform.localRotation = Quaternion.Euler(ghostLocalEuler);
        _ghost.transform.localScale = ghostLocalScale;
        _ghost.SetActive(true);
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (_ghost) _ghost.SetActive(false);
    }

    bool Pass(GameObject go)
    {
        bool layerOk = (validLayers.value & (1 << go.layer)) != 0;
        bool tagOk = string.IsNullOrEmpty(requiredTag) || go.CompareTag(requiredTag);
        return layerOk && tagOk;
    }
}
