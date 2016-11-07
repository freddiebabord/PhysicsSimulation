using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PhysicsResolver : MonoBehaviour {

    public static PhysicsResolver inst;

    private List<CustomBoxCollider> colliders = new List<CustomBoxCollider>();
    private List<CustomBoxCollider> immovablecolliders = new List<CustomBoxCollider>();
    private List<CustomBoxCollider> allColliders = new List<CustomBoxCollider>();
    public Text collisionInfoText;
    public Toggle showBoundingVolumes;
    void Awake()
    {
        inst = this;
    }

	void FixedUpdate()
	{
        for (var i = 0; i < allColliders.Count; i++)
        {
            DrawBoundingVolume(allColliders[i], Color.green);
        }
	    collisionInfoText.text = "";
        for (int i = 0; i < colliders.Count; ++i)
        {
            CustomBoxCollider colliderA = colliders[i];

            for (int j = 0; j < immovablecolliders.Count; ++j)
            {
                CustomBoxCollider colliderB = immovablecolliders[j];
                

                CustomCollisionInfo collisionInfo;
                if (CheckCollision(colliderA, colliderB, out collisionInfo))
                {
                    //  Debug.Log(colliderA.gameObject + " collision with " + colliderB.gameObject);
                    collisionInfoText.text += colliderA.gameObject.name + " collision with " + colliderB.gameObject.name + "\n";

                    float ax = Mathf.Abs(collisionInfo.intersectionSize.x);
                    float ay = Mathf.Abs(collisionInfo.intersectionSize.y);
                    float az = Mathf.Abs(collisionInfo.intersectionSize.z);

                    float sx = colliderA.transform.position.x < colliderB.transform.position.x ? -1.0f : 1.0f;
                    float sy = colliderA.transform.position.y < colliderB.transform.position.y ? -1.0f : 1.0f;
                    float sz = colliderA.transform.position.z < colliderB.transform.position.z ? -1.0f : 1.0f;
                    Vector3 firstCollisionNorm;
                    if (ax <= ay && ax <= az)
                    {
                      //  Debug.Log("X");
                        firstCollisionNorm = colliderB.U().normalized * sx;
                    }
                    else if (ay <= az)
                    {
                      //  Debug.Log("Y");
                        firstCollisionNorm = colliderB.V().normalized * sy;
                    }
                    else
                    {
                     //   Debug.Log("Z");
                        firstCollisionNorm = colliderB.W().normalized * sz;
                    }
                    if (colliderA.GetComponent<CustomRigidbody>())
                    {
                        
                        var newVel =
                            Vector3.Reflect(colliderA.GetComponent<CustomRigidbody>().currentVelocity.normalized,
                                firstCollisionNorm) * colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude * colliderA.bouncieness;
                        newVel = newVel.magnitude < 0.25f ? Vector3.zero : newVel;
                        if (collisionInfo.intersectionSize.magnitude >= 0.1f)
                        {
                            if (ax <= ay && ax <= az)
                            {
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.x;
                            }
                            else if (ay <= az)
                            {
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.y;
                            }
                            else
                            {
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.z;
                            }
                        }
                        
                        colliderA.GetComponent<CustomRigidbody>().currentVelocity = newVel;
                    }
                }
            }


            for (int j = i+1; j < colliders.Count; ++j)
            {
                CustomBoxCollider colliderB = colliders[j];

                CustomCollisionInfo collisionInfo;
                if (CheckCollision(colliderA, colliderB, out collisionInfo))
                {
                    // Debug.Log(colliderA.gameObject + " collision with " + colliderB.gameObject);
                    collisionInfoText.text += colliderA.gameObject.name + " collision with " + colliderB.gameObject.name + "\n";

                    float ax = Mathf.Abs(collisionInfo.intersectionSize.x);
                    float ay = Mathf.Abs(collisionInfo.intersectionSize.y);
                    float az = Mathf.Abs(collisionInfo.intersectionSize.z);

                    float sx = colliderA.transform.position.x < colliderB.transform.position.x ? -1.0f : 1.0f;
                    float sy = colliderA.transform.position.y < colliderB.transform.position.y ? -1.0f : 1.0f;
                    float sz = colliderA.transform.position.z < colliderB.transform.position.z ? -1.0f : 1.0f;
                    Vector3 firstCollisionNorm;
                    if (ax <= ay && ax <= az)
                    {
                      //  Debug.Log("X");
                        firstCollisionNorm = colliderB.W().normalized * sx;
                    }
                    else if (ay <= az)
                    {
                      //  Debug.Log("Y");
                        firstCollisionNorm = colliderB.V().normalized * sy;
                    }
                    else
                    {
                      //  Debug.Log("Z");
                        firstCollisionNorm = colliderB.U().normalized * sz;
                    }
                    if (colliderA.GetComponent<CustomRigidbody>())
                    {

                        var newVel =
                            Vector3.Reflect(colliderA.GetComponent<CustomRigidbody>().currentVelocity.normalized,
                                firstCollisionNorm) * colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude * colliderA.bouncieness;
                        if (colliderB.GetComponent<CustomRigidbody>())
                            newVel *= colliderB.GetComponent<CustomRigidbody>().currentVelocity.magnitude > 1 ? colliderB.GetComponent<CustomRigidbody>().currentVelocity.magnitude : 1;
                        newVel = newVel.magnitude < 0.25f ? Vector3.zero : newVel;
                        if (collisionInfo.intersectionSize.magnitude >= 0.1f)
                        {
                            if (ax <= ay && ax <= az)
                            {
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.x;
                            }
                            else if (ay <= az)
                            {
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.y;
                            }
                            else
                            {
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.z;
                            }
                        }

                        colliderA.GetComponent<CustomRigidbody>().currentVelocity = newVel;
                    }
                    if (colliderB.GetComponent<CustomRigidbody>())
                    {
                        var newVel =
                            Vector3.Reflect(colliderB.GetComponent<CustomRigidbody>().currentVelocity.normalized,
                               -firstCollisionNorm) * colliderB.GetComponent<CustomRigidbody>().currentVelocity.magnitude * colliderB.bouncieness;
                        if (colliderA.GetComponent<CustomRigidbody>())
                            newVel *= colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude > 1 ? colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude : 1;
                        newVel = newVel.magnitude < 0.25f ? Vector3.zero : newVel;
                        if (collisionInfo.intersectionSize.magnitude >= 0.1f)
                        {
                            if (ax <= ay && ax <= az)
                            {
                                colliderB.transform.position += -firstCollisionNorm * collisionInfo.intersectionSize.x;
                            }
                            else if (ay <= az)
                            {
                                colliderB.transform.position += -firstCollisionNorm * collisionInfo.intersectionSize.y;
                            }
                            else
                            {
                                colliderB.transform.position += -firstCollisionNorm * collisionInfo.intersectionSize.z;
                            }
                        }

                        colliderB.GetComponent<CustomRigidbody>().currentVelocity = newVel;
                    }
                }
            }
        }
    }

    void DrawBoundingVolume(CustomBoxCollider one, Color color, bool forceDraw = false)
    {
        var xMin1 = one.transform.position.x + one.colliderOffset.x - (one.colliderSize.x * one.transform.lossyScale.x) / 2;
        var xMax1 = one.transform.position.x + one.colliderOffset.x + (one.colliderSize.x * one.transform.lossyScale.x) / 2;
        var yMin1 = one.transform.position.y + one.colliderOffset.y - (one.colliderSize.y * one.transform.lossyScale.y) / 2;
        var yMax1 = one.transform.position.y + one.colliderOffset.y + (one.colliderSize.y * one.transform.lossyScale.y) / 2;
        var zMin1 = one.transform.position.z + one.colliderOffset.z - (one.colliderSize.z * one.transform.lossyScale.z) / 2;
        var zMax1 = one.transform.position.z + one.colliderOffset.z + (one.colliderSize.z * one.transform.lossyScale.z) / 2;

        if (showBoundingVolumes.isOn || forceDraw)
        {
            GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMin1), new Vector3(xMax1, yMin1, zMin1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMax1), new Vector3(xMin1, yMin1, zMin1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMax1), new Vector3(xMax1, yMin1, zMax1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMax1), new Vector3(xMax1, yMin1, zMin1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMax1), new Vector3(xMin1, yMax1, zMax1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMin1), new Vector3(xMax1, yMax1, zMin1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMin1, yMax1, zMin1), new Vector3(xMax1, yMax1, zMin1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMin1, yMax1, zMax1), new Vector3(xMin1, yMax1, zMin1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMin1, yMax1, zMax1), new Vector3(xMax1, yMax1, zMax1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMax1, yMax1, zMax1), new Vector3(xMax1, yMax1, zMin1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMax1), new Vector3(xMax1, yMax1, zMax1), color, Time.deltaTime, true);
            GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMax1), new Vector3(xMax1, yMax1, zMax1), color, Time.deltaTime, true);
        }
    }

    bool CheckCollision(CustomBoxCollider one, CustomBoxCollider two, out CustomCollisionInfo hitInfo)//  AABB - AABB collision
    {
        var xMin1 = one.transform.position.x + one.colliderOffset.x - (one.colliderSize.x * one.transform.lossyScale.x) / 2;
        var xMax1 = one.transform.position.x + one.colliderOffset.x + (one.colliderSize.x * one.transform.lossyScale.x) / 2;
        var yMin1 = one.transform.position.y + one.colliderOffset.y - (one.colliderSize.y * one.transform.lossyScale.y) / 2;
        var yMax1 = one.transform.position.y + one.colliderOffset.y + (one.colliderSize.y * one.transform.lossyScale.y) / 2;
        var zMin1 = one.transform.position.z + one.colliderOffset.z - (one.colliderSize.z * one.transform.lossyScale.z) / 2;
        var zMax1 = one.transform.position.z + one.colliderOffset.z + (one.colliderSize.z * one.transform.lossyScale.z) / 2;

        var xMin2 = two.transform.position.x + two.colliderOffset.x - (two.colliderSize.x * two.transform.lossyScale.x) / 2;
        var xMax2 = two.transform.position.x + two.colliderOffset.x + (two.colliderSize.x * two.transform.lossyScale.x) / 2;
        var yMin2 = two.transform.position.y + two.colliderOffset.y - (two.colliderSize.y * two.transform.lossyScale.y) / 2;
        var yMax2 = two.transform.position.y + two.colliderOffset.y + (two.colliderSize.y * two.transform.lossyScale.y) / 2;
        var zMin2 = two.transform.position.z + two.colliderOffset.z - (two.colliderSize.z * two.transform.lossyScale.z) / 2;
        var zMax2 = two.transform.position.z + two.colliderOffset.z + (two.colliderSize.z * two.transform.lossyScale.z) / 2;

        hitInfo = new CustomCollisionInfo();

        bool res = xMin1 <= xMax2 && xMax1 >= xMin2 &&
               yMin1 <= yMax2 && yMax1 >= yMin2 &&
               zMin1 <= zMax2 && zMax1 >= zMin2;

        if (res)
        {
            hitInfo.intersectionSize.x = Mathf.Max(Mathf.Min(xMax2, xMax1), 0);
            hitInfo.intersectionSize.y = Mathf.Max(Mathf.Min(yMax2, yMax1), 0);
            hitInfo.intersectionSize.z = Mathf.Max(Mathf.Min(zMax2, zMax1), 0);
            DrawBoundingVolume(one, Color.red, true);
            DrawBoundingVolume(two, Color.red, true);
        }

        return res;

    }

    public void RegisterCollider(CustomBoxCollider newCollider, bool immovable)
    {
        if(immovable)
            immovablecolliders.Add(newCollider);
        else
            colliders.Add(newCollider);
        allColliders.Add(newCollider);
    }

    public void DeregisterCollider(CustomBoxCollider requestedCollider, bool immovable)
    {
        if (immovable)
        {
            immovablecolliders.Remove(requestedCollider);
            immovablecolliders.TrimExcess();
        }
        else
        {
            colliders.Remove(requestedCollider);
            colliders.TrimExcess();
        }

        allColliders.Remove(requestedCollider);
        allColliders.TrimExcess();
    }

    public void Reset()
    {
        colliders.Clear();
        allColliders.Clear();
        allColliders.AddRange(immovablecolliders);
        //immovablecolliders.Clear();
    }

    public bool Raycast(CustomRay ray, out CustomHitInfo hitInfo, int maxDistance)
    {
        hitInfo = new CustomHitInfo();
       //  TODO: Write raycast function
        /*
        for (int i = 0; i < 3; ++i)
        {
            float invD = 1/ray.m_direction[i];
            float t0 = (i - ray.m_origin[i])*invD;
        }
        */
        return true;
    }
}

public class CustomRay
{
    public CustomRay(Vector3 origin, Vector3 direction)
    {
        m_direction = direction;
        m_origin = origin;
    }

    public Vector3 m_origin, m_direction;
}

public struct CustomHitInfo
{
    public CustomBoxCollider hitCollider;
    public Vector3 hitPoint;
}

public class CustomCollisionInfo
{
    public Vector3 intersectionSize;

}
