using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsTest : MonoBehaviour {

    public int hp = 100;

    public void Update()
    {
        if (hp <= 0)
            Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
    }
}
