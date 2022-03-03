using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public static DirectionIndicator instance;
    Dictionary<Directions, GameObject> directions;


    public void Awake()
    {
        instance = this;
        directions = new Dictionary<Directions, GameObject>();
        directions.Add(Directions.N, transform.Find("North").gameObject);
        directions.Add(Directions.S, transform.Find("South").gameObject);
        directions.Add(Directions.E, transform.Find("East").gameObject);
        directions.Add(Directions.W, transform.Find("West").gameObject);
        directions.Add(Directions.NW, transform.Find("NorthWest").gameObject);
        directions.Add(Directions.NE, transform.Find("NorthEast").gameObject);
        directions.Add(Directions.SW, transform.Find("SouthWest").gameObject);
        directions.Add(Directions.SE, transform.Find("SouthEast").gameObject);
    }

    public void HideAll()
    {
        foreach (KeyValuePair<Directions, GameObject> direction in directions)
        {
            direction.Value.SetActive(false);
        }
    }

    public void ShowDirection (Directions dir)
    {
        directions[dir].SetActive(true);
    }
}
