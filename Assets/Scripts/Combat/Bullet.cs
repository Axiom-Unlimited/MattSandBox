using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int baseDamage;
    private int adjustedDamage;

    private void OnTriggerEnter(Collider other)
    {
        #region Damage
        if (other.CompareTag("Head"))
        {
            adjustedDamage = baseDamage * 4;
            other.GetComponentInParent<PlayerStatsTest>().TakeDamage(adjustedDamage);
            Debug.Log("Damage Dealt: " + adjustedDamage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Body"))
        {
            adjustedDamage = baseDamage * 2;
            other.GetComponentInParent<PlayerStatsTest>().TakeDamage(adjustedDamage);
            Debug.Log("Damage Dealt: " + adjustedDamage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Arms") || other.CompareTag("Legs"))
        {
            adjustedDamage = baseDamage * 1;
            other.GetComponentInParent<PlayerStatsTest>().TakeDamage(adjustedDamage);
            Debug.Log("Damage Dealt: " + adjustedDamage);
            Destroy(gameObject);
        }
        #endregion

        if (gameObject.CompareTag("Bullet"))
            return;

        Destroy(gameObject);
    }
}
