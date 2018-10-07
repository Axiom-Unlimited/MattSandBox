using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOptimization : MonoBehaviour {

    [Header("Disable/Enable on Trigger Enter/Exit")]
    public bool isDisable;
    public bool isEnable;
    public GameObject[] disableGameObject;
    public Behaviour[] disableBehaviour;

    private void OnValidate()
    {
        if (isDisable)
        {
            isEnable = false;
        }
        if(isEnable)
        {
            isDisable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (isDisable)
            {
                for (int i = 0; i < disableGameObject.Length; i++)
                {
                    disableGameObject[i].SetActive(false);
                }
                for (int i = 0; i < disableBehaviour.Length; i++)
                {
                    disableBehaviour[i].enabled = false;
                }
            }
            if (isEnable)
            {
                for (int i = 0; i < disableGameObject.Length; i++)
                {
                    disableGameObject[i].SetActive(true);
                }
                for (int i = 0; i < disableBehaviour.Length; i++)
                {
                    disableBehaviour[i].enabled = true;
                }
            }
        }
    }
}
