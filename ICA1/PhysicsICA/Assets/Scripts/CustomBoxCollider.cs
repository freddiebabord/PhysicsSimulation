using UnityEngine;

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