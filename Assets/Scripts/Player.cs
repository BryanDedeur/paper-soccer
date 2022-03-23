using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int>
{
}

[System.Serializable]
public class StringEvent : UnityEvent<string>
{
}

[System.Serializable]
public class ColorEvent : UnityEvent<Color>
{
}

public class Player : MonoBehaviour
{
    public static uint playersCount;
    public uint id;
    public bool isAI = false;
    public bool placing = false;
    public int searchDepth = 1;

    public Color color;
    public Material material;

    public float turnTime;
    public int moves;
    public int bounces;
    public int wins;
    public float evaluation;

    public StringEvent OnTimeUpdate;
    public ColorEvent OnColorChange;
    public StringEvent OnMove;
    public StringEvent OnBounce;
    public StringEvent OnWin;
    public StringEvent OnWinPrint;
    public StringEvent OnEvaluation;
    public StringEvent OnDepthChanged;
    public UnityEvent OnTurnStarted;
    public UnityEvent OnTurnEnded;


    public void Awake()
    {
        id = playersCount;
        playersCount++;
        ColorChange();
    }

    public void Reset()
    {
        moves = 0;
        bounces = 0;
        turnTime = 0;
        OnMove.Invoke("Moves: " + moves.ToString());
        OnBounce.Invoke("Bounces: " + bounces.ToString());
        OnTimeUpdate.Invoke("Time(s): " + turnTime.ToString("F"));
    }

    public void SetAI(bool state)
    {
        isAI = state;
    }

    public void SetPlacing(bool state)
    {
        if (state)
        {
            OnTurnStarted.Invoke();
        } else
        {
            OnTurnEnded.Invoke();
        }
        placing = state;
    }

    public void IncrementMoveCounter()
    {
        moves++;
        OnMove.Invoke("Moves: " + moves.ToString());
    }

    public void IncrementBounceCounter()
    {
        bounces++;
        OnBounce.Invoke("Bounces: " + bounces.ToString());
    }

    public void IncrementWinCounter()
    {
        wins++;
        OnWin.Invoke("Wins: " + wins.ToString());
        PrintWinnerName();
    }

    public void PrintWinnerName()
    {
        if (isAI)
        {
            OnWinPrint.Invoke("Winner: Player" + (id + 1).ToString() + " (AI)");
        } else
        {
            OnWinPrint.Invoke("Winner: Player" + (id + 1).ToString());
        }
    }

    public void EvaluationUpdate(float staticEvaluatorValue)
    {
        evaluation = staticEvaluatorValue;
        OnEvaluation.Invoke("Evaluator: " + staticEvaluatorValue.ToString("F"));
    }

    private void ColorChange()
    {
        material.color = color;
        material.SetColor("_EmissionColor", color);
        OnColorChange.Invoke(color);
    }

    private void TimeUpdate()
    {
        turnTime += Time.deltaTime;
        OnTimeUpdate.Invoke("Time(s): " + turnTime.ToString("F"));
    }

    public void UpdateSearchDepth(float value)
    {
        searchDepth = (int) value;
        OnDepthChanged.Invoke("MinMax Depth (" + searchDepth.ToString() + ")");
    }

    public void Update()
    {
        if (placing)
            TimeUpdate();
    }

}
