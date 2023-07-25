using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sapra.ObjectController;

[RoutineCategory("Others")]
public class PRealisticFall : AbstractPassive
{
    //PCustomGravity _pCustomGravity;
    [Range(0, 10)] public float airResistance = 5;
    [Range(0, 10)] public float fallSpeed = 8;
    public float TerminalSpeed = 50;
    PGroundDetection _pGroundDetection;
    PWaterDetection _pWaterDetection;
    CMotor motor;

    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.BeforeActive)
        {
            Vector3 GravityForce = Vector3.zero;
            Vector3 baseGravity = motor.gravityDirection*motor.gravityMultiplier;
            Vector3 hor = motor.velocity - Vector3.Project(motor.velocity,-motor.gravityDirection);
            bool applyGravity = true;

            if(_pWaterDetection != null && _pWaterDetection.Floating)
                return;

            if( _pGroundDetection.Walkable && _pGroundDetection.detectionResult.rb == null)
            {
                if((input._inputVector.magnitude < .1f && motor.velocity.magnitude < .1f))
                    applyGravity = false;
            }   
            else
            {
                GravityForce = (baseGravity*fallSpeed);
                GravityForce += (-hor*airResistance/10f);
            } 

            if(applyGravity)
                motor.EnableGravity(this);
            else
                motor.DisableGravity(this);

            float currentVelocity = Vector3.Dot(motor.velocity, motor.gravityDirection);
            
            if(currentVelocity > TerminalSpeed)             
                motor.SetVelocity(Vector3.ProjectOnPlane(motor.velocity, -motor.gravityDirection)+TerminalSpeed*motor.gravityDirection);
            else
                motor.AddVelocity(GravityForce*Time.deltaTime);
        }
    }
    protected override void Awake()
    {
        PassiveModule passiveModule = GetModule<PassiveModule>();
        _pGroundDetection = passiveModule.GetRoutine<PGroundDetection>(true);
        _pWaterDetection = passiveModule.GetRoutine<PWaterDetection>(false);
        motor = GetComponent<CMotor>(true);
    }
}
