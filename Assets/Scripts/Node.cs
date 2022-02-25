using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Store the neighbor nodes in terms of 2D direction
    public List<Node> options;

    public Selector selector;
    public Highlight highlight;

    void Awake()
    {
        options = new List<Node>();
        selector = GetComponent<Selector>();
        selector.OnIndicatorChange.AddListener(IndicatorChanged);
        selector.OnInteraction.AddListener(Interacted);

        highlight = GetComponent<Highlight>();

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



}
