using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Door : Interactable {

    public bool isLocked;
    public bool isOpen;
    private bool isActivated;

    public AudioClip lockedSoundEffect;
    public AudioClip openCloseSoundEffect;
    public Animator[] animationEffects;

    AudioSource au;

    // Use this for initialization
    void Start()
    {
        au = GetComponent<AudioSource>();

        if (animationEffects.Length > 0)
        {
            for (int i = 0; i < animationEffects.Length; i++)
            {
                animationEffects[i].SetBool("isOpen", isOpen);
            }
        }
    }

    public override void Use()
    {
        base.Use();

        #region Audio
        if (isLocked)
        {
            if (lockedSoundEffect != null)
            {
                au.clip = lockedSoundEffect;
                au.Play();
            }
        }
        else
        {
            if(openCloseSoundEffect != null)
            {
                au.clip = openCloseSoundEffect;
                au.Play();
            }
        }
        #endregion
        #region Animation
        if (isLocked)
            return;
        else
        {
            if (animationEffects.Length > 0)
            {
                for (int i = 0; i < animationEffects.Length; i++)
                {
                    animationEffects[i].SetBool("isOpen", isOpen);
                }
            }
        }
        #endregion
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetAxis("Interact") != 0 && !isActivated)
        {
            if (isOpen)
                isOpen = false;
            else
                isOpen = true;

            Use();
            isActivated = true;
        }

        if (Input.GetAxis("Interact") == 0 && isActivated)
        {
            isActivated = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isActivated = false;
    }
}
