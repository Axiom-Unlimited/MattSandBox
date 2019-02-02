using UnityEngine;
using UnityEngine.UI;

public class AlphaButton : MonoBehaviour {

    public float alphaThreshold = 0.1f;

	void Start () {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
	}
	
}
