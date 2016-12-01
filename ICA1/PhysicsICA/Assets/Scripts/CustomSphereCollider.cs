public class CustomSphereCollider : CustomCollider
{
    public float radius = 1;

    void OnEnable()
    {
        PhysicsResolver.inst.RegisterCollider(this, immovable);
    }

    void OnDisable()
    {
        PhysicsResolver.inst.DeregisterCollider(this, immovable);
    }
}