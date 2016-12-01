using UnityEngine;
using System.Collections.Generic;
using System.Net;
using UnityEditorInternal;
using UnityEngine.UI;

public enum CollisionMode
{
    SAT,
    AABB
}

public class PhysicsResolver : MonoBehaviour {

    public static PhysicsResolver inst;

    private List<CustomCollider> colliders = new List<CustomCollider>();
    private List<CustomCollider> immovablecolliders = new List<CustomCollider>();
    private List<CustomCollider> allColliders = new List<CustomCollider>();
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
            CustomCollider colliderA = colliders[i];
            

            for (int j = 0; j < immovablecolliders.Count; ++j)
            {
                CustomCollider colliderB = immovablecolliders[j];

                CustomCollisionInfo collisionInfo;
                if (CheckCollision(colliderA, colliderB, out collisionInfo))
                {
                    Debug.Log(colliderA.gameObject + " collision with " + colliderB.gameObject);
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
                CustomCollider colliderB = colliders[j];

                CustomCollisionInfo collisionInfo;
                if (CheckCollision(colliderA, colliderB, out collisionInfo))
                {
                     Debug.Log(colliderA.gameObject + " collision with " + colliderB.gameObject);
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

    void DrawBoundingVolume(CustomCollider one, Color color, bool forceDraw = false, CustomCollider.ColliderType cType = CustomCollider.ColliderType.Box, CollisionMode mode = CollisionMode.SAT)
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
                if (one.colliderType == CustomCollider.ColliderType.Box)
                {
                    var box = ((CustomBoxCollider)one).GetOrientedBoxBounds();
                    GLDebug.DrawLine(box.dot1, box.dot2, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot2, box.dot6, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot5, box.dot6, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot5, box.dot1, color, Time.deltaTime, true);

                    GLDebug.DrawLine(box.dot4, box.dot8, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot3, box.dot7, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot4, box.dot3, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot8, box.dot7, color, Time.deltaTime, true);

                    GLDebug.DrawLine(box.dot1, box.dot4, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot2, box.dot3, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot6, box.dot7, color, Time.deltaTime, true);
                    GLDebug.DrawLine(box.dot5, box.dot8, color, Time.deltaTime, true);
                }
                else if (one.colliderType == CustomCollider.ColliderType.Sphere)
                {
                    var sphere = ((CustomSphereCollider) one);

                    Vector3 previousDirection = sphere.U().normalized;
                    for (int x = 0; x <= 20; x++)
                    {
                        Vector3 start = sphere.transform.position + previousDirection*sphere.radius;
                        Vector3 newDirection = Quaternion.Euler(0, x * (360/20), 0)*sphere.U();
                        Vector3 end = sphere.transform.position + newDirection.normalized * sphere.radius;
                        previousDirection = newDirection;
                        GLDebug.DrawLine(start, end, color, Time.deltaTime, true);
                    }
                    previousDirection = sphere.V().normalized;
                    for (int x = 0; x <= 20; x++)
                    {
                        Vector3 start = sphere.transform.position + previousDirection * sphere.radius;
                        Vector3 newDirection = Quaternion.Euler(0, 0, x * (360 / 20)) * sphere.V();
                        Vector3 end = sphere.transform.position + newDirection.normalized * sphere.radius;
                        previousDirection = newDirection;
                        GLDebug.DrawLine(start, end, color, Time.deltaTime, true);
                    }
                    previousDirection = sphere.W().normalized;
                    for (int x = 0; x <= 20; x++)
                    {
                        Vector3 start = sphere.transform.position + previousDirection * sphere.radius;
                        Vector3 newDirection = Quaternion.Euler(x * (360 / 20), 0, 0) * sphere.W();
                        Vector3 end = sphere.transform.position + newDirection.normalized * sphere.radius;
                        previousDirection = newDirection;
                        GLDebug.DrawLine(start, end, color, Time.deltaTime, true);
                    }
                }
            }
        }
    }

