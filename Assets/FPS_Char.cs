using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]

public class FPS_Char : MonoBehaviour {

    [Header("Player Settings")]
    public float Cur_Speed;
    public float Walk_Speed = 6;
    public float Run_Speed = 12;
    [Space(10)]
    public bool isGrounded;
    public float Jump_Height = 12;
    [Space(10)]
    [Header("Camera Settings")]
    public Camera PlayerCamera;
    private float mouseX, mouseY;
    public float Look_Sensitivity = 100;
    public float Turn_Sensitivity = 100;
    public float Look_Min = -60;
    public float Look_Max = 80;
    Rigidbody rb;

    void OnValidate()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        //gameObject.tag = "Player";
        if(PlayerCamera == null)
        {
            PlayerCamera = Camera.main;
        }
    }

    void Awake()
    {
        PlayerCamera.transform.position = transform.position + transform.up * 0.75f;
        PlayerCamera.transform.parent = transform;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 60), "Press Escape to Use your Mouse");
        GUI.Label(new Rect(10, 25, 300, 60), "(Only While Testing)");
    }

    // Use this for initialization
    void Start () {;
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        //Set the Gravity in the Scene
        Physics.gravity = new Vector3(0, -40, 0);
        //Lock Mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {

        //Toggle Run/Walk
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Cur_Speed = Run_Speed;
        }
        else
        {
            Cur_Speed = Walk_Speed;
        }

        //Activate Jump
        if(isGrounded && Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(transform.up * Jump_Height, ForceMode.Impulse);
            isGrounded = false;
        }

        //Input Settings
        var x = Input.GetAxis("Horizontal") * Cur_Speed * Time.deltaTime;
        var z = Input.GetAxis("Vertical") * Cur_Speed * Time.deltaTime;
        mouseX -= Input.GetAxis("Mouse Y") * Look_Sensitivity * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse X") * Turn_Sensitivity * Time.deltaTime;
        mouseX = Mathf.Clamp(mouseX, Look_Min, Look_Max);

        //Movement Controls
        transform.Translate(x, 0, z);
        transform.rotation = Quaternion.Euler(0, mouseY, 0);
        PlayerCamera.transform.rotation = Quaternion.Euler(mouseX, mouseY, 0);

	}

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}
