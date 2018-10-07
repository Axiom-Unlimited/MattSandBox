using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCamera : MonoBehaviour {

    GameObject pivot;
    Camera cam;

    [HideInInspector]
    public float passiveFov;
    [HideInInspector]
    public float passiveSensitiviy;

    [Tooltip("Set FOV for the camera during combat")]
    public float activeFov = 37.5f;
    [Tooltip("Set FOV for the camera while the player is aiming")]
    public float aimFov = 20.0f;
    float fov;

    [Tooltip("Set look sensitivity while aiming")]
    public float aimSensitivty = 3.0f;
    
    [Tooltip("Set camera's offset during combat")]
    public float activeOffset = 0.5f;
    [Tooltip("Set camera's offset during normal, passive gameplay")]
    public float passiveOffset = 0f;

    [Tooltip("The step speed for the offset's transition")]
    public float offsetStepSpeed = 5;
    [Tooltip("The amount that the FOV will be incremented by when there is an FOV change")]
    public float fovStep = 2;

    [HideInInspector]
    public bool activeCam = false;
    bool aim = false;

    float targetFov;
    float targetOffset;
    GameObject freeLookRig;

    void Start () {
        cam = this.GetComponent<Camera>();
        freeLookRig = this.transform.parent.transform.parent.gameObject;
        pivot = this.transform.parent.gameObject;
        passiveFov = cam.fieldOfView;
        passiveSensitiviy = freeLookRig.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().m_TurnSpeed;
        fov = passiveFov;
	}

    private void Update()
    {
        float step = offsetStepSpeed * Time.deltaTime;
       
        //aim input detection
        if (Input.GetAxis("Aim") > 0)
        {
            aim = true;
        }
        else
        {
            aim = false;
        }

        //set target FOV and offset based on combat and aim status
        if (activeCam)
        {
            targetFov = activeFov;
            targetOffset = activeOffset;
            freeLookRig.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().m_TurnSpeed = passiveSensitiviy;
        }
        if (aim)
        {
            targetFov = aimFov;
            targetOffset = activeOffset;
            freeLookRig.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().m_TurnSpeed = aimSensitivty;
        }
        if (!activeCam && !aim)
        {
            targetFov = passiveFov;
            targetOffset = passiveOffset;
            freeLookRig.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().m_TurnSpeed = passiveSensitiviy;

        }

        //set camera's FOV
        if (fov < targetFov)
        {
            fov = Mathf.Clamp(fov - fovStep, targetFov, passiveFov);
            cam.fieldOfView = fov;
        }
        else if (fov > targetFov)
        {
            fov = Mathf.Clamp(fov + fovStep, aimFov, targetFov);
            cam.fieldOfView = fov;
        }

        //set camera's offset
        if (pivot.transform.localPosition.x < targetOffset)
        {
            pivot.transform.localPosition = Vector3.MoveTowards(pivot.transform.localPosition, new Vector3(activeOffset, pivot.transform.localPosition.y, pivot.transform.localPosition.z), step);
        }
        else if (pivot.transform.localPosition.x > targetOffset)
        {
            pivot.transform.localPosition = Vector3.MoveTowards(pivot.transform.localPosition, new Vector3(passiveOffset, pivot.transform.localPosition.y, pivot.transform.localPosition.z), step);
        }
    }

    public void ActiveCamera ()
    {
        activeCam = true;
    }

    public void PassiveCamera ()
    {
        activeCam = false;
    }
}
