﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class TimerScript : NetworkBehaviour
{
    public TextMeshProUGUI timerText;

    public AudioSource timerAudio;
    public AudioClip timerSFX;

    [SyncVar]
    private float flashTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();

        if (LevelTimer.Instance.TimeLeft <= 31 && LevelTimer.Instance.levelStarted)
        {
            TextFlash();
        }

        if (LevelTimer.Instance.hasLevelEnded)
        {
            timerAudio.Stop();
            timerText.gameObject.SetActive(true);
            timerText.text = "CLOSED";
        }
    }

    public void UpdateTime()
    {
        timerText.text = LevelTimer.Instance.TimingToString();
    }

    public void TextFlash()
    {
        flashTimer += Time.deltaTime;

        if (flashTimer >= 0.5f)
        {
            timerText.enabled = true;

            //timerText.color = new Color32(238, 0, 0, 255);

        }

        if (flashTimer >= 1f)
        {
            timerText.enabled = false;
            flashTimer = 0;
        }
    }
}
