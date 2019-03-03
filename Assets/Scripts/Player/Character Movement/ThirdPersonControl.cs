using UnityEngine;

//Original script: "ThirdPersonUserControl" from Unity Standard Assets

[RequireComponent(typeof(PlayerCharacter))]
[RequireComponent(typeof(Animator))]

public class ThirdPersonControl : MonoBehaviour {
    #region Variables

    #region Basic Control Options
    [Header("Player Control Options")]
    [Tooltip("If checked the player character will walk by default and sprint when sprint button is held.")]
    public bool autoWalk = true;
    [Tooltip("If checked the player character will be able to jump.")]
    public bool canJump = false;
    [Tooltip("If checked the player character will stay crouched after the button is pressed and stand when pressed again.")]
    public bool toggleCrouch = true;
    [HideInInspector]
    public bool canMove = true;
    bool camRelative;
    #endregion

    #region Player and Camera Related Dependencies
    private PlayerCharacter playerCharacter;
    Animator animator;
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    [HideInInspector]
    public Vector3 m_Move;                    // the world-relative desired move direction, calculated from the camForward and user input.
    private bool m_Jump;
    #endregion

    #region Movement Variables
    float h;
    float v;
    bool crouch;
    #endregion

    #region Vaulting Related Variables
    //layerMask for vaulting check set to detect only layer 8 (Player).  It is then reversed in Start()
    int layerMask = 1 << 8;
    [Header("Vault Check Values")]
    [Tooltip("The height (from floor) of the ray that will check for obstacles to potentially vault over.")]
    public float vaultHeight = 1f;
    [Tooltip("The length of the ray (distance from player) that will check for obstacles to potentially vault over.")]
    public float vaultDistance = 1f;
    [Tooltip("The height (from vaultHeight) that will check to ensure the obstacle is short enough to vault over.")]
    public float vaultHeightLimit = 0.5f;
    [Tooltip("The maximum depth an object can be to still be vaulted over.")]
    public float vaultDepthLimit = 1.5f;

    bool obstacle = false;
    bool tooHigh = false;
    bool canVault;    

    RaycastHit vaultCheckHit;
    RaycastHit vaultLandHit;
    Vector3 vaultRayOrigin;
    Vector3 vaultRayTarget;
    Vector3 vaultLandingPoint;
    
    int frameCount = 0;
    bool vaulting;
    #endregion

    #region Cover Related Variables
    Vector3 hitNormal;
    Vector3 hitPerp;
    Vector3 coverMove;
    Vector3 coverStart;
    bool startCover;
    bool coverObstacle;
    Vector3 coverObstacleTarget;

    //variables used when calculating the movement vector while in cover
    float coverZ;
    float quad;
    float xScale;
    float zScale;        
    
    [Header("Cover Check Values")]
    [Tooltip("The scaling value for the cover check rays.  The rays are used while in cover to detect when the player leaves cover.  Smaller value means a narrower angle between the rays.")]
    public float coverCheckScale = 30f;
    Vector3 coverCheckTargetLeft;
    Vector3 coverCheckTargetRight;
    //layerMask for cover check set to detect only layer 9 (Cover)
    int coverLayerMask = 1 << 9;
    bool turnLeft;
    float coverCamAngle;
    bool coverCheck = false;

    public bool inCover = false;
    #endregion    

    [Space]
    [Tooltip("Check to draw debug gizmos in OnDrawGizmos.")]
    public bool drawGizmos;

    #endregion

    private void Start()
    {      
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
        }

        animator = GetComponent<Animator>();
        playerCharacter = GetComponent<PlayerCharacter>();

