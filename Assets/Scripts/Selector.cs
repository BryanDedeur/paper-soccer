using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using UnityEngine;

public class Selector : MonoBehaviour
{
    private GameObject selectionIndicator;
    public UnityEvent OnIndicatorChange; // fires when selection indicator changes
    public UnityEvent OnInteraction; // fires when click is in range

    private void Awake()
    {
        SelectionMgr.instance.selectors.Add(this);

        // Create the selection indicator
        selectionIndicator = Instantiate(SelectionMgr.instance.selectionIndicatorPrefab);
        selectionIndicator.transform.position = transform.position - new Vector3(0,-0.01f,0);
        selectionIndicator.transform.parent = transform;

        SetState(false);
    }

    private void OnDestroy()
    {
        SelectionMgr.instance.selectors.Remove(this);
    }

    public void Interact()
    {
        OnInteraction.Invoke();
    }

    public void SetState(bool isSelected)
    {
        selectionIndicator.SetActive(isSelected);
        OnIndicatorChange.Invoke();
        SelectionMgr.instance.UpdateTracking(this);
    }

    public bool IsSelected()
    {
        // Use selection indicator to indicate if the object is selected or not.
        return selectionIndicator.activeSelf;
    }

    private void Update()
    {
        // do something to animate the selection indicator
    }



}
