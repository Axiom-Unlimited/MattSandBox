using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitiateCombat ()
    {
        //replace with appropriate code when combat AI has been developed
        this.gameObject.GetComponent<Renderer>().material.color = Color.red;
        //Debug.Log("Combat Initiated", transform);
    }

    public void ExitCombat ()
    {
        //replace with appropriate code when combat AI has been developed
        this.gameObject.GetComponent<Renderer>().material.color = Color.white;
        //Debug.Log("Combat Exited", transform);
    }

    private void ObjectKilled ()
    {
        //notify "CombatArea" that this object has been killed
        //decrement CombatArea's enemiesRemaining value
    }
}
