using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehavior : MonoBehaviour {

    ParticleSystem ps;

    [Tooltip("True when gun is fully automatic.")]
    public bool fullAuto = false;
    [Tooltip("The force that the bullet applies to the to target on collision.")]
    public int collisionForce = 10;

    [Tooltip("The number of rounds that can be fired before reload.")]
    public int ammoCount = 6;
    [Tooltip("The wait time, in seconds, between each round being fired.")]
    public float fireTime = 0.01f;


    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var psMain = ps.main;
        var psCollision = ps.collision;

        psCollision.colliderForce = collisionForce;
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }
}
