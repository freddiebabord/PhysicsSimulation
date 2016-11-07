using UnityEngine;
using System.Collections;

public class TestPlunger : MonoBehaviour {

    [HideInInspector]public Vector3 startPosition;
    [HideInInspector]public Quaternion startRotation;

    public float maxForce = 20.0f;
    private float currentForce = 0.0f;
    public float forceToAddPerSeccond = 2.0f;
    public Transform rotationPivot;
    bool fire = false;
    private float force_;
    CustomRigidbody rb;
    public float forceAdjust = 50;
    private Vector3 localHeadPos;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponentInChildren<CustomRigidbody>();
        localHeadPos = rb.transform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space) && currentForce < maxForce)
        {
            currentForce += forceToAddPerSeccond * Time.deltaTime;
            transform.position = startPosition + transform.forward * -currentForce;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            fire = true;
            force_ = currentForce * forceAdjust;
        }
        transform.RotateAround(rotationPivot.position, Vector3.up, Input.GetAxis("Horizontal"));

        if (fire)
        {
            if (currentForce > 0.1f)
            {
                transform.Translate(transform.forward * currentForce);
                currentForce -= (force_ / forceAdjust);
                rb.currentVelocity = transform.forward * force_;
                rb.transform.localPosition = localHeadPos;
            }
            else
            {
                transform.position = startPosition;
                currentForce = 0.0f;
                rb.currentVelocity = Vector3.zero;
                rb.transform.localPosition = localHeadPos;
                fire = false;
            }
        }
    }
}
