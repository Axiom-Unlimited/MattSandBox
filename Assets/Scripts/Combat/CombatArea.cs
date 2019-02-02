using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArea : MonoBehaviour {

    [Tooltip("When checked this object will automatically populate the 'Enemies' parent object and enemyList array.")]
    public bool autoPopulateEnemies = true;

    [Tooltip("An object that contains all of this area's enemies.  Enemies should be children of this object.")]
    public GameObject enemies;
    [Tooltip("An array filled with all the enemies that are in this combat area.")]
    public Transform[] enemyList;

    [Tooltip("The number of enemies left.  Only visible in inspector for debug purposes, change to 'HideInInspector when done.")]
    public int enemiesRemaining;

    private void Start()
    {
        if (autoPopulateEnemies)
        {
            //Find "Enemy" opbject
            if (!(enemies = GameObject.Find("Enemies")))
            {
                Debug.LogWarning("Enemies not found for CombatArea.  Make sure this CombatArea has an object named 'Enemies' or populate enemies manually.", transform);
            }
            //fill enemyList with Enemies' children
            enemyList = enemies.GetComponentsInChildren<Transform>();
            //get accurate enemy count
            enemiesRemaining = enemyList.Length - 1; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //when player enters this combat area each enemy in the enemyList is told to initiate combat
        if (other.tag == "Player")
        {
            other.GetComponent<FireGun>().DrawGun();

            foreach (Transform enemy in enemyList)
            {
                if (enemy.tag == "Enemy")
                {
                    enemy.GetComponent<CombatBehavior>().InitiateCombat();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //when player exits combat area, tell each enemy in the area to exit combat
        if (other.tag == "Player")
        {
            other.GetComponent<FireGun>().HolsterGun();

            foreach (Transform enemy in enemyList)
            {
                if (enemy.tag == "Enemy")
                {
                    enemy.GetComponent<CombatBehavior>().ExitCombat();
                }
            }
        }
    }

    public void DecrementEnemiesRemaining()
    {
        if (enemiesRemaining > 0)
        {
            enemiesRemaining--;
        }
        else
        {
            //all enemies killed, end combat for player
        }
    }

}
