using UnityEngine;
using System.Collections;

public class CustomCollider : MonoBehaviour
{

    [HideInInspector] public Vector3 previousPosition = Vector3.zero;
    public float frictionCoefficient = 0.1f;
    public float bouncieness = 0.35f;

    public virtual void FixedUpdate()
    {
        previousPosition = transform.position;
    }

}