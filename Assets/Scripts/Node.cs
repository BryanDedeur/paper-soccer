using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Store the neighbor nodes in terms of 2D direction

    public Selector selector;
    public Highlight highlight;
    public Interactable interactable;

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnInteraction.AddListener(Interacted);
        highlight = GetComponent<Highlight>();
    }

    public void Interacted()
    {
        //GameMgr.instance.MoveBallToNode(this);
    }

    public void IndicatorChanged()
    {
        // does nothing for now
    }



}
