using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all sound effects playing
/// </summary>
public class SoundManager:MonoBehaviour
{
    #region Singleton

    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

    private void Awake()
    {
        //Debug.Log(this.gameObject.name);

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    public void PlaySound(AudioClip audio, AudioSource audioSource)
    {
        audioSource.PlayOneShot(audio);

    }
}
