using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Original script: "ThirdPersonUserControl" from Unity Standard Assets

//change required component if we switch from using Unity's Third Person Character script
[RequireComponent(typeof(UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter))]

public class ThirdPersonControl : MonoBehaviour {

    [Tooltip("If checked the player character will walk by default and sprint when sprint button is held.")]
    public bool autoWalk = true;
    [Tooltip("If checked the player character will be able to jump.")]
    public bool canJump = false;
    [Tooltip("If checked the player character will stay crouched after the button is pressed and stand when pressed again.")]
    public bool toggleCrouch = true;

    [HideInInspector]
    public bool canMove = true;

    //change required component if we switch from using Unity's Third Person Character script
    private UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    //[HideInInspector]
    public Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

    float h;
    float v;
    bool crouch;

    Animator animator;

    int layerMask = 1 << 8;
    [Tooltip("The height (from floor) of the ray that will check for obstacles to potentially vault over.")]
    public float vaultHeight = 1f;
    [Tooltip("The length of the ray (distance from player) that will check for obstacles to potentially vault over.")]
    public float vaultDistance = 1f;
    [Tooltip("The height (from vaultHeight) that will check to ensure the obstacle is short enough to vault over.")]
    public float vaultHeightLimit = 0.5f;
    [Tooltip("The maximum depth an object can be to still be vaulted over.")]
    public float vaultDepthLimit = 1.5f;

    public bool obstacle = false;
    public bool tooHigh = false;
    public bool canVault;

    RaycastHit vaultCheckHit;
    RaycastHit vaultLandHit;
    Vector3 vaultRayOrigin;
    Vector3 vaultRayTarget;
    Vector3 vaultLandingPoint;
    
    public int frameCount = 0;
    public bool vaulting;

    public float coverAngle;
    public bool inCover = false;
    public float coverBreakAngle = 20f;
    public Vector3 hitNormal;
    public Vector3 hitPerp;
    public Vector3 coverMove;
    
    public bool coverCheck = false;


    private void Start()
    {
        animator = GetComponent<Animator>();

        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        //get the third person character ( this should never be null due to require component )
        //change required component if we switch from using Unity's Third Person Character script
        m_Character = GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();

        layerMask = ~layerMask;
    }


    private void Update()
    {
        //if (canJump)
        //{
        //    if (!m_Jump)
        //    {
        //        m_Jump = Input.GetButtonDown("Jump");
        //    }
        //}   

        //the vector point on the player that acts as the origin for the obstacle detection ray
        vaultRayOrigin = transform.position + new Vector3(0f, vaultHeight, 0f);
        //the vector point that acts as the targeted end point of the obstacle detection ray
        vaultRayTarget = vaultRayOrigin + new Vector3(transform.forward.x * vaultDistance, transform.forward.y, transform.forward.z * vaultDistance);

        //draw ray for obstacle detection
        Debug.DrawLine(vaultRayOrigin, vaultRayTarget, Color.green);
        //cast ray checking for obstacles
        if (Physics.Raycast(vaultRayOrigin, new Vector3(transform.forward.x * vaultDistance, transform.forward.y, 
                            transform.forward.z * vaultDistance), 
                            out vaultCheckHit, Vector3.Distance(vaultRayOrigin, vaultRayTarget), layerMask))
        {
            Debug.DrawRay(vaultCheckHit.point, vaultCheckHit.normal, Color.red);
            obstacle = true;


            hitNormal = vaultCheckHit.normal;
            hitPerp = Vector3.Cross(hitNormal, Vector3.up);

            //draw ray for obstacle height detection
            Debug.DrawLine(vaultRayOrigin + new Vector3(0f, vaultHeightLimit, 0f), vaultRayTarget + new Vector3(0f, vaultHeightLimit, 0f), Color.green);
            //cast ray checking for vaultable objects
            //if this ray hits then there isn't enough "head room" to execute a vault
            if (Physics.Raycast(vaultRayOrigin + new Vector3(0f, vaultHeightLimit, 0f),
                                new Vector3(transform.forward.x * vaultDistance, transform.forward.y, transform.forward.z * vaultDistance),
                                Vector3.Distance(vaultRayOrigin, vaultRayTarget), layerMask))
            {
                tooHigh = true;
            }
            else
            {
                tooHigh = false;

                //the following rays are used to check the other side of the object that will be vaulted, used to make sure it is safe on the other side

                //ray from the point of collision between the ray and obstacle and the height limit defined by vaultHeightLimit
                Debug.DrawRay(vaultCheckHit.point, new Vector3 (0f, vaultHeightLimit, 0f));
                if (Physics.Raycast(vaultCheckHit.point, new Vector3(0f, vaultHeightLimit, 0f), vaultHeightLimit, layerMask))
                {
                    Debug.Log("Hit 1");
                    tooHigh = true;
                }

                //ray from height limit at the front of the obstacle to the depth specified by vaultDepthLimit
                Debug.DrawRay(vaultCheckHit.point + new Vector3(0f, vaultHeightLimit, 0f), Vector3.ClampMagnitude(-vaultCheckHit.normal * 10, vaultDepthLimit));
                if (Physics.Raycast(vaultCheckHit.point + new Vector3(0f, vaultHeightLimit, 0f), Vector3.ClampMagnitude(-vaultCheckHit.normal * 10, vaultDepthLimit),
                    vaultDepthLimit, layerMask))
                {
                    Debug.Log("Hit 2");
                    tooHigh = true;
                }

                //rays from vaultDepthLimit to check for the floor on the other side of the obstacle

                //ray that checks to see if the the floor on the other side is too high for vaulting is appropriate (ex. a climbing up animation would be more appropriate)
                Debug.DrawRay(vaultCheckHit.point + new Vector3(0f, vaultHeightLimit, 0f) + Vector3.ClampMagnitude(-vaultCheckHit.normal * 10, vaultDepthLimit),
                              new Vector3(0f, -(vaultHeight + vaultHeightLimit), 0f));
                if (Physics.Raycast(vaultCheckHit.point + new Vector3(0f, vaultHeightLimit, 0f) + Vector3.ClampMagnitude(-vaultCheckHit.normal * 10, vaultDepthLimit),
                    new Vector3(0f, -(vaultHeight + vaultHeightLimit), 0f), out vaultLandHit, vaultHeight + vaultHeightLimit - 0.5f, layerMask))
                {
                    //Debug.Log("Other side too high");
                    tooHigh = true;
                }
                //ray that checks to see if the floor is the same height as the floor that the player is currently on
                else if (Physics.Raycast(vaultCheckHit.point + new Vector3(0f, vaultHeightLimit, 0f) + Vector3.ClampMagnitude(-vaultCheckHit.normal * 10, vaultDepthLimit),
                    new Vector3(0f, -(vaultHeight + vaultHeightLimit), 0f), out vaultLandHit, vaultHeight + vaultHeightLimit + 0.001f, layerMask))
                {
                    //Debug.Log("Other side right height");
                    tooHigh = false;
                }
                //if all else fails then the floor on the other side is too low for a safe vault
                else
                {
                    //Debug.Log("Other side too low");
                    tooHigh = true;
                }
            }
        }
        else
        {
            obstacle = false;
        }

        //allow vaulting if all requirements are met
        if (obstacle && !tooHigh)
        {
            canVault = true;
        }
        else
        {
            canVault = false;
        }

        if (crouch && obstacle)
        {
            inCover = true;            
        }
        else if (!crouch)
        {
            inCover = false;
        }

        if (inCover)
        {
            //cast a ray while in cover to check if the player is still behind cover or has walked out
            Debug.DrawRay(vaultRayOrigin, -hitNormal + new Vector3(transform.forward.x * vaultDistance, transform.forward.y,
                            transform.forward.z * vaultDistance), Color.cyan);
            if (Physics.Raycast(vaultRayOrigin, -hitNormal + new Vector3(transform.forward.x * vaultDistance, transform.forward.y,
                            transform.forward.z * vaultDistance), Vector3.Distance(vaultRayOrigin, vaultRayTarget), layerMask)) coverCheck = true;
            else coverCheck = false;

            //if cover check fails then kick player out of cover mode
            if (coverCheck)
            {
                inCover = true;
            }
            else
            {
                inCover = false;
            }
        }
        
        //calculate angle between player input and the direction the player character is facing
        //if (inCover)
        coverAngle = Vector3.SignedAngle(m_Move, hitPerp, Vector3.up);
        
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (canMove)
        {
            // read inputs
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            //handle crouch based on toggleCrouch choice
            if (toggleCrouch)
            {
                if (Input.GetButtonDown("Crouch"))
                {
                    crouch = !crouch;
                }
            }
            else
            {
                crouch = Input.GetButton("Crouch");
            }

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;

                //player input direction
                Debug.DrawRay(transform.position, m_Move, Color.yellow);
                //normal of wall direction
                if (obstacle) Debug.DrawRay(transform.position, vaultCheckHit.normal, Color.red);
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }

            // walk speed multiplier
            if (autoWalk)
            {
                if (!Input.GetButton("Sprint")) m_Move *= 0.5f;
            }
            else
            {
                if (Input.GetButton("Sprint")) m_Move *= 0.5f;
            }

            if (inCover && crouch)
            {
                //when player input is at a certain angle with respect to the character's direction cover will be broken and normal input returned
                if (coverAngle < -90 + coverBreakAngle && coverAngle > -90 - coverBreakAngle)
                {
                    inCover = false;
                }



                //calculate vector to replace normal movement vector
                //scales a vector that is perpendicular to the cover's normal based in input

                //---------------Convert hitPerp to be world relative---------------//

                Debug.DrawRay(transform.position, hitPerp, Color.black);
                coverMove = new Vector3(Mathf.Abs(hitPerp.x) * m_Move.x, m_Move.y, Mathf.Abs(hitPerp.z) * m_Move.z);

                

                // pass all parameters to the character control script
                m_Character.Move(coverMove, crouch, m_Jump);
                m_Jump = false;
            }
            else
            {
                // pass all parameters to the character control script
                m_Character.Move(m_Move, crouch, m_Jump);
                m_Jump = false;
            }
            
        }
        //disables player movement and sets the character movement to stop
        else
        {
            m_Character.Move(Vector3.zero, false, false);
        }

