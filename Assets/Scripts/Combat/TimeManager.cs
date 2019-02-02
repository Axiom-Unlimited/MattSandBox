using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    public bool uIEnabled;
    public GameObject choiceInterface;
    public Image timerImage;
    [Space]
    public bool activate; //Triggers the RunTimer
    public float slowDownRate = 0.05f; //Rate of Slow Speed
    public float slowDownTime = 2; //Time before Returning to Normal

    private void Update()
    {
        RunTimer();
        RunUI();
    }

    #region Timer
    void RunTimer()
    {
        if (activate)
        {
            Time.timeScale += (1 / slowDownTime) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);

            if (Time.timeScale == 1 && activate)
                activate = false;//Destroy(gameObject);
        }
        
    }
    #endregion
    #region UI
    bool RunUI()
    {
        if (uIEnabled)
        {
            if (activate && choiceInterface != null && choiceInterface.activeSelf == false)
                choiceInterface.SetActive(true);
            else if (!activate && choiceInterface != null && choiceInterface.activeSelf == true)
                choiceInterface.SetActive(false);

            #region Inputs
            if (activate)
            {
                if (Input.GetButtonDown("Input1"))
                {
                    Choice1();
                }
                else if (Input.GetButtonDown("Input2"))
                {
                    Choice2();
                }
                else if (Input.GetButtonDown("Input3"))
                {
                    Choice3();
                }
                else if (Input.GetButtonDown("Input4"))
                {
                    Choice4();
                }

            }
            #endregion

            if (timerImage != null)
                timerImage.fillAmount = Time.timeScale;
            return true;
        }
        else
            return false;
        
    }
    #endregion
    #region Slow Motion
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
    #endregion
    #region Activators on Collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ActivateSlowMotion();
    }
    #endregion
    #region Choices
    public void Choice1()
    {
        Debug.Log("Choice 1");
        Time.timeScale = 0.9f;
    }
    public void Choice2()
    {
        Debug.Log("Choice 2");
        Time.timeScale = 0.9f;
    }
    public void Choice3()
    {
        Debug.Log("Choice 3");
        Time.timeScale = 0.9f;
    }
    public void Choice4()
    {
        Debug.Log("Choice 4");
        Time.timeScale = 0.9f;
    }
    #endregion
}
