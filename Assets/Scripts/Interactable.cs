using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool active;
    public UnityEvent OnIndicatorChange; // fires when selection indicator changes
    public UnityEvent OnInteraction; // fires when click is in range

    public Direction associatedDirection;

    private void Start()
    {
        InteractableMgr.instance.interactables.Add(this);
    }

    private void OnDestroy()
    {
        InteractableMgr.instance.interactables.Remove(this);
    }

    public void SetState(bool state)
    {
        active = state;
        if (active)
            InteractableMgr.instance.active.Add(this);
        else
            InteractableMgr.instance.active.Remove(this);

    }

    public void Interact()
    {
        OnInteraction.Invoke();
        GameMgr.instance.Move(associatedDirection);
    }
}
