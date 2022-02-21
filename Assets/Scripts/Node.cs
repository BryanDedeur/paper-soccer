using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbors;
    private GameObject selector;

    public void SetSelector(bool state)
    {
        selector.SetActive(state);
    }

    // Start is called before the first frame update
    void Awake()
    {
        selector = transform.Find("Selection").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
