using UnityEngine;
using System.Collections;

public class CustomRigidbody : MonoBehaviour {

    public float mass;
    public bool useGravity = true;
    public Vector3 currentVelocity = Vector3.zero;
    public Vector3 currentDirection { get { return currentVelocity.normalized; } }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        var acceleration = Vector3.zero;
        var velocity = currentVelocity;
        Vector3 existingAccelleration = currentVelocity / mass;
        if (useGravity)
            acceleration = new Vector3(0, -9.81f, 0) + existingAccelleration;

        acceleration += ApplyDrag(acceleration, 5);
        transform.Translate(velocity * Time.deltaTime);

        velocity += acceleration * Time.deltaTime;

        currentVelocity = velocity;
    }


    Vector3 ApplyDrag(Vector3 acceleration, float area)
    {
       // Vector3 vsquared = new Vector3(acceleration.x * acceleration.x, acceleration.y * acceleration.y, acceleration.z * acceleration.z);
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
}
