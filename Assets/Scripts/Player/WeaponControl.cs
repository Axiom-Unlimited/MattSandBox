using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    public bool combatActivated;
    TestInventory playerInventory;

    private void LateUpdate()
    {
        if (!combatActivated)
            return;

        #region Activating Weapons
        if (Input.GetKeyDown(KeyCode.Alpha1) && FindObjectOfType<TestInventory>().weaponSlot1 != null)
            Debug.Log("Activate Weapon 1 and Animator Change");
        if (Input.GetKeyDown(KeyCode.Alpha2) && FindObjectOfType<TestInventory>().weaponSlot2 != null)
            Debug.Log("Activate Weapon 2 and Animator Change");
        if (Input.GetKeyDown(KeyCode.Alpha3) && FindObjectOfType<TestInventory>().weaponSlot3 != null)
            Debug.Log("Activate Weapon 3 and Animator Change");
        if (Input.GetKeyDown(KeyCode.Alpha4) && FindObjectOfType<TestInventory>().weaponSlot4 != null)
            Debug.Log("Activate Weapon 4 and Animator Change");

        #endregion
    }

}
