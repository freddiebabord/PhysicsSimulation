using UnityEngine;
using System.Collections;

public class TestPuck : MonoBehaviour {

    CustomRigidbody rb;
    public Vector3 testDirection = Vector3.forward;
    public float forceMagnitude = 5;

	// Use this for initialization
	void Start () {
        rb = GetComponent<CustomRigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(testDirection * forceMagnitude);
        }
	}
}
