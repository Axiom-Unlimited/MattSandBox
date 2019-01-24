using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBuster : MonoBehaviour {

    public float sensitivity;
    private float mouseX, mouseY;
    public GameObject bullet;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update () {

        //Look
        mouseY += Input.GetAxis("Mouse X") * sensitivity;
        mouseX -= Input.GetAxis("Mouse Y") * sensitivity;
        mouseX = Mathf.Clamp(mouseX, -65, 80);
        transform.rotation = Quaternion.Euler(mouseX, mouseY, 0);

        //Fire
        if (Input.GetMouseButtonDown(0))
        {
            GameObject spawn = Instantiate(bullet) as GameObject;
            spawn.transform.position = transform.position;
            spawn.GetComponent<Rigidbody>().velocity += transform.forward * 50;
            Destroy(spawn, 2);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if(!GetComponent<TimeManager>().activate)
                GetComponent<TimeManager>().ActivateSlowMotion();
        }

	}
}
