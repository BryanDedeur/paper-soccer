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
    public StringEvent OnEvaluation;


    public void Awake()
    {
        id = playersCount;
        playersCount++;
        ColorChange();
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
    }

    public void EvaluationUpdate(float staticEvaluatorValue)
    {
        evaluation = staticEvaluatorValue;
        OnEvaluation.Invoke("Evaluator: " + staticEvaluatorValue.ToString("F"));
    }

    private void ColorChange()
    {
        material.color = color;
        material.SetColor("_EmissionColor", Color.gray);
        OnColorChange.Invoke(color);
    }

    private void TimeUpdate()
    {
        turnTime += Time.deltaTime;
        OnTimeUpdate.Invoke("Time(s): " + turnTime.ToString("F"));
    }

    public void Update()
    {
        if (placing)
            TimeUpdate();
    }

}
