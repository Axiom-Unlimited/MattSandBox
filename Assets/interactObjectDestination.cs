using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactObjectDestination : MonoBehaviour {

    public Vector3 lossScale;

	// Use this for initialization
	void Start () {
        lossScale = transform.lossyScale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
