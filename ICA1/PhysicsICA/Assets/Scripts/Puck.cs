using UnityEngine;
using System.Collections;

public class Puck : MonoBehaviour {

    public float intitialVelocity;
    public float mass = 5.0f;
    public float frictionAmount;
    public Vector3 direction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var currentVelocity = mass * (intitialVelocity - frictionAmount);
        transform.Translate(direction * currentVelocity * Time.deltaTime);

    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.collider);
    //    if (collision.collider.gameObject.name == "Plunger")
    //    {
    //        direction = transform.position - collision.contacts[0].point;
    //        direction.Normalize();
    //        direction.y = 0;
    //        intitialVelocity = 5;
    //    }
    //}

    //void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log(collision.collider);
    //    if (collision.collider.gameObject.name == "Plunger")
    //    {
    //        direction = transform.position - collision.contacts[0].point;
    //        direction.Normalize();
    //        direction.y = 0;
    //        intitialVelocity = collision.collider.gameObject.GetComponent<Plunger>().Speed * 10;
    //    }
    //}

}
