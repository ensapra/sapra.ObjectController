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
            motor.gravityDirection = direction.normalized;
            controller.gravityMultiplier = gravityMultiplierBase;
            if(useGravity)
                rb.AddForce(direction.normalized*gravityMultiplierBase, ForceMode.Acceleration);
            GravityApplied = useGravity;
            useGravity = true;
        }
    }

    public void DisableGravity()
    {
        this.useGravity = false;
    }

    protected override void AwakeRoutine()
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        direction = motor.gravityDirection;
        gravityMultiplierBase = controller.gravityMultiplier;
    }
}