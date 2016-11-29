using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CollisionMode
{
    SAT,
    AABB
}

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

            Debug.DrawRay(colliderA.transform.position, colliderA.U() * 10, Color.magenta);
            Debug.DrawRay(colliderA.transform.position, colliderA.V() * 10, Color.magenta);
            Debug.DrawRay(colliderA.transform.position, colliderA.W() * 10, Color.magenta);

            for (int j = 0; j < immovablecolliders.Count; ++j)
            {
                CustomBoxCollider colliderB = immovablecolliders[j];
                GLDebug.DrawRay(colliderB.transform.position, colliderB.U() * 10, Color.magenta);
                GLDebug.DrawRay(colliderB.transform.position, colliderB.V() * 10, Color.magenta);
                GLDebug.DrawRay(colliderB.transform.position, colliderB.W() * 10, Color.magenta);

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
                    if (!colliderA.GetComponent<CustomRigidbody>().disablePhysicsInteractions)
                    {
                        
                        var newVel =
                            Vector3.Reflect(colliderA.GetComponent<CustomRigidbody>().currentVelocity.normalized,
                                firstCollisionNorm) * colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude * colliderA.bouncieness * (1 - colliderB.frictionCoefficient);
                        newVel = newVel.magnitude < 0.25f ? Vector3.zero : newVel;
                       // colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.magnitude;
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

                    // Pulled from Year 2 sphere vs cube simulation assignment
                    float m1, m2, x1, x2;
                    Vector3 v1, v2, v1x, v2x, v1y, v2y, x = (colliderA.transform.position - colliderB.transform.position);
                    var rbA = colliderA.GetComponent<CustomRigidbody>();
                    var rbB = colliderB.GetComponent<CustomRigidbody>();

                    

                    x.Normalize();
                    v1 = rbA.currentVelocity;
                    x1 = Vector3.Dot(x, v1);
                    v1x = x * x1;
                    v1y = v1 - v1x;
                    m1 = rbA.mass;

                    x = x * -1;
                    v2 = rbB.currentVelocity;
                    x2 = Vector3.Dot(x, v2);
                    v2x = x * x2;
                    v2y = v2 - v2x;
                    m2 = rbB.mass;
                    if (!colliderA.GetComponent<CustomRigidbody>().disablePhysicsInteractions)
                        rbA.currentVelocity = (v1x * (m1 - m2) / (m1 + m2) + v2x * (2 * m2) / (m1 + m2) + v1y);
                    if (!colliderB.GetComponent<CustomRigidbody>().disablePhysicsInteractions)
                        rbB.currentVelocity = (v1x * (2 * m1) / (m1 + m2) + v2x * (m2 - m1) / (m1 + m2) + v2y);

                    /*continue;

                    if (colliderA.GetComponent<CustomRigidbody>())
                    {
                        var directionA = colliderA.GetComponent<CustomRigidbody>().currentVelocity.normalized;
                        var directionB = colliderB.GetComponent<CustomRigidbody>().currentVelocity.normalized;
                        var newDir = (directionA - directionB).normalized;
                        var speed = colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude +
                                    colliderB.GetComponent<CustomRigidbody>().currentVelocity.magnitude;

                        var newVel =
                            Vector3.Reflect(newDir, -firstCollisionNorm) * speed * colliderA.bouncieness * colliderB.bouncieness;
                        if (colliderB.GetComponent<CustomRigidbody>())
                            newVel *= colliderB.GetComponent<CustomRigidbody>().currentVelocity.magnitude > 1 ? colliderB.GetComponent<CustomRigidbody>().currentVelocity.magnitude : 1;
                        newVel = newVel.magnitude < 0.25f ? Vector3.zero : newVel;
                        //  colliderA.transform.position += firstCollisionNorm*collisionInfo.intersectionSize.magnitude;
                        if (collisionInfo.intersectionSize.magnitude >= 0.1f)
                        {
                            if (ax <= ay && ax <= az)
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.x;
                            else if (ay <= az)
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.y;
                            else
                                colliderA.transform.position += firstCollisionNorm * collisionInfo.intersectionSize.z;
                        }

                        colliderA.GetComponent<CustomRigidbody>().currentVelocity = newVel;
                    }
                    if (colliderB.GetComponent<CustomRigidbody>())
                    {
                        var directionA = colliderA.GetComponent<CustomRigidbody>().currentVelocity.normalized;
                        var directionB = colliderB.GetComponent<CustomRigidbody>().currentVelocity.normalized;
                        var newDir = (directionA).normalized;
                        var speed = colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude +
                                    colliderB.GetComponent<CustomRigidbody>().currentVelocity.magnitude;

                        var newVel =
                            Vector3.Reflect(newDir, firstCollisionNorm) * speed * colliderA.bouncieness * colliderB.bouncieness;
                        if (colliderA.GetComponent<CustomRigidbody>())
                            newVel *= colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude > 1 ? colliderA.GetComponent<CustomRigidbody>().currentVelocity.magnitude : 1;
                        newVel = newVel.magnitude < 0.25f ? Vector3.zero : newVel;
                        // colliderB.transform.position += -firstCollisionNorm * collisionInfo.intersectionSize.magnitude;
                        if (collisionInfo.intersectionSize.magnitude >= 0.1f)
                        {
                            if (ax <= ay && ax <= az)
                                colliderB.transform.position += -firstCollisionNorm * collisionInfo.intersectionSize.x;
                            else if (ay <= az)
                                colliderB.transform.position += -firstCollisionNorm * collisionInfo.intersectionSize.y;
                            else
                                colliderB.transform.position += -firstCollisionNorm * collisionInfo.intersectionSize.z;
                        }

                        colliderB.GetComponent<CustomRigidbody>().currentVelocity = newVel;
                    }*/
                }
            }
        }
    }

    void DrawBoundingVolume(CustomBoxCollider one, Color color, bool forceDraw = false, CollisionMode mode = CollisionMode.SAT)
    {
        if (showBoundingVolumes.isOn || forceDraw)
        {
            if (mode == CollisionMode.AABB)
            {

                var xMin1 = one.transform.position.x + one.colliderOffset.x -
                            (one.colliderSize.x*one.transform.lossyScale.x)/2;
                var xMax1 = one.transform.position.x + one.colliderOffset.x +
                            (one.colliderSize.x*one.transform.lossyScale.x)/2;
                var yMin1 = one.transform.position.y + one.colliderOffset.y -
                            (one.colliderSize.y*one.transform.lossyScale.y)/2;
                var yMax1 = one.transform.position.y + one.colliderOffset.y +
                            (one.colliderSize.y*one.transform.lossyScale.y)/2;
                var zMin1 = one.transform.position.z + one.colliderOffset.z -
                            (one.colliderSize.z*one.transform.lossyScale.z)/2;
                var zMax1 = one.transform.position.z + one.colliderOffset.z +
                            (one.colliderSize.z*one.transform.lossyScale.z)/2;


                GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMin1), new Vector3(xMax1, yMin1, zMin1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMax1), new Vector3(xMin1, yMin1, zMin1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMax1), new Vector3(xMax1, yMin1, zMax1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMax1), new Vector3(xMax1, yMin1, zMin1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMin1, yMin1, zMax1), new Vector3(xMin1, yMax1, zMax1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMin1), new Vector3(xMax1, yMax1, zMin1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMin1, yMax1, zMin1), new Vector3(xMax1, yMax1, zMin1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMin1, yMax1, zMax1), new Vector3(xMin1, yMax1, zMin1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMin1, yMax1, zMax1), new Vector3(xMax1, yMax1, zMax1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMax1, yMax1, zMax1), new Vector3(xMax1, yMax1, zMin1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMax1), new Vector3(xMax1, yMax1, zMax1), color,
                    Time.deltaTime, true);
                GLDebug.DrawLine(new Vector3(xMax1, yMin1, zMax1), new Vector3(xMax1, yMax1, zMax1), color,
                    Time.deltaTime, true);
            }
            else
            {
                var box = one.GetOrientedBoxBounds();
                GLDebug.DrawLine(box.dot1, box.dot2, color,Time.deltaTime, true);
                GLDebug.DrawLine(box.dot2, box.dot6, color,Time.deltaTime, true);
                GLDebug.DrawLine(box.dot5, box.dot6, color,Time.deltaTime, true);
                GLDebug.DrawLine(box.dot5, box.dot1, color,Time.deltaTime, true);

                GLDebug.DrawLine(box.dot4, box.dot8, color, Time.deltaTime, true);
                GLDebug.DrawLine(box.dot3, box.dot7, color, Time.deltaTime, true);
                GLDebug.DrawLine(box.dot4, box.dot3, color, Time.deltaTime, true);
                GLDebug.DrawLine(box.dot8, box.dot7, color, Time.deltaTime, true);

                GLDebug.DrawLine(box.dot1, box.dot4, color, Time.deltaTime, true);
                GLDebug.DrawLine(box.dot2, box.dot3, color, Time.deltaTime, true);
                GLDebug.DrawLine(box.dot6, box.dot7, color, Time.deltaTime, true);
                GLDebug.DrawLine(box.dot5, box.dot8, color, Time.deltaTime, true);
            }
        }
    }

    bool CheckCollision(CustomBoxCollider one, CustomBoxCollider two, out CustomCollisionInfo hitInfo, CollisionMode mode = CollisionMode.SAT)//  AABB - AABB collision
    {
        hitInfo = new CustomCollisionInfo();

        if (mode == CollisionMode.SAT)
        {
            List<Vector3> box1Points = one.GetOrientedBoxBounds().GetPoints();
            List<Vector3> box2Points = two.GetOrientedBoxBounds().GetPoints();


            bool right = CheckAxis(box1Points, box2Points, Vector3.right);
            bool up = CheckAxis(box1Points, box2Points, Vector3.up);
            bool fwd = CheckAxis(box1Points, box2Points, Vector3.forward);

            return right && up && fwd;
        }
        else
        {

            var xMin1 = one.GetOrientedBoxBounds().min.x;
            var xMax1 = one.GetOrientedBoxBounds().max.x;
            var yMin1 = one.GetOrientedBoxBounds().min.y;
            var yMax1 = one.GetOrientedBoxBounds().max.y;
            var zMin1 = one.GetOrientedBoxBounds().min.z;
            var zMax1 = one.GetOrientedBoxBounds().max.z;

            var xMin2 = two.GetOrientedBoxBounds().min.x;
            var xMax2 = two.GetOrientedBoxBounds().max.x;
            var yMin2 = two.GetOrientedBoxBounds().min.y;
            var yMax2 = two.GetOrientedBoxBounds().max.y;
            var zMin2 = two.GetOrientedBoxBounds().min.z;
            var zMax2 = two.GetOrientedBoxBounds().max.z;



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
    }

    bool CheckAxis(List<Vector3> boxPoints1, List<Vector3> boxPoints2, Vector3 axis)
    {
        float min_proj_box1 = Vector3.Dot(boxPoints1[0], axis);
        int min_dot_box1 = 0;
        float max_proj_box1 = Vector3.Dot(boxPoints1[0], axis);
        int max_dot_box1 = 0;

        for (var i = 1; i < boxPoints1.Count; i++)
        {
            float currentProjection = Vector3.Dot(boxPoints1[i], axis);
            if (min_proj_box1 > currentProjection)
            {
                min_proj_box1 = currentProjection;
                min_dot_box1 = i;
            }

            if (currentProjection > max_proj_box1)
            {
                max_proj_box1 = currentProjection;
                max_dot_box1 = i;
            }
        }

        float min_proj_box2 = Vector3.Dot(boxPoints1[0], axis);
        int min_dot_box2 = 0;
        float max_proj_box2 = Vector3.Dot(boxPoints1[0], axis);
        int max_dot_box2 = 0;
        for (var i = 1; i < boxPoints2.Count; i++)
        {
            float currentProjection = Vector3.Dot(boxPoints2[i], axis);
            if (min_proj_box2 > currentProjection)
            {
                min_proj_box2 = currentProjection;
                min_dot_box2 = i;
            }

            if (currentProjection > max_proj_box2)
            {
                max_proj_box2 = currentProjection;
                max_dot_box2 = i;
            }
        }

        return max_proj_box2 < min_proj_box1 || max_proj_box1 < min_proj_box2;
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
