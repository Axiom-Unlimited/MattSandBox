using UnityEngine;

public class CodeTutorial : MonoBehaviour
{
    //What is a Function and How can I use them?
    //Examples
    public float speed;

    void Update()
    {
        Locomotion();
    }

    void Locomotion()
    {
        var x = Input.GetAxis("Horizontal") * speed;
        var z = Input.GetAxis("Vertical") * speed;
        transform.Translate(x, 0, z);
    }



}
