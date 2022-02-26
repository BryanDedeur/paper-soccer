using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMgr : MonoBehaviour
{
    public static LineMgr instance;
    private GameObject container;
    public static int count = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        container = new GameObject();
        container.name = "Lines";
    }

    public LineRenderer CreateLine(Vector3 start, Vector3 end, float width = 0.2f)
    {
        count++;
        GameObject lineObject = new GameObject();
        lineObject.name = "Line" + count.ToString();
        lineObject.transform.parent = container.transform;

        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        lr.startWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return lr;
    }
}
