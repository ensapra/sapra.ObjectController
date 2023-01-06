using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sapra.ObjectController;

[RoutineCategory("Others")]
public class PExtraGravity : AbstractPassive
{
    PCustomGravity _pCustomGravity;
    [Range(0, 10)] public float airResistance = 5;
    [Range(0, 10)] public float fallSpeed = 8;
    PGroundDetection _pGroundDetection;
    PWaterDetection _pWaterDetection;
    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.BeforeActive)
        {
            Vector3 GravityForce = Vector3.zero;
            Vector3 gravity = controller.gravityDirection*controller.gravityMultiplier;
            Vector3 hor = rb.velocity - Vector3.Project(rb.velocity,-controller.gravityDirection);
            if(_pWaterDetection != null && _pWaterDetection.Floating)
                return;

            if( _pGroundDetection.Walkable && _pGroundDetection.detectionResult.rb == null)
            {
                if(!(input._inputVector == Vector2.zero && rb.velocity.magnitude < 0.1f))
                    _pCustomGravity.DisableGravity();
            }   
            else
            {
                GravityForce += (gravity*fallSpeed);
                GravityForce += (-hor*airResistance/10f);
            } 

            rb.velocity += GravityForce;//(GravityForce, position, ForceMode.Acceleration);
        }
    }
    protected override void AwakeRoutine(AbstractCObject controller)
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _pCustomGravity = passiveModule.RequestRoutine<PCustomGravity>(true);
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
        _pWaterDetection = passiveModule.RequestRoutine<PWaterDetection>(false);
    }
}