        //check for vaulting ability and input
        if (canVault && Input.GetButtonDown("Jump"))
        {
            vaulting = true;
        }
        //execute vault
        if (vaulting)
        {
            //adjust capsule collider height and gravity to allow the character to pass over the obstacle
            //hopefully this will need to be changed (better yet removed) whenever a more final model is being used
            //more accurate colliders on the character will also (hopefully) remove the need to resize the colliders to avoid the obstacle
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<CapsuleCollider>().height = 0.7f;
            GetComponent<CapsuleCollider>().center = new Vector3(0f, 1.3f, 0f);

            //play the Vaulting state in the animator (functionally speaking, play vault animation)
            animator.Play("Vaulting");

            //count frames and reset all the changed values after the obstacle has been cleared
            frameCount++;
            if (frameCount >= 20)
            {
                GetComponent<CapsuleCollider>().height = 1.6f;
                GetComponent<CapsuleCollider>().center = new Vector3(0f, 0.8f, 0f);
                GetComponent<Rigidbody>().useGravity = true;
                frameCount = 0;
                vaulting = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //draw a wire sphere where obstacle detection ray starts
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(vaultRayOrigin, 0.03f);

        if (obstacle)
        {
            //if obstacle is detected draw wire sphere where the height check ray starts
            Gizmos.DrawWireSphere(vaultRayOrigin + new Vector3(0f, vaultHeightLimit, 0f), 0.03f);

            Gizmos.color = Color.green;
            //draw wire spheres at the end of the obstacle and and height check rays
            Gizmos.DrawWireSphere(vaultRayTarget, 0.03f);
            Gizmos.DrawWireSphere(vaultRayTarget + new Vector3(0f, vaultHeightLimit, 0f), 0.03f);

            Gizmos.color = Color.white;
            //draw wire sphere at the end point of the floor check rays
            Gizmos.DrawWireSphere(vaultLandHit.point, 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + vaultCheckHit.normal, 0.05f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + m_Move, 0.05f);
        }

    }
}
