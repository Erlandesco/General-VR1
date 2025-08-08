using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRControllerInteraction : MonoBehaviour
{
    // Aksi input untuk "klik" atau "tombol ambil" pada controller
    public InputActionProperty clickAction;

    void OnEnable()
    {
        clickAction.action.Enable();
        clickAction.action.performed += OnClickPerformed;
    }

    void OnDisable()
    {
        clickAction.action.Disable();
        clickAction.action.performed -= OnClickPerformed;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        // Lakukan Raycast dari controller ke depan
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            // Cek apakah yang kena adalah rotary switch
            SwitchRotator rotarySwitch = hit.transform.GetComponent<SwitchRotator>();
            if (rotarySwitch != null)
            {
                // Panggil fungsi TurnSwitch
                //rotarySwitch.TurnSwitch();
            }
        }
    }
}
