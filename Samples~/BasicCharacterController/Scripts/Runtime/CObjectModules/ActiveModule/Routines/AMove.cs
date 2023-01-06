using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Simple")]
public class AMove : AbstractActive
{
    public override int priorityID => 0;
    private StatsContainer _statContainer;
    private PGroundDetection _pGroundDetection;
    private PDirectionManager _pDirectionManager;
    private bool overriden;
    [Range(0.01f, 1)] public float minRotationSpeed = 0.1f;
    [Range(0.01f, 1)] public float maxRotationSpeed = 0.25f;

    protected override void AwakeRoutine(AbstractCObject controller)
    {
        _statContainer = controller.RequestComponent<StatsContainer>(true);
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
        _pDirectionManager = passiveModule.RequestRoutine<PDirectionManager>(true);
        passiveModule.RequestRoutine<PVelocityCap>(true);        
    }
    public void setVelocity(float targetValue)
    {
        _statContainer.SetDynamicSpeed(targetValue);
        overriden = true;
    }
    public override void DoActive(InputValues _input)
    {
        if(!overriden)
        {
            if(_input._wantRun)
                setVelocity(_statContainer.MaximumSpeed); 
            else  if(_input._wantWalk)
                setVelocity(_statContainer.MinimumSpeed);
            else
                setVelocity(_statContainer.MiddleSpeed);
        }
        overriden = false;
        float clampedMagnitude = Mathf.Clamp(_input._inputVector.magnitude,0,1); 
        Vector3 movementDirection = _pDirectionManager.GetMoveDirection(_input);
        rb.velocity = Vector3.ClampMagnitude(movementDirection * clampedMagnitude* _statContainer.DynamicMovingSpeed, _statContainer.DynamicMovingSpeed);
        var speedFactor = Mathf.InverseLerp(_statContainer.MiddleSpeed, _statContainer.MaximumSpeed, rb.velocity.magnitude);
        speedFactor = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, 1-speedFactor);
        _pDirectionManager.RotateBody(speedFactor);      
    }
    public override bool WantActive(InputValues input)
    {
        if(input._inputVector != Vector2.zero && _pGroundDetection.Walkable)
            return true;
        else
            return false;
    }
}