        //inverts the bit mask so that everything but the layer specified in the layerMask decleration will be detected by the layerMask
        layerMask = ~layerMask;
    }


    private void Update()
    {
        #region Jump Input
        //if (canJump)
        //{
        //    if (!m_Jump)
        //    {
        //        m_Jump = Input.GetButtonDown("Jump");
        //    }
        //}   
        #endregion

        #region Crouch Input
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
        #endregion

        #region Vault Check
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
                    //Debug.Log("Hit 1");
                    tooHigh = true;
                }

                //ray from height limit at the front of the obstacle to the depth specified by vaultDepthLimit
                Debug.DrawRay(vaultCheckHit.point + new Vector3(0f, vaultHeightLimit, 0f), Vector3.ClampMagnitude(-vaultCheckHit.normal * 10, vaultDepthLimit));
                if (Physics.Raycast(vaultCheckHit.point + new Vector3(0f, vaultHeightLimit, 0f), Vector3.ClampMagnitude(-vaultCheckHit.normal * 10, vaultDepthLimit),
                    vaultDepthLimit, layerMask))
                {
                    //Debug.Log("Hit 2");
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
        #endregion

        #region Cover Check
        //initiate cover if checks pass and player crouches
        if (crouch && obstacle)
        {
            inCover = true;
        }
        else if (!crouch)
        {
            inCover = false;
        }              
        
        //cover checks while already in cover
        if (inCover)
        {
            //cast rays to the left and right of player that will check for cover objects while they are in cover
            coverCheckTargetLeft = vaultRayOrigin + transform.forward + (transform.right * -coverCheckScale);
            coverCheckTargetRight = vaultRayOrigin + transform.forward + (transform.right * coverCheckScale);
            Debug.DrawRay(vaultRayOrigin, coverCheckTargetLeft - vaultRayOrigin, Color.cyan);
            Debug.DrawRay(vaultRayOrigin, coverCheckTargetRight - vaultRayOrigin, Color.cyan);
            
            if (Physics.Raycast(vaultRayOrigin, coverCheckTargetLeft - vaultRayOrigin, Vector3.Distance(vaultRayOrigin, coverCheckTargetLeft), coverLayerMask))
                coverCheck = true;
            else if (Physics.Raycast(vaultRayOrigin, coverCheckTargetRight - vaultRayOrigin, Vector3.Distance(vaultRayOrigin, coverCheckTargetRight), coverLayerMask))
                coverCheck = true;
            else
                coverCheck = false;

            //if cover check fails then kick player out of cover mode
            if (coverCheck)
            {
                inCover = true;
            }
            else if (!coverCheck && !coverObstacle)
            {
                inCover = false;
            }
        }
        #endregion
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        #region Movement Input and Calculation
        if (canMove)
        {
            // read inputs
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;

            #region Movement Calculation
            // calculate move direction to pass to character
            if (m_Cam != null && camRelative)
            {
                // calculate camera relative direction to move:                
                m_Move = v * m_CamForward + h * m_Cam.right;                
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }

            //player input direction
            Debug.DrawRay(transform.position, m_Move, Color.yellow);
            //normal of wall direction
            if (obstacle) Debug.DrawRay(transform.position, vaultCheckHit.normal, Color.red);

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
                coverCamAngle = Vector3.Angle(m_CamForward, hitPerp);

                if (coverCamAngle > 90 && coverCamAngle < 270)
                {
                    turnLeft = true;
                    playerCharacter.coverPerp = -hitPerp;
                }
                else
                {
                    turnLeft = false;
                    playerCharacter.coverPerp = hitPerp;
                }

                //using the angle between the camera and the cover's perpendicular vector calculate the appropriate weights to be applied to each input value
                quad = Mathf.Floor(coverCamAngle / 90);
                if (quad < 1)
                {
                    xScale = (coverCamAngle / 90) - quad;
                    zScale = 1 - xScale;
                }
                else
                {
                    zScale = (coverCamAngle / 90) - quad;
                    xScale = 1 - zScale;
                }
                
                //combine the input values into one number (coverZ) to be used to move the character forwards or backwards
                coverZ = (m_Move.x * xScale) * Mathf.Sign(m_Move.x) + (m_Move.z * zScale);

                coverMove = new Vector3(0f, m_Move.y, coverZ);

                // pass all parameters to the character control script
                playerCharacter.Move(coverMove, crouch, inCover);
                m_Jump = false;
            }
            else
            {
                // pass all parameters to the character control script
                playerCharacter.Move(m_Move, crouch, inCover);
                m_Jump = false;
            }
            #endregion
        }
        //disables player movement and sets the character movement to stop
        else
        {
            playerCharacter.Move(Vector3.zero, false, false);
        }
        #endregion

        #region Vaulting Input and Execution
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
        #endregion
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            //norm of obstacle
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitNormal, 0.1f);
            //perpendicular to norm
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(hitPerp, 0.11f);
            //m_Move
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(m_Move, 0.1f);
            //coverMove
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(coverMove, 0.12f);
            //cam forward
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(m_CamForward, 0.11f);
        }
    }
}