    bool CheckCollision(CustomCollider one_, CustomCollider two_, out CustomCollisionInfo hitInfo, CollisionMode mode = CollisionMode.SAT)//  AABB - AABB collision
    {
        hitInfo = new CustomCollisionInfo();

        if (one_.colliderType == CustomCollider.ColliderType.Box && two_.colliderType == CustomCollider.ColliderType.Box)
        {
            CustomBoxCollider one = one_ as CustomBoxCollider;
            CustomBoxCollider two = two_ as CustomBoxCollider;

            if (mode == CollisionMode.SAT)
            {
                List<Vector3> box1Points = one.GetOrientedBoxBounds().GetPoints();
                List<Vector3> box2Points = two.GetOrientedBoxBounds().GetPoints();


                var P1 = GetMinMaxObj(box1Points, one.V());
                var P2 = GetMinMaxObj(box2Points, one.V());
                var Q1 = GetMinMaxObj(box1Points, one.U());
                var Q2 = GetMinMaxObj(box2Points, one.U());
                var R1 = GetMinMaxObj(box1Points, one.W());
                var R2 = GetMinMaxObj(box2Points, one.W());

                var S1 = GetMinMaxObj(box1Points, two.V());
                var S2 = GetMinMaxObj(box2Points, two.V());
                var T1 = GetMinMaxObj(box1Points, two.U());
                var T2 = GetMinMaxObj(box2Points, two.U());
                var U1 = GetMinMaxObj(box1Points, two.W());
                var U2 = GetMinMaxObj(box2Points, two.W());

                bool P = P1.max < P2.min || P2.max < P1.min;
                bool Q = Q1.max < Q2.min || Q2.max < Q1.min;
                bool R = R1.max < R2.min || R2.max < R1.min;
                bool S = S1.max < S2.min || S2.max < S1.min;
                bool T = T1.max < T2.min || T2.max < T1.min;
                bool U = U1.max < U2.min || U2.max < U1.min;

                //Check if seperated
                bool res = P || Q || R || S || T || U;

                if (!res)
                {
                    hitInfo.intersectionSize.x =
                        Mathf.Max(Mathf.Min(two.GetOrientedBoxBounds().max.x, one.GetOrientedBoxBounds().max.x), 0);
                    hitInfo.intersectionSize.y =
                        Mathf.Max(Mathf.Min(two.GetOrientedBoxBounds().max.y, one.GetOrientedBoxBounds().max.x), 0);
                    hitInfo.intersectionSize.z =
                        Mathf.Max(Mathf.Min(two.GetOrientedBoxBounds().max.z, one.GetOrientedBoxBounds().max.x), 0);
                    DrawBoundingVolume(one, Color.red, true);
                    DrawBoundingVolume(two, Color.red, true);
                }

                return !res;
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
        
        if (one_.colliderType == CustomCollider.ColliderType.Sphere &&
            two_.colliderType == CustomCollider.ColliderType.Box)
        {
            return BoxVsSphereCollision((CustomBoxCollider)two_, (CustomSphereCollider)one_, out hitInfo);
        }
        else if (one_.colliderType == CustomCollider.ColliderType.Box &&
                    two_.colliderType == CustomCollider.ColliderType.Sphere)
        {
            return BoxVsSphereCollision((CustomBoxCollider) one_, (CustomSphereCollider) two_, out hitInfo);
        }
        
        return false;
    }

    public bool BoxVsSphereCollision(CustomBoxCollider box, CustomSphereCollider sphere, out CustomCollisionInfo info)
    {
        info = new CustomCollisionInfo();
        var spherePosition = sphere.transform.position + sphere.colliderOffset;
        var boxPosition = box.transform.position + box.colliderOffset;


        //List<Vector3> box1Points = box.GetOrientedBoxBounds().GetPoints();
        //List<Vector3> box2Points = new List<Vector3>();
        //box2Points.Add(spherePosition + sphere.U() * sphere.radius);
        //box2Points.Add(spherePosition - sphere.U() * sphere.radius);
        //box2Points.Add(spherePosition + sphere.V() * sphere.radius);
        //box2Points.Add(spherePosition - sphere.V() * sphere.radius);
        //box2Points.Add(spherePosition + sphere.W() * sphere.radius);
        //box2Points.Add(spherePosition - sphere.W() * sphere.radius);


        //var P1 = GetMinMaxObj(box1Points, box.V());
        //var P2 = GetMinMaxObj(box2Points, box.V());
        //var Q1 = GetMinMaxObj(box1Points, box.U());
        //var Q2 = GetMinMaxObj(box2Points, box.U());
        //var R1 = GetMinMaxObj(box1Points, box.W());
        //var R2 = GetMinMaxObj(box2Points, box.W());

        //var S1 = GetMinMaxObj(box1Points, sphere.V());
        //var S2 = GetMinMaxObj(box2Points, sphere.V());
        //var T1 = GetMinMaxObj(box1Points, sphere.U());
        //var T2 = GetMinMaxObj(box2Points, sphere.U());
        //var U1 = GetMinMaxObj(box1Points, sphere.W());
        //var U2 = GetMinMaxObj(box2Points, sphere.W());

        //bool P = P1.max < P2.min || P2.max < P1.min;
        //bool Q = Q1.max < Q2.min || Q2.max < Q1.min;
        //bool R = R1.max < R2.min || R2.max < R1.min;
        //bool S = S1.max < S2.min || S2.max < S1.min;
        //bool T = T1.max < T2.min || T2.max < T1.min;
        //bool U = U1.max < U2.min || U2.max < U1.min;

        ////Check if seperated
        //bool res = P || Q || R || S || T || U;

        //if (!res)
        //{
        //    info.intersectionSize.x =
        //        Mathf.Max(Mathf.Min((spherePosition + (sphere.U() * sphere.radius + sphere.V() * sphere.radius + sphere.W() * sphere.radius)).x, box.GetOrientedBoxBounds().max.x), 0);
        //    info.intersectionSize.y =
        //        Mathf.Max(Mathf.Min((spherePosition + (sphere.U() * sphere.radius + sphere.V() * sphere.radius + sphere.W() * sphere.radius)).y, box.GetOrientedBoxBounds().max.y), 0);
        //    info.intersectionSize.z =
        //        Mathf.Max(Mathf.Min((spherePosition + (sphere.U() * sphere.radius + sphere.V() * sphere.radius + sphere.W() * sphere.radius)).z, box.GetOrientedBoxBounds().max.z), 0);
        //    DrawBoundingVolume(box, Color.red, true);
        //    DrawBoundingVolume(sphere, Color.red, true, CustomCollider.ColliderType.Sphere);
        //}


        //return res;

        var normal = (boxPosition - spherePosition).normalized;
        var rad = Mathf.Abs(Vector3.Dot(box.U(), normal) + Vector3.Dot(box.V(), normal) + Vector3.Dot(box.W(), normal));
        var dot = Vector3.Dot(boxPosition, normal) - Vector3.Dot(spherePosition, normal);
        if (dot < rad)
        {
            var xMax1 = box.GetOrientedBoxBounds().max.x;
            var yMax1 = box.GetOrientedBoxBounds().max.y;
            var zMax1 = box.GetOrientedBoxBounds().max.z;

            var xMax2 = spherePosition.x + sphere.radius;
            var yMax2 = spherePosition.y + sphere.radius;
            var zMax2 = spherePosition.z + sphere.radius;

            info.intersectionSize.x = Mathf.Max(Mathf.Min(xMax2, xMax1), 0);
            info.intersectionSize.y = Mathf.Max(Mathf.Min(yMax2, yMax1), 0);
            info.intersectionSize.z = Mathf.Max(Mathf.Min(zMax2, zMax1), 0);
            DrawBoundingVolume(box, Color.red, true);
            DrawBoundingVolume(sphere, Color.red, true, CustomCollider.ColliderType.Sphere);
            return true;
        }
        return false;


        //Vector3 v;
        //var centerBox = box.transform.position + box.colliderOffset;
        //var currentCorner = box.GetOrientedBoxBounds().GetPoints()[0];
        //var max = Vector3.Distance(centerBox, spherePosition);
        //var box2sphere = boxPosition - spherePosition;
        //var box2SphereNormalised = box2sphere.normalized;
        //var points = box.GetOrientedBoxBounds().GetPoints();
        //var closestPoint = centerBox;
        //for (int i = 1; i < points.Count; ++i)
        //{
        //    currentCorner = points[i];
        //    if (Vector3.Distance(currentCorner, spherePosition) < max)
        //        closestPoint = currentCorner;
        //}

        //if ((sphere.radius - (spherePosition - closestPoint).magnitude) <= 0)
        //    return false;
        //else
        //{
        //    var xMax1 = box.GetOrientedBoxBounds().max.x;
        //    var yMax1 = box.GetOrientedBoxBounds().max.y;
        //    var zMax1 = box.GetOrientedBoxBounds().max.z;

        //    var xMax2 = sphere.radius - (spherePosition.x - closestPoint.x);
        //    var yMax2 = sphere.radius - (spherePosition.y - closestPoint.y);
        //    var zMax2 = sphere.radius - (spherePosition.z - closestPoint.z);

        //   // info.intersectionSize.x = sphere.radius - (spherePosition.x - closestPoint.x);
        //   // info.intersectionSize.y = sphere.radius - (spherePosition.y - closestPoint.y);
        //   // info.intersectionSize.z = sphere.radius - (spherePosition.z - closestPoint.z);

        //    info.intersectionSize.x = Mathf.Max(Mathf.Min(xMax2, xMax1), 0) * 2;
        //    info.intersectionSize.y = Mathf.Max(Mathf.Min(yMax2, yMax1), 0) * 2;
        //    info.intersectionSize.z = Mathf.Max(Mathf.Min(zMax2, zMax1), 0) * 2;

        //    return true;
        //}




        //if (box2sphere.magnitude - max - sphere.radius > 0 && box2sphere.magnitude > 0)
        //    return false;

        //DrawBoundingVolume(box, Color.red, true);
        //DrawBoundingVolume(sphere, Color.red, true, CustomCollider.ColliderType.Sphere);

        //var xMin1 = box.GetOrientedBoxBounds().min.x;
        //var xMax1 = box.GetOrientedBoxBounds().max.x;
        //var yMin1 = box.GetOrientedBoxBounds().min.y;
        //var yMax1 = box.GetOrientedBoxBounds().max.y;
        //var zMin1 = box.GetOrientedBoxBounds().min.z;
        //var zMax1 = box.GetOrientedBoxBounds().max.z;

        //var xMin2 = spherePosition.x - sphere.radius;
        //var xMax2 = spherePosition.x + sphere.radius;
        //var yMin2 = spherePosition.y - sphere.radius;
        //var yMax2 = spherePosition.y + sphere.radius;
        //var zMin2 = spherePosition.z - sphere.radius;
        //var zMax2 = spherePosition.z + sphere.radius;

        //info.intersectionSize.x = Mathf.Max(Mathf.Min(xMax2, xMax1), 0);
        //info.intersectionSize.y = Mathf.Max(Mathf.Min(yMax2, yMax1), 0);
        //info.intersectionSize.z = Mathf.Max(Mathf.Min(zMax2, zMax1), 0);

        //return true;
    }

    public struct MinMaxObj
    {
        public float min;
        public float max;
    }

    MinMaxObj GetMinMaxObj(List<Vector3> points, Vector3 axis)
    {
        float min_proj_box1 = Vector3.Dot(points[0], axis);
        int min_dot_box1 = 0;
        float max_proj_box1 = Vector3.Dot(points[0], axis);
        int max_dot_box1 = 0;

        for (var i = 1; i < points.Count; i++)
        {
            float currentProjection = Vector3.Dot(points[i], axis);
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

        MinMaxObj obj = new MinMaxObj();
        obj.min = min_proj_box1;
        obj.max = max_proj_box1;
        return obj;
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

    public void RegisterCollider(CustomCollider newCollider, bool immovable)
    {
        if(immovable)
            immovablecolliders.Add(newCollider);
        else
            colliders.Add(newCollider);
        allColliders.Add(newCollider);
    }

    public void DeregisterCollider(CustomCollider requestedCollider, bool immovable)
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
