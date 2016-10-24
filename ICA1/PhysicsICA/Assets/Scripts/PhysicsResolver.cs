using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsResolver : MonoBehaviour {

    public static PhysicsResolver inst;

    private List<CustomBoxCollider> colliders = new List<CustomBoxCollider>();
    
    void Awake()
    {
        inst = this;
    }

	void FixedUpdate()
    {
        for(int i = 0; i < colliders.Count; ++i)
        {
            for(int j = i+1; j < colliders.Count; ++j)
            {
                if (CheckCollision(colliders[i], colliders[j]))
                {
                    print(colliders[i].gameObject.name + " collided with " + colliders[j].gameObject.name);
                    

                    var speed1 = (colliders[i].transform.position - colliders[i].previousPosition) / Time.deltaTime;
                    Vector3 reflected = 2*(speed1.normalized * colliders[i].GetComponent<CustomRigidbody>().currentVelocity.magnitude);
                    //var mag = Vector3.Magnitude(speed1);
                    var force = reflected * colliders[i].bouncieness * colliders[j].bouncieness;
                    if (force.y < 0.05f && force.y > -0.05f)
                        force.y = 0;
                    if (colliders[i].GetComponent<CustomRigidbody>())
                    {
                        if (!colliders[i].GetComponent<CustomRigidbody>().grounded)
                        {
                            colliders[i].GetComponent<CustomRigidbody>().currentVelocity = -force;
                        }
                        else
                        {
                            Debug.Log(1);
                            colliders[i].GetComponent<CustomRigidbody>().frictionActingOnObject += -force * colliders[j].frictionCoefficient;
                        }
                    }
                    if (colliders[j].GetComponent<CustomRigidbody>())
                    {
                        if (!colliders[j].GetComponent<CustomRigidbody>().grounded)
                        {
                            colliders[j].GetComponent<CustomRigidbody>().currentVelocity = force;
                        }
                        else
                        {
                            Debug.Log(2);
                            colliders[j].GetComponent<CustomRigidbody>().frictionActingOnObject += -force * colliders[i].frictionCoefficient;
                        }
                    }
                }
            }
        }
    }

    bool CheckCollision(CustomBoxCollider one, CustomBoxCollider two) // AABB - AABB collision
    {
        bool xCheck = one.xMax > two.xMin && one.xMin < two.xMax;
        bool yCheck = one.yMin < two.yMax && one.yMax > two.yMin;
        bool zCheck = one.zMax > two.zMin && one.zMin < two.zMax;

        return xCheck && yCheck && zCheck;
    }

    public void RegisterCollider(CustomBoxCollider newCollider)
    {
        colliders.Add(newCollider);
    }

    public void DeregisterCollider(CustomBoxCollider requestedCollider)
    {
        colliders.Remove(requestedCollider);
        colliders.TrimExcess();
    }
}
