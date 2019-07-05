using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    float speed = 300f;
    float rotationSpeed = 500f;
    Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent("Rigidbody") as Rigidbody;

    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        //transform.Translate(0, 0, translation);
        rigidBody.AddRelativeForce(new Vector3(0, 0, translation));
        // Rotate around our y-axis
        //transform.Rotate(0, rotation, 0);
        rigidBody.AddTorque(new Vector3(0, rotation, 0), ForceMode.Force);
        Camera.main.transform.position = transform.position + new Vector3(0, 20, 4);
    }
}
