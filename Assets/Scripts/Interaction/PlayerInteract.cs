using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

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

    public int interactType = 0;

    Transform interactObjectLocation;
    Vector3 interactObjectStart;

    //player/character control script
    ThirdPersonControl tpControl;

    [Tooltip("Speed with which an interactible moves into player's view when the player is interacting with it.")]
    public float interactMoveSpeed = 1f;

    public bool canInteract = false;
    public bool interacting = false;
    float lastTime;

    Animator animator;
    Animation animationComp;
    public AnimationClip interactClip;

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

        if (!(animationComp = GetComponent<Animation>()))
        {
            Debug.LogWarning("No animation component found on Player object.");
        }
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

            if (interactType == 1)
            {
                objIndicator.SetActive(false);
            }
            else if (interactType == 2)
            {
                animator.SetFloat("interactIndex", interactObject.GetComponent<SceneInteract>().interactIndex);
            }            
        }

        if (interacting)
        {
            if (interactType == 1)
            {
                //move interact object to interact locataion
                interactObject.transform.position = Vector3.MoveTowards(interactObject.transform.position, interactObjectLocation.position, step);

                interactCamera.SetActive(true);
                cameraRig.SetActive(false);
                interactCamera.transform.LookAt(interactObject.transform);
            }
            else if (interactType == 2)
            {
                animator.Play("Interacting");

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

            if (interactType == 1)
            {
                interactObject.transform.position = interactObjectStart;

                interactCamera.SetActive(false);
                cameraRig.SetActive(true);                

                objIndicator.SetActive(true);
            }

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
