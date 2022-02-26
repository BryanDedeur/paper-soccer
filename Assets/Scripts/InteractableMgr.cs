using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMgr : MonoBehaviour
{
    public static InteractableMgr instance;
    public GameObject focusIndicatorPrefab;
    public List<Interactable> interactables;

    public List<Interactable> active;

    public float interactableRange = 1;

    private GameObject focusIndicator;
    private Interactable focus;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        } else
        {
            instance = this;
        }
        focusIndicator = Instantiate(focusIndicatorPrefab);
    }

    float IsInRange(Vector3 pos1, Vector3 pos2, float range)
    {
        float distance = (pos1 - pos2).magnitude;
        if (distance <= range)
        {
            return distance;
        }

        return -1;
    }

    public Interactable GetNearestInteractableInRange(float range, Vector3 point)
    {
        Interactable interactable = null;
        float bestDistance = float.PositiveInfinity;
        foreach (Interactable s in interactables)
        {
            float distance = (point - s.transform.position).magnitude;
            if (distance < bestDistance)
            {
                interactable = s;
                bestDistance = distance;
            }
        }
        return interactable;
    }

    public void FocusOn(Interactable interactable)
    {
        focusIndicator.SetActive(interactable != null);
        focus = interactable;

        if (interactable == null)
        {
            return;
        }

        if (interactable.active)
        {
            focusIndicator.transform.position = interactable.transform.position;
        } else
        {
            focus = null;
            focusIndicator.SetActive(false);
        }
    }

    private void Update()
    {

        // Check for raycasts collisions on surface plane 
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
        {
            FocusOn(GetNearestInteractableInRange(interactableRange, hit.point));
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (focus)
            {
                focus.Interact();
            }
        }
    }
}
