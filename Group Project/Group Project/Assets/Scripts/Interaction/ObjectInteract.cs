using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteract : MonoBehaviour {

    GameObject indicator;
    [HideInInspector]
    public Vector3 startLocation;

	// Use this for initialization
	void Start () {

        if (!(indicator = this.gameObject.transform.GetChild(0).gameObject))
        {
            Debug.LogWarning(this.name + ": indicator not found.  Make sure indicator is Child(0)");
        }

        indicator.SetActive(false);

        startLocation = this.transform.position;
	}
}
