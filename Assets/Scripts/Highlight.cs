using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using UnityEngine;

public class Highlight : MonoBehaviour
{
    private GameObject highlightIndicator;

    private void Awake()
    {
        // Create the selection indicator
        highlightIndicator = Instantiate(HighlightMgr.instance.highlightPrefab);
        highlightIndicator.transform.position = transform.position - new Vector3(0, -0.01f, 0);
        highlightIndicator.transform.parent = transform;
        highlightIndicator.SetActive(false);
    }

    private void Start()
    {
        HighlightMgr.instance.highlightable.Add(this);
    }

    private void OnDestroy()
    {
        HighlightMgr.instance.highlightable.Remove(this);
    }

    public void SetState(bool isSelected)
    {
        highlightIndicator.SetActive(isSelected);
        HighlightMgr.instance.UpdateTracking(this);
    }

    public bool IsHighlighted()
    {
        // Use selection indicator to indicate if the object is selected or not.
        return highlightIndicator.activeSelf;
    }

    private void Update()
    {
        // do something to animate the selection indicator
    }
}
