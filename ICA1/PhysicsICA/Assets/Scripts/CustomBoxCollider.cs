﻿using System.Collections.Generic;
using UnityEngine;

public struct Box
{
    public Vector3 dot1, dot2, dot3, dot4, dot5, dot6, dot7, dot8;
    public Vector3 min, max;

    public List<Vector3> GetPoints()
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(dot1);
        points.Add(dot2);
        points.Add(dot3);
        points.Add(dot4);
        points.Add(dot5);
        points.Add(dot6);
        points.Add(dot7);
        points.Add(dot8);
        return points;
    }
}

public class CustomBoxCollider : CustomCollider
{
    public Vector3 colliderSize;
    public Vector3 colliderOffset;
    public bool immovable = false;
    
    void OnEnable()
    {
        PhysicsResolver.inst.RegisterCollider(this, immovable);
    }

    void OnDisable()
    {
        PhysicsResolver.inst.DeregisterCollider(this, immovable);
    }
    

    public Vector3 U()
    {
        //return transform.InverseTransformDirection(Vector3.right);
        return transform.localToWorldMatrix.MultiplyVector(Vector3.right).normalized;
    }

    public Vector3 V()
    {
        //return transform.InverseTransformDirection(Vector3.up);
        return transform.localToWorldMatrix.MultiplyVector(Vector3.up).normalized;
    }

    public Vector3 W()
    {
        //return transform.InverseTransformDirection(Vector3.forward);
        return transform.localToWorldMatrix.MultiplyVector(Vector3.forward).normalized;
    }

    public Box GetOrientedBoxBounds()
    {
        Box box = new Box();
        Vector3 offsetPosition = transform.position 
            + U() * colliderOffset.x
            + V() * colliderOffset.y
            + W() * colliderOffset.z;

        box.dot1 = offsetPosition
            - U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            + V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            + W() * ((colliderSize.z * transform.lossyScale.z) / 2);

        box.dot2 = offsetPosition
            + U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            + V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            + W() * ((colliderSize.z * transform.lossyScale.z) / 2);

        box.dot3 = offsetPosition
            + U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            - V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            + W() * ((colliderSize.z * transform.lossyScale.z) / 2);

        box.dot4 = offsetPosition
            - U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            - V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            + W() * ((colliderSize.z * transform.lossyScale.z) / 2);


        box.dot5 = offsetPosition
            - U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            + V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            - W() * ((colliderSize.z * transform.lossyScale.z) / 2);

        box.dot6 = offsetPosition
            + U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            + V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            - W() * ((colliderSize.z * transform.lossyScale.z) / 2);

        box.dot7 = offsetPosition
            + U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            - V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            - W() * ((colliderSize.z * transform.lossyScale.z) / 2);

        box.dot8 = offsetPosition
            - U() * ((colliderSize.x * transform.lossyScale.x) / 2)
            - V() * ((colliderSize.y * transform.lossyScale.y) / 2)
            - W() * ((colliderSize.z * transform.lossyScale.z) / 2);

        var xMin1 = transform.position.x + colliderOffset.x - (colliderSize.x * transform.lossyScale.x) / 2;
        var xMax1 = transform.position.x + colliderOffset.x + (colliderSize.x * transform.lossyScale.x) / 2;
        var yMin1 = transform.position.y + colliderOffset.y - (colliderSize.y * transform.lossyScale.y) / 2;
        var yMax1 = transform.position.y + colliderOffset.y + (colliderSize.y * transform.lossyScale.y) / 2;
        var zMin1 = transform.position.z + colliderOffset.z - (colliderSize.z * transform.lossyScale.z) / 2;
        var zMax1 = transform.position.z + colliderOffset.z + (colliderSize.z * transform.lossyScale.z) / 2;

        box.min = new Vector3(xMin1, yMin1, zMin1);
        box.max = new Vector3(xMax1, yMax1, zMax1);

        return box;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, U());
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawRay(transform.position, V());
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(transform.position, W());
    //}
}