using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateStar : MonoBehaviour
{

    public AudioSource starAudio;
    public AudioClip starSFX;

    // Start is called before the first frame update
    void Start()
    {
        starAudio.PlayOneShot(starSFX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
