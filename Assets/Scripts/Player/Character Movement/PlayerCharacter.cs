using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

public class PlayerCharacter : MonoBehaviour {

    #region Variables

    #region Player Compontents
    Rigidbody rb;

    CapsuleCollider capsule;
    float capsuleHeight;
    Vector3 capsuleCenter;

    Transform cam;
    Vector3 camForward;
    float camAngle;

    Animator animator;
    #endregion

    //dampTime to be used when setting the float values for the movement animations.  
    //acts as a value to smooth the setting of the float value similar to a Lerp
    float animDampTime = 0.1f;

    #region Movement
    //variables to be used when setting the float values for the animator
    float moveX;
    float moveY;

    Vector3 lookVect;
    Vector3 targetDirection;
    Quaternion lookRotation;
    [Tooltip("The speed at which the player character will rotate to face the same direction as the camera.")]
    public float rotationSpeed = 10f;

    bool crouched;
    
    [HideInInspector]
    public Vector3 coverPerp;
    #endregion

    #endregion

    void Start () {

        //check for and get main camera
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\".", gameObject);
        }

        rb = GetComponent <Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponent<Animator>();

        capsule = GetComponent<CapsuleCollider>();
        capsuleCenter = capsule.center;
        capsuleHeight = capsule.height;
	}

    /// <summary> Pass in a Vector3 (move) that will be used to set proper parameter values in the standard movement blend tree.
    /// <para> The "move" vector used so that moveX and moveY in the animator will be assigned as: moveX = move.x and moveY = move.z.</para>
    /// </summary>
    public void Move(Vector3 move, bool crouch, bool inCover)
    {
        #region Face Forward
        if (!inCover)
        {
            //get angle between player character's forward and the camera's forward
            camAngle = Vector3.SignedAngle(transform.forward, cam.forward, Vector3.up);
            //vector going from player towards camera's forward
            lookVect = new Vector3(transform.position.x + cam.forward.x, transform.position.y, transform.position.z + cam.forward.z);
            //the target direction for the player to rotate towards
            targetDirection = (lookVect - transform.position).normalized;
            //quaternion representing the rotation to the targetDirection
            lookRotation = Quaternion.LookRotation(targetDirection);
        }
        else if (inCover)
        {
            //make the target direction the inverse of the cover's normal so that the player is facing the cover object
            targetDirection = coverPerp;
            lookRotation = Quaternion.LookRotation(targetDirection);
        }

        //check for input and rotate when player tries to move
        if (move.x != 0 || move.z != 0)
        {
                //smoothly slerp from current rotation to target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            
        }
        #endregion

        #region Handle Movement
        //normalize the movement vector so that it doesn't exceed animator parameter requirements
        if (move.magnitude > 1f) move.Normalize();

        //the movement vector's (x,z) are assigned to the Cartesian plane used in the animator blend tree (x,y)
        moveX = move.x;
        moveY = move.z;
        //set animator (x,y) values so that the correct animation is played from the blend tree
        animator.SetFloat("moveX", moveX, animDampTime, Time.deltaTime);
        animator.SetFloat("moveY", moveY, animDampTime, Time.deltaTime);
        #endregion

        #region Handle Crouch
        if (crouch)
        {
            if (crouched) return;
            animator.SetBool("crouched", crouch);
            
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;

            crouched = true;
        }
        else
        {
            animator.SetBool("crouched", crouch);
                        
            capsule.height = capsuleHeight;
            capsule.center = capsuleCenter;

            crouched = false;
        }
        #endregion
    }

    //called after every animator state check
    public void OnAnimatorMove()
    {
        //new Vector3 to be used to assign this objects velocity
        //animator.deltaPosition gives us the change in position based on the animations being played
        Vector3 rbVelocity = (animator.deltaPosition) / Time.deltaTime;

        //get objects current velocity from y because the y is uneffected from player's input
        rbVelocity.y = rb.velocity.y;
        //set object's velocity using the values given by the animator
        rb.velocity = rbVelocity;
    }
}
