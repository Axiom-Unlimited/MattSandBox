using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    [Tooltip("Set true to automatically populate Player Camera, Camera Rig, and Interact Camera.")]
    public bool autoPopulateCameras = true;

    //GameObject for the interactable objects indicator that shows the player can interact
    GameObject objIndicator;

    [Tooltip("The main player camera located in the player's camera rig.")]
    public GameObject playerCamera;
    [Tooltip("The player's camera rig.")]
    public GameObject cameraRig;
    [Tooltip("The camera that will be set as active during interaction with an object.")]
    public GameObject interactCamera;
    GameObject interactObject;

    int interactType = 0;
    [Tooltip("The speed with which the interactable object is rotated.")]
    public float interactRotationRate = 1;

    Transform interactObjectLocation;
    Vector3 interactObjectStart;
    Quaternion interactObjectRotaion;

    //player/character control script
    ThirdPersonControl tpControl;

    [Tooltip("Speed with which an interactible moves into player's view when the player is interacting with it.")]
    public float interactMoveSpeed = 1f;

    bool canInteract = false;
    bool interacting = false;
    float lastTime;

    Animator animator;
    AnimationClip interactClip;
    AnimatorOverrideController animatorOverrideController;
    RuntimeAnimatorController runtimeAnimator;

    int frameCount = 0;

	// Use this for initialization
	void Start () {
        
        //get object location marker
        if (!(interactObjectLocation = this.transform.Find("InteractObjectLocation").transform))
        {
            Debug.LogWarning("InteractObjectLocation NOT found.  Make sure object is attached to player object and that the name matches the name used in 'PlayerInteract' script.");
        }

        if (!(tpControl = GetComponent<ThirdPersonControl>()))
        {
            Debug.LogWarning("ThirdPersonControl script NOT found.  Make sure script is attached to player object and that the name matches the name used in 'PlayerInteract' script.");
        }

        if (autoPopulateCameras)
        {
            //get interaction camera
            if (!(interactCamera = GameObject.Find("InteractCamera")))
            {
                Debug.LogWarning("InteractCamera NOT found.  Make sure object is attached to player object and that the name matches the name used in 'PlayerInteract' script.");
            }
            interactCamera.SetActive(false);
            //get main player camera
            if (!(playerCamera = GameObject.Find("MainPlayerCamera")))
            {
                Debug.LogWarning("MainPlayerCamera NOT found.  Make sure object is attached to player object and that the name matches the name used in 'PlayerInteract' script.");
            }
            //get full camera rig
            if (!(cameraRig = GameObject.Find("FreeLookCameraRig")))
            {
                Debug.LogWarning("FreeLookCameraRig NOT found.  Make sure object is attached to player object and that the name matches the name used in 'PlayerInteract' script.");
            }
        }

        if (!(animator = GetComponent<Animator>()))
        {
            Debug.LogWarning("No animator found on Player object.");
        }

        //create an override controller to be used when assigning the proper interaction animation clip
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
    }
	
	// Update is called once per frame
	void Update () {
        float step = interactMoveSpeed * Time.deltaTime;

		//get interact input if player is not interacting --> Start interacting
        if (canInteract && !interacting && Input.GetButtonDown("Interact"))
        {
            //lastTime lines are used to delay input being registered multiple times in one click
            lastTime = Time.time;

            interacting = true;

            tpControl.canMove = false;            

            //determine the type of interaction object
                //1: single object that is zoomed in and can be investigated by player
                //2: more elaborate interaction that plays a specific animation when activated
            if (interactType == 1)
            {
                objIndicator.SetActive(false);
            }
            else if (interactType == 2)
            {
                //animator.SetFloat("interactIndex", interactObject.GetComponent<SceneInteract>().interactIndex);

                //set the interaction clip on the player object equal to the interaction clip on the scene interact object
                interactClip = interactObject.GetComponent<SceneInteract>().interactAnim;
                                
                //set the interaction animation (motion) in the override controller to the newly assigned interaction clip
                animatorOverrideController["DefaultPickUp"] = interactClip;
                //set the controller in the animator to the override controller
                animator.runtimeAnimatorController = animatorOverrideController;
            }      
            else
            {
                Debug.LogWarning("Unknown interact type.");
            }
        }

        if (interacting)
        {
            if (interactType == 1)
            {
                //move interact object to interact locataion
                interactObject.transform.position = Vector3.MoveTowards(interactObject.transform.position, interactObjectLocation.position, step);
                //interactObject.transform.LookAt(interactCamera.transform);

                interactCamera.SetActive(true);
                cameraRig.SetActive(false);
                interactCamera.transform.LookAt(interactObject.transform);
                //rotate the object being interacted with
                interactObject.transform.Rotate(interactRotationRate * Input.GetAxis("Vertical"), interactRotationRate * Input.GetAxis("Horizontal"), 0);
            }
            else if (interactType == 2)
            {
                //run the interaction state
                animator.Play("Interacting");
                //give the animation time to play before restoring player control
                frameCount++;
                if (frameCount >= 20)
                {
                    frameCount = 0;
                    interacting = false;
                    tpControl.canMove = true;
                }
            }

        }

        //get interact input if player is interacting --> Stop interacting
        if (interacting && Input.GetButtonDown("Interact") && lastTime != Time.time)
        {
            lastTime = Time.time;
            interacting = false;

            //reset the object that was being interacted with
            if (interactType == 1)
            {
                interactObject.transform.position = interactObjectStart;
                interactObject.transform.rotation = interactObjectRotaion;

                interactCamera.SetActive(false);
                cameraRig.SetActive(true);                

                objIndicator.SetActive(true);
                
            }
            //return player control
            tpControl.canMove = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "InteractObject")
        {
            interactObject = other.gameObject;

            canInteract = true;

            if (interactObject.GetComponent<ObjectInteract>())
            {
                interactObjectStart = other.GetComponent<ObjectInteract>().startLocation;
                interactObjectRotaion = other.GetComponent<ObjectInteract>().startRotation;
                
                objIndicator = other.transform.GetChild(0).gameObject;
                objIndicator.SetActive(true);

                interactType = 1;
            }
            else if (interactObject.GetComponent<SceneInteract>())
            {
                interactClip = interactObject.GetComponent<SceneInteract>().interactAnim;

                interactType = 2;
            }                      
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "InteractObject")
        {
            canInteract = false;

            interactType = 0;

            if (other.GetComponent<ObjectInteract>())
            {
                objIndicator.SetActive(false);                
            }
        }
    }
}