using UnityEngine;

public class GloveInflateReverse : MonoBehaviour
{
    public Animator anim;

    // Panggil ini kalau mau inflate dulu, lalu otomatis lanjut deflate
    public void StartInflate()
    {
        anim.SetBool("Deflate", false);
        anim.SetBool("Inflate", true);
    }

    // Panggil ini kalau mau deflate dulu, lalu otomatis lanjut inflate
    public void StartDeflate()
    {
        anim.SetBool("Deflate", false);
        anim.SetBool("Inflate", true);
    }
}
