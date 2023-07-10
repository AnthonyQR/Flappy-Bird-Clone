using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    public int bestScore;

    public ScoreData (GameManagerScript gameManager)
    {
        bestScore = gameManager.bestScore;
    }
}
