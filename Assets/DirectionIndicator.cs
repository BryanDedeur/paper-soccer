using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public static DirectionIndicator instance;
    Dictionary<Direction, GameObject> directions;


    public void Awake()
    {
        instance = this;
        directions = new Dictionary<Direction, GameObject>();
        directions.Add(Direction.N, transform.Find("North").gameObject);
        directions.Add(Direction.S, transform.Find("South").gameObject);
        directions.Add(Direction.E, transform.Find("East").gameObject);
        directions.Add(Direction.W, transform.Find("West").gameObject);
        directions.Add(Direction.NW, transform.Find("NorthWest").gameObject);
        directions.Add(Direction.NE, transform.Find("NorthEast").gameObject);
        directions.Add(Direction.SW, transform.Find("SouthWest").gameObject);
        directions.Add(Direction.SE, transform.Find("SouthEast").gameObject);
    }

    public void HideAll()
    {
        foreach (KeyValuePair<Direction, GameObject> direction in directions)
        {
            direction.Value.SetActive(false);
        }
    }

    public void ShowDirection (Direction dir)
    {
        directions[dir].SetActive(true);
    }
}
