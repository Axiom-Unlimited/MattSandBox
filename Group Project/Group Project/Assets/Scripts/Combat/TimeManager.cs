using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public bool activate; //Triggers the RunTimer
    public float slowDownRate = 0.05f; //Rate of Slow Speed
    public float slowDownTime = 2; //Time before Returning to Normal

    private void Update()
    {
        RunTimer();
    }

    void RunTimer()
    {
        if (activate)
        {
            Time.timeScale += (1 / slowDownTime) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
        }
        if (Time.timeScale == 1 && activate)
            activate = false;//Destroy(gameObject);
    }

    public void ActivateSlowMotion()
    {
        if (!activate)
        {
            Time.timeScale = slowDownRate;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            activate = true;
        }
        else
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ActivateSlowMotion();
    }
}
