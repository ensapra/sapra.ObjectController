using UnityEngine;
using sapra.InputHandler;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Others")]
public class PVelocityCap : AbstractPassive
{
    private PGroundDetection _pGroundDetection;
    [Tooltip("Maximum allowed velocity on the controller")]
    public float maxVelocity = -50;
    private InputValues _input;
    private ActiveModule activeModule;

    protected override void AwakeRoutine()
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        activeModule = controller.RequestModule<ActiveModule>();
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(false);
        _input = GetComponent<InputContainer>(true).input;
    }
    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.LastOne)
        {
            float value = Vector3.Dot(rb.velocity, -motor.gravityDirection);
            if (value < maxVelocity)        
                rb.velocity = rb.velocity - Vector3.Project(rb.velocity, motor.gravityDirection) - maxVelocity*motor.gravityDirection;
            
            if(_pGroundDetection == null || _input == null)
                return;        
                
            if(_pGroundDetection != null &&_pGroundDetection.Walkable && activeModule.currentAction == null)
                if(!_pGroundDetection.detectionResult.rb)                
                    rb.velocity = Vector3.zero;       //Needs to be checked 
        }    
/*         if(currentPassivePriority == PassivePriority.FirstOfAll)
        {
            if(_pGroundDetection != null &&_pGroundDetection.Walkable)
                if(!_pGroundDetection.detectionResult.rb)                
                    rb.velocity = Vector3.zero;       
        }     */
    }

}