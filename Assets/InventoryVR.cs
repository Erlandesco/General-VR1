using UnityEngine;
using UnityEngine.InputSystem; // NEW Input System

public class InventoryVR_XRI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject inventory;
    public Transform anchor;

    [Header("Toggle (Input System)")]
    // Seret action ke sini (mis. "XRI LeftHand Interaction/Menu")
    public InputActionReference toggleAction;

    [Header("Follow Options")]
    public bool parentWhileActive = false;          // true = jadikan child anchor saat aktif
    public Vector3 localPositionOffset = Vector3.zero;
    public Vector3 localEulerOffset = new Vector3(15f, 0f, 0f); // sama seperti +15 derajat di X

    bool active;
    Transform originalParent;

    void Awake()
    {
        if (inventory)
        {
            originalParent = inventory.transform.parent;
            inventory.SetActive(false);
            active = false;
        }
    }

    void OnEnable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed += OnToggle;
            toggleAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed -= OnToggle;
            toggleAction.action.Disable();
        }
    }

    void OnToggle(InputAction.CallbackContext _)
    {
        SetActive(!active);
    }

    public void SetActive(bool value)
    {
        active = value;
        if (!inventory || !anchor) return;

        inventory.SetActive(active);

        if (active)
        {
            if (parentWhileActive)
            {
                // Tempel sebagai child agar selalu ikut anchor
                inventory.transform.SetParent(anchor, worldPositionStays: false);
                inventory.transform.localPosition = localPositionOffset;
                inventory.transform.localRotation = Quaternion.Euler(localEulerOffset);
            }
            else
            {
                // Letakkan di world (bukan child), lalu follow tiap frame
                PlaceOnce();
            }
        }
        else
        {
            if (parentWhileActive && originalParent)
                inventory.transform.SetParent(originalParent, worldPositionStays: true);
        }
    }

    void LateUpdate()
    {
        // Follow jika tidak diparent-kan
        if (active && !parentWhileActive) PlaceOnce();
    }

    void PlaceOnce()
    {
        Vector3 worldPos = anchor.TransformPoint(localPositionOffset);
        Quaternion worldRot = anchor.rotation * Quaternion.Euler(localEulerOffset);
        inventory.transform.SetPositionAndRotation(worldPos, worldRot);
    }
}
