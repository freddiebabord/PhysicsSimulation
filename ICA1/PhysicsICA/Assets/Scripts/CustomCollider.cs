using UnityEngine;
using System.Collections;

public class CustomCollider : MonoBehaviour
{

    [HideInInspector] public Vector3 previousPosition = Vector3.zero;
    public float frictionCoefficient = 0.1f;
    public float bouncieness = 0.35f;
    public Vector3 colliderSize;
    public Vector3 colliderOffset;
    public bool immovable = false;
    public ColliderType colliderType;

    public enum ColliderType
    {
        Box, Sphere,Cylinder
    }

    public virtual void FixedUpdate()
    {
        previousPosition = transform.position;
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
}