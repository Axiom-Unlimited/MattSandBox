using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGun : MonoBehaviour {

    GameObject playerCamera;

    public GameObject gun;
    public GameObject target;

    public GameObject gunPsObj;
    [HideInInspector]
    public ParticleSystem gunPS;

    bool fullAuto = false;
    int ammoCount;
    int shotsFired;
    float fireTime;
    float fireWaitTime;

    bool fire = false;
    bool shooting = false;
    bool canShoot = true;
    public bool inCombat = false;
    public bool reloadOn = true;

    public bool drawGizmos = true;

	// Use this for initialization
	void Start () {
        if (!(playerCamera = GameObject.Find("MainPlayerCamera")))
        {
            Debug.LogWarning("MainPlayerCamera NOT found.  Make sure object is attached to player object and that the name matches the name used in 'FireGun' script.");
        }

        gunPS = gunPsObj.GetComponent<ParticleSystem>();
        fullAuto = gunPsObj.GetComponent<GunBehavior>().fullAuto;
        ammoCount = gunPsObj.GetComponent<GunBehavior>().ammoCount;
        fireTime = gunPsObj.GetComponent<GunBehavior>().fireTime;
    }
	
	// Update is called once per frame
	void Update () {
        //check amount of bullets fired against the max amount of bullets that can be fired before reload
        if (shotsFired <= ammoCount || !reloadOn)
        {
            //"Fire" is used for button based input (mouse click)
            //"FireAxis" is used for axis based input (controller triggers)

            //fully automatic input
            if (fullAuto)
            {
                //M&KB
                if (Input.GetAxis("Fire") > 0 && inCombat && !shooting)
                {
                    fire = true;
                    shooting = true;
                }
                //Controller
                if (Input.GetAxis("FireAxis") > 0 && inCombat && !shooting)
                {
                    fire = true;
                    shooting = true;
                }
            }

            //semi automatice input
            else
            {
                //M&KB
                if (Input.GetButtonDown("Fire") && inCombat && canShoot)
                {
                    fire = true;
                }
                //Controller
                if (Input.GetAxis("FireAxis") > 0 && inCombat && !shooting && canShoot)
                {
                    fire = true;
                    shooting = true;
                }
                if (Input.GetAxis("FireAxis") <= 0)
                {
                    shooting = false;
                }
            }
        }
        //stop shooting when ammo count reaches 0 for full auto weapons 
        else if (shotsFired > ammoCount && fullAuto)
        {
            gunPS.Stop();
            shooting = false;
        }

        //input is released, stop firing
        if (Input.GetAxis("Fire") <= 0 && Input.GetAxis("FireAxis") <= 0 && fullAuto)
        {
            gunPS.Stop();
            shooting = false;
            fire = false;
        }

        //reload/reset shots fired to 0 
        if (Input.GetKeyDown(KeyCode.R))
        {
            shotsFired = 0;
        }
    }

    private void FixedUpdate()
    {
        //count the time until the weapon's fire rate lets it shoot again 
        if (!canShoot)
        {
            fireWaitTime += Time.deltaTime;
            if (fireWaitTime >= fireTime)
            {
                canShoot = true;
                fireWaitTime = 0;
            }
        }

        //fire gun 
        if (fire && canShoot && shotsFired <= ammoCount)
        {
            canShoot = false;
            gunPS.Play();
            shotsFired++;
            Debug.Log("Firing");
            if (!fullAuto) fire = false;
        }
    }

    public void DrawGun ()
    {
        inCombat = true;

        playerCamera.GetComponent<CombatCamera>().activeCam = true;

        gun.SetActive(true);
        target.SetActive(true);
    }

    public void HolsterGun ()
    {
        inCombat = false;

        playerCamera.GetComponent<CombatCamera>().activeCam = false;

        gun.SetActive(false);
        target.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gun.transform.position, target.transform.position);
        }
    }
}
