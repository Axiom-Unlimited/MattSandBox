using UnityEngine;

public class ObjectInteract : MonoBehaviour {

    GameObject indicator;
    [HideInInspector]
    public Vector3 startLocation;
    [HideInInspector]
    public Quaternion startRotation;
    
	void Start () {

        if (!(indicator = this.gameObject.transform.GetChild(0).gameObject))
        {
            Debug.LogWarning(this.name + ": indicator not found.  Make sure indicator is Child(0)");
        }

        indicator.SetActive(false);

        startLocation = this.transform.position;
        startRotation = transform.rotation;
	}
}
