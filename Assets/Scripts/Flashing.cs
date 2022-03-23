using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flashing : MonoBehaviour
{
    public bool running = false;

    public float onTime = 2;
    public float offTime = 2;

    public UnityEvent OnActivated;
    public UnityEvent OffActivated;

    private bool active = true;
    private float timeRemaining = 1;

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0)
            {
                active = !active;
                if (active)
                {
                    OnActivated.Invoke();
                    timeRemaining = onTime;
                }
                else
                {
                    timeRemaining = offTime;
                    OffActivated.Invoke();
                }

            }
        }
    }
}
