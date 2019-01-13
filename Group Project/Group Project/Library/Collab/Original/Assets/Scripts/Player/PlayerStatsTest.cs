using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsTest : MonoBehaviour {

    public int hp = 100;

    public void TakeDamage(int amount)
    {
        hp -= amount;
    }
}
