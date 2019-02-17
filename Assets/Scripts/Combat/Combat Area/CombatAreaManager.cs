using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatAreaManager : MonoBehaviour
{
    [Header("Combat Check")]
    [Tooltip("Decides whether Player can Activate Combat Mode")]
    public bool combatInitiated;
    [Tooltip("If Player leaves the Combat Area, this will Activate and Combat Initiated will Reset, so that the Player can re-inact Combat")]
    public float combatResetTime = 1;
    [Header("Slow Effect")]
    [Tooltip("Slow Effect increments over time")]
    public float slowRate = 0.05f;
    [Tooltip("Slow Effect time before returning to normal")]
    public float slowTime = 2;
    [Header("UI Elements")]
    public GameObject combatOptionsMenu;
    public Image weaponSlot1;
    public Image weaponSlot2;
    public Image weaponSlot3;
    public Image weaponSlot4;

    private void Update()
    {
        #region Slow Motion
        if (combatInitiated)
        {
            combatOptionsMenu.SetActive(true);

            Time.timeScale += (1/slowTime) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);

            if (Time.timeScale == 1 && combatInitiated)
            {
                combatOptionsMenu.SetActive(false);
                combatInitiated = false;
            }
        }
        #endregion
    }


    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateSlowMotion(); FindObjectOfType<WeaponControl>().combatActivated = true;

            if (FindObjectOfType<TestInventory>().weaponSlot1 == null)
                weaponSlot1.enabled = false;
            else
                weaponSlot1.enabled = true;

            if (FindObjectOfType<TestInventory>().weaponSlot2 == null)
                weaponSlot2.enabled = false;
            else
                weaponSlot2.enabled = true;

            if (FindObjectOfType<TestInventory>().weaponSlot3 == null)
                weaponSlot3.enabled = false;
            else
                weaponSlot3.enabled = true;

            if (FindObjectOfType<TestInventory>().weaponSlot4 == null)
                weaponSlot4.enabled = false;
            else
                weaponSlot4.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Reset()); FindObjectOfType<WeaponControl>().combatActivated = false;

            combatOptionsMenu.SetActive(false);
        }
    }
    #endregion
    #region Events
    void ActivateSlowMotion()
    {
        if (!combatInitiated)
        {
            Time.timeScale = slowRate;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            combatInitiated = true;
        }
        else return;
    }
    #endregion
    #region Coroutines
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(combatResetTime);

        combatInitiated = false;
        Time.timeScale = 1;
        StopCoroutine(Reset());
    }
    #endregion
}
