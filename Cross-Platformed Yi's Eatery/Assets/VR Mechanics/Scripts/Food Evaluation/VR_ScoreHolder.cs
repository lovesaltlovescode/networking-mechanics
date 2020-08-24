using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ScoreHolder
{
    private float idealScore, receivedScore;

    public VR_ScoreHolder(float _idealScore, float _receivedScore)
    {
        this.idealScore = _idealScore;
        this.receivedScore = _receivedScore;
    }
}
