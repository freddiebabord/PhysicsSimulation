using UnityEngine;
using System.Collections;

public class CustomRigidbody : MonoBehaviour {

    public float mass;
    public bool useGravity = true;
    public Vector3 currentVelocity = Vector3.zero;
    public Vector3 currentDirection { get { return currentVelocity.normalized; } }
    private float previousY = 0;
    public bool grounded { get { return currentVelocity.y < 0.25f && currentVelocity.y > -0.25f; } }
    public Vector3 frictionActingOnObject;

    // Use this for initialization
    void Start () {
        previousY = transform.position.y;
        currentVelocity = new Vector3(0, -9.8f, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        var acceleration = Vector3.zero;
        var velocity = currentVelocity;
        Vector3 existingAccelleration = currentVelocity / mass;
        if (useGravity && !grounded)
            acceleration = new Vector3(0, -9.81f, 0) + existingAccelleration;
        else
        {
            velocity.y = 0;
            acceleration = existingAccelleration;
            acceleration.y = 0;
            currentVelocity.y = 0;
        }
        acceleration += ApplyDrag(acceleration, 5);
       // velocity = ApplyFriction(acceleration);

        velocity += acceleration * Time.deltaTime;
       
        transform.Translate(velocity * Time.deltaTime);
        currentVelocity = velocity;
        previousY = transform.position.y;
    }


    Vector3 ApplyDrag(Vector3 acceleration, float area)
    {
        //Vector3 vsquared = new Vector3(acceleration.x * acceleration.x, acceleration.y * acceleration.y, acceleration.z * acceleration.z);
        return (0.47f * area * 0.5f * 1.225f * acceleration) / mass;
    }

    public void AddForce(Vector3 force)
    {
        currentVelocity += force;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + currentVelocity);
    }

    Vector3 ApplyFriction(Vector3 currentVelocity_)
    {
        Vector3 tempFriction = frictionActingOnObject;
        frictionActingOnObject = Vector3.zero;
        return currentVelocity_ -= (tempFriction * Time.deltaTime);
    }
}
