using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAim : MonoBehaviour {

    //Camera cam;
    RaycastHit hit;
    int layerMask = 1 << 8;

    public bool rayHit = false;
    public GameObject target;
    public GameObject gun;

    public bool drawGizmos = true;

	// Use this for initialization
	void Start () {
        //if (!(cam = this.GetComponent<Camera>()))
        //{
        //    Debug.LogError("Main camera not found in 'CameraAim' script");
        //}
        //else
        //{
        //    target.transform.position = cam.transform.position;
        //}
        
        layerMask = ~layerMask;
	}
	
	// Update is called once per frame
	void Update () {

		if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            target.transform.position = hit.point;
            rayHit = true;
            gun.transform.LookAt(target.transform);
        }
        else
        {
            rayHit = false;
            target.transform.position = this.transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.transform.position, target.transform.position);
        }
    }
}
