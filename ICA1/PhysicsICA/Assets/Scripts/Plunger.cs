using UnityEngine;
using System.Collections;

public class Plunger : MonoBehaviour {

    public float maxSpeed;
    private float currentSpeed = 0;
    public float chargeSpeed = 5.0f;
    private float startzPosition = 0.0f;
    private bool fire = false;
    public float Speed { get { return currentSpeed; } }

    public Transform puckXForm;
    public float rotationSpeed = 5.0f;

	// Use this for initialization
	void Start () {
        startzPosition = transform.position.z;

    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKey(KeyCode.Space))
        {
            if (currentSpeed < maxSpeed)
            {
                currentSpeed += chargeSpeed * Time.deltaTime;
                transform.Translate(Vector3.back * 0.5f * Time.deltaTime);
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            fire = true;
        }
        if(fire)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            if (transform.position.z >= startzPosition + 0.5f)
            {
                fire = false;
                currentSpeed = 0.0f;
            }
        }

        if (Input.GetKey(KeyCode.A))
            transform.RotateAround(puckXForm.position, new Vector3(0, 1, 0), rotationSpeed);
        if (Input.GetKey(KeyCode.D))
            transform.RotateAround(puckXForm.position, new Vector3(0, 1, 0), -rotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("HELLO " + other.gameObject.name + " " + currentSpeed);
        if(other.gameObject.name == "Puck")
        {
            other.GetComponent<Puck>().direction = other.transform.position - transform.position;
            other.GetComponent<Puck>().direction.y = 0;
            other.GetComponent<Puck>().direction.Normalize();
            other.GetComponent<Puck>().intitialVelocity = currentSpeed;
        }
    }
}
