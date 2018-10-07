using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {
    public Transform Target;

    private void FixedUpdate()
    {
        transform.LookAt(Target);
    }
}
