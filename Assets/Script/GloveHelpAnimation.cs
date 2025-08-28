using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveHelpAnimation : MonoBehaviour
{
    public Animator gloveAnimator;

    public void PlayHelpAnimation(string stateName)
    {
        if (gloveAnimator != null)
        {
            gloveAnimator.Play(stateName); // layer 0, mulai dari awal
        }
    }
    public void PlayPinchAnimation_0(string stateName)
    {
        if (gloveAnimator != null)
        {
            gloveAnimator.Play(stateName, 0, 0f); // layer 0, mulai dari awal
        }
    }

    public void PlayFirstAnimation_1(string stateName)
    {
        if (gloveAnimator != null)
        {
            gloveAnimator.Play(stateName, 0, 0f); // layer 0, mulai dari awal
        }
    }

    public void PlayLeftAnimation_2(string stateName)
    {
        if (gloveAnimator != null)
        {
            gloveAnimator.Play(stateName, 0, 0f); // layer 0, mulai dari awal
        }

    }
}
