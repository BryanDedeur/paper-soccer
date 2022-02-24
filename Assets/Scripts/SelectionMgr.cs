using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMgr : MonoBehaviour
{
    public static SelectionMgr instance;
    public GameObject selectionIndicatorPrefab;
    public List<Selector> selectors;

    public List<Selector> selected;
    public float selectionRange = 1;

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
        while(selected.Count > 0)
        {
            selected[0].SetState(false);
        }
    }

    float IsInRange(Vector3 pos1, Vector3 pos2, float range)
    {
        float distance = (pos1 - pos2).magnitude;
        if (distance <= range)
        {
            return distance;
        }

        return 0;
    }

    public SortedList<float, Selector> GetSelectorsInRange(float range, Vector3 point)
    {
        SortedList<float, Selector> selectorsInRange = new SortedList<float, Selector>();
        foreach (Selector s in selectors)
        {
            float distance = IsInRange(s.transform.position, point, range);
            if (distance != 0)
            {
                selectorsInRange.Add(distance, s);
            }
        }
        return selectorsInRange;
    }

    public void UpdateTracking(Selector modifiedSelector)
    {
        // Adjusts the tracked selectors
        if (modifiedSelector.IsSelected())
        {
            selected.Add(modifiedSelector);
        }
        else
        {
            selected.Remove(modifiedSelector);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check for raycasts collisions on surface plane 
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                SortedList<float, Selector> selectorsInRange = GetSelectorsInRange(selectionRange, hit.point);
                if (selectorsInRange.Count > 0)
                {
                    // Fire an event related to the direct interaction with a selector before we modify it
                    selectorsInRange[selectorsInRange.Keys[0]].Interact();

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        // Invert the selected state for all selectors in range
                        foreach (KeyValuePair<float, Selector> pair in selectorsInRange)
                        {
                            pair.Value.SetState(!pair.Value.IsSelected());
                        }
                    }
                    else
                    {
                        DeselectAll();
                        // Invert the selected state on only the nearest selector
                        Selector nearest = selectorsInRange[selectorsInRange.Keys[0]];
                        nearest.SetState(true);
                    }
                }
            }
        }
    }
}
