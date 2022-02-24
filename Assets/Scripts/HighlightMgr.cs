using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightMgr : MonoBehaviour
{
    public static HighlightMgr instance;
    public GameObject highlightPrefab;
    public List<Highlight> highlightable;
    public List<Highlight> highlighted;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        } else
        {
            instance = this;
        }
    }

    public void DeselectAll()
    {
        while(highlighted.Count > 0)
        {
            highlighted[0].SetState(false);
        }
    }

    public void UpdateTracking(Highlight modifiedHighlight)
    {
        // Adjusts the tracked selectors
        if (modifiedHighlight.IsHighlighted())
        {
            highlighted.Add(modifiedHighlight);
        }
        else
        {
            highlighted.Remove(modifiedHighlight);
        }
    }
}
