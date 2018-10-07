using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public bool isFalling;
    [Space]

    public bool canJump;
    public float jumpHeight;
    [Space]

    public float walkSpeed;
    public float runSpeed;
    private float speed;
    [Space]

    Rigidbody rb;

    public Animator anim;
    [Header("Set in Animator Parameters")]
    public string animIsRunning;
    public string animSpeed;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        Actions();
        
    }

    void Actions()
    {
        #region Sprint
        if (Input.GetButton("Sprint"))
            speed = runSpeed;
        else
            speed = walkSpeed;
        #endregion
        #region Jump
        if (!canJump)
            return;
        else if (Input.GetButton("Jump") && !isFalling)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode.Force);
            isFalling = true;
        }
        #endregion
        #region Animations
        if(speed == walkSpeed)
            anim.SetBool(animIsRunning, false); //Set isRunning = false;
        else if(!anim.GetBool(animIsRunning))
            anim.SetBool(animIsRunning, true); //Set isRunning = true;

        anim.SetFloat(animSpeed, speed); //Set Speed of Character
        #endregion
    }
}
