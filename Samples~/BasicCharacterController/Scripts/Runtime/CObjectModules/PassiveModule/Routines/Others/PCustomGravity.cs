using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Others")]
public class PCustomGravity : AbstractPassive
{
    [NoEdit] [SerializeField] private Vector3 direction;
    public float gravityMultiplierBase;
    private bool useGravity;
    [NoEdit] [SerializeField] private bool GravityApplied;
    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.LastOne)
        {
            if(rb.useGravity)
                rb.useGravity = false;
            controller.gravityDirection = direction.normalized;
            controller.gravityMultiplier = gravityMultiplierBase;
            if(useGravity)
                rb.velocity += direction.normalized*gravityMultiplierBase;
            GravityApplied = useGravity;
            useGravity = true;
        }
    }

    public void DisableGravity()
    {
        this.useGravity = false;
    }

    protected override void AwakeRoutine(AbstractCObject controller)
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        direction = controller.gravityDirection;
        gravityMultiplierBase = controller.gravityMultiplier;
    }
}