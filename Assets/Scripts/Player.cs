using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static uint playersCount;
    public uint id;
    public bool isAI = false;
    public bool placing = false;
    public Material material;

    public void Awake()
    {
        id = playersCount;
        playersCount++;
    }

}
