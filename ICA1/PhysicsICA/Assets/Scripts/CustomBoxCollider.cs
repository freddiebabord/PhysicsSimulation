using UnityEngine;

public class CustomBoxCollider : CustomCollider
{
    public Vector3 colliderSize;
    public Vector3 colliderOffset;

    public float xMin { get { return transform.position.x - (colliderSize.x * transform.lossyScale.x) - colliderOffset.x; } }
    public float xMax { get { return transform.position.x + (colliderSize.x * transform.lossyScale.x) + colliderOffset.x; } }
    public float yMin { get { return transform.position.y - (colliderSize.y * transform.lossyScale.y) - colliderOffset.y; } }
    public float yMax { get { return transform.position.y + (colliderSize.y * transform.lossyScale.y) + colliderOffset.y; } }
    public float zMin { get { return transform.position.z - (colliderSize.z * transform.lossyScale.z) - colliderOffset.z; } }
    public float zMax { get { return transform.position.z + (colliderSize.z * transform.lossyScale.z) + colliderOffset.z; } }


    void OnEnable()
    {
        PhysicsResolver.inst.RegisterCollider(this);
    }

    void OnDisable()
    {
        PhysicsResolver.inst.DeregisterCollider(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + colliderOffset, new Vector3(colliderSize.x * transform.lossyScale.x, colliderSize.y * transform.lossyScale.y, colliderSize.z * transform.lossyScale.z));
    }

    public Vector3 U()
    {
        var mat = transform.localToWorldMatrix;
        return mat.GetRow(0);
    }
    public Vector3 V()
    {
        var mat = transform.localToWorldMatrix;
        return mat.GetRow(1);
    }
    public Vector3 W()
    {
        var mat = transform.localToWorldMatrix;
        return mat.GetRow(2);
    }
}