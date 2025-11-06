using UnityEngine;

/// <summary>
/// Apply target transform (pos/rot/scale) to a chosen object, with safe caching & revert.
/// Default uses LOCAL space (recommended for socketed objects).
/// </summary>
[DisallowMultipleComponent]
public class TargetMover : MonoBehaviour
{
    [Header("What to affect")]
    [Tooltip("Object whose transform will be changed. If null, uses this.gameObject.")]
    [SerializeField] private GameObject targetObject;

    [Header("Space")]
    [Tooltip("Apply in local space (recommended). If false, applies in world space.")]
    [SerializeField] private bool useLocalSpace = true;

    [Header("Affects")]
    public bool affectPosition = true;
    public bool affectRotation = true;
    public bool affectScale = true;

    [Header("Custom Pose (applied when MoveTargetToNewSocket is called)")]
    public Vector3 customPosition = Vector3.zero;          // local or world depending on useLocalSpace
    public Vector3 customEulerAngles = Vector3.zero;       // local or world depending on useLocalSpace
    public Vector3 customScale = Vector3.one;              // LOCAL scale

    [Header("Optional: Original Parent (for manual reparent if needed)")]
    public Transform originalParent;

    // cache
    private Transform _t;
    private Vector3 _origLocalPos;
    private Quaternion _origLocalRot;
    private Vector3 _origLocalScale;
    private Vector3 _origWorldPos;
    private Quaternion _origWorldRot;
    private bool _cached;

    private void Awake()
    {
        if (targetObject == null)
            targetObject = this.gameObject;

        _t = targetObject.transform;
        CacheOriginal();
    }

    private void CacheOriginal()
    {
        _origLocalPos = _t.localPosition;
        _origLocalRot = _t.localRotation;
        _origLocalScale = _t.localScale;

        _origWorldPos = _t.position;
        _origWorldRot = _t.rotation;

        _cached = true;
    }

    /// <summary>
    /// Apply the custom transform to the target.
    /// </summary>
    [ContextMenu("Apply Custom Transform")]
    public void MoveTargetToNewSocket()
    {
        if (_t == null)
        {
            Debug.LogWarning("[TargetMover] Target transform is null.");
            return;
        }
        if (!_cached) CacheOriginal();

        if (useLocalSpace)
        {
            if (affectPosition) _t.localPosition = customPosition;
            if (affectRotation) _t.localRotation = Quaternion.Euler(customEulerAngles);
        }
        else
        {
            if (affectPosition) _t.position = customPosition;
            if (affectRotation) _t.rotation = Quaternion.Euler(customEulerAngles);
        }

        if (affectScale) _t.localScale = customScale;
    }

    /// <summary>
    /// Revert target transform to the cached original values.
    /// </summary>
    [ContextMenu("Revert To Original Transform")]
    public void RevertToOriginalTransform()
    {
        if (_t == null || !_cached) return;

        // Revert position & rotation respecting chosen space
        if (useLocalSpace)
        {
            if (affectPosition) _t.localPosition = _origLocalPos;
            if (affectRotation) _t.localRotation = _origLocalRot;
        }
        else
        {
            if (affectPosition) _t.position = _origWorldPos;
            if (affectRotation) _t.rotation = _origWorldRot;
        }

        if (affectScale) _t.localScale = _origLocalScale;
    }

    /// <summary>Swap target at runtime; caches new original.</summary>
    public void SetNewTarget(GameObject newTarget)
    {
        if (!newTarget)
        {
            Debug.LogWarning("[TargetMover] SetNewTarget received null.");
            return;
        }
        targetObject = newTarget;
        _t = targetObject.transform;
        CacheOriginal();
    }

    /// <summary>Optional helper to restore parent.</summary>
    public void ReparentToOriginal()
    {
        if (originalParent == null || _t == null) return;
        _t.SetParent(originalParent, true);
    }

    // Convenience setters
    public void SetCustomPosition(Vector3 pos) => customPosition = pos;
    public void SetCustomEuler(Vector3 euler) => customEulerAngles = euler;
    public void SetCustomScale(Vector3 scl) => customScale = scl;
    public void UseLocalSpace(bool local) => useLocalSpace = local;
}
