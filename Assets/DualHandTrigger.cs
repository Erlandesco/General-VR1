using System.Collections;
using UnityEngine;

public class DualHandTrigger : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    [Tooltip("Nama state anim buka di layer 0")]
    public string openStateName = "Open";
    [Tooltip("Nama state anim tutup di layer 0")]
    public string closeStateName = "Close";
    [Tooltip("Waktu crossfade (detik)")]
    public float crossFade = 0.05f;

    [Header("Glove (opsional)")]
    public GameObject gloveOpen;
    public GameObject gloveClose;

    // sensor state (di-update dari HandTrigger)
    bool rightInside = false;
    bool leftInside = false;

    // pintu state
    bool isOpen = false;

    // anti-spam
    bool bothInsideLast = false;   // rising-edge detector
    bool armed = true;             // hanya toggle saat sudah re-armed
    bool animLock = false;         // true = sedang anim, block input

    public void RightHandEnter() { SetRight(true); }
    public void RightHandExit() { SetRight(false); }
    public void LeftHandEnter() { SetLeft(true); }
    public void LeftHandExit() { SetLeft(false); }

    void SetRight(bool inside)
    {
        rightInside = inside;
        TryUpdate();
    }
    void SetLeft(bool inside)
    {
        leftInside = inside;
        TryUpdate();
    }

    void TryUpdate()
    {
        if (animLock) return; // ⛔ jangan terima input saat anim berjalan

        bool bothNow = rightInside && leftInside;

        // rising edge: sebelumnya tidak lengkap -> sekarang lengkap
        if (!bothInsideLast && bothNow && armed)
        {
            ToggleDoor();
            armed = false; // tunggu sampai salah satu keluar dulu
        }

        // re-arm ketika salah satu keluar
        if (!bothNow) armed = true;

        bothInsideLast = bothNow;
    }

    public void ToggleDoor()
    {
        if (animLock)
        {
            return;   // ⛔ cegah retrigger saat anim belum selesai
        }

        isOpen = !isOpen;

        // atur visual glove (opsional)
        if (gloveOpen != null)
        {
        }
        if (gloveClose != null)
        {
        }

        // mainkan anim dan kunci input sampai selesai
        string targetState = isOpen ? openStateName : closeStateName;
        StartCoroutine(PlayAndLock(targetState));

        if (isOpen)
        {
            gloveOpen.SetActive(isOpen);
            gloveClose.SetActive(false);
            Debug.Log("TOGGLE: OPEN");
        }
        else
        {
            Debug.Log("TOGGLE: CLOSE");
        }
    }


    IEnumerator PlayAndLock(string stateName)
    {
        animLock = true;

        // crossfade masuk ke state
        animator.CrossFadeInFixedTime(stateName, crossFade);

        // tunggu benar2 masuk ke state itu
        yield return null;
        var info = animator.GetCurrentAnimatorStateInfo(0);
        int guard = 0;
        while (!info.IsName(stateName) && guard++ < 10)
        {
            yield return null;
            info = animator.GetCurrentAnimatorStateInfo(0);
        }

        // tunggu sampai anim (nyaris) selesai
        while (info.IsName(stateName) && info.normalizedTime < 0.98f)
        {
            yield return null;
            info = animator.GetCurrentAnimatorStateInfo(0);
        }

        yield return new WaitForSeconds(1f);
        animLock = false; // ✅ boleh input lagi
    }
}
