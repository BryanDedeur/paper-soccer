using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Store the neighbor nodes in terms of 2D direction
    public Dictionary<Vector2Int, Node> neighbors;
    public Dictionary<Vector2Int, Node> available;

    public Selector selector;
    public Highlight highlight;

    void Awake()
    {
        neighbors = new Dictionary<Vector2Int, Node>();
        available = new Dictionary<Vector2Int, Node>();
        selector = GetComponent<Selector>();
        selector.OnIndicatorChange.AddListener(IndicatorChanged);
        selector.OnInteraction.AddListener(Interacted);

        highlight = GetComponent<Highlight>();

    }

    public void Reset()
    {
        // Sets the neighbors to availableNeighbors
        available = neighbors;
    }

    public void Interacted()
    {
        if (highlight.IsHighlighted())
        {
            GameMgr.instance.MoveBallToNode(this);
        }

    }

    public void IndicatorChanged()
    {
        // does nothing for now
    }

    public void ToggleOptions(bool active)
    {
        foreach (KeyValuePair<Vector2Int, Node> neighbor in available)
        {
            neighbor.Value.highlight.SetState(true);
        }
    }

}
