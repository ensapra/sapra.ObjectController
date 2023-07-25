using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Simple")]
public class AMove : ActiveRoutine
{
    [Header("Movement Settings")]
    public bool AutoRun;
    public float TimeToRun = 2f;
    
    public override int priorityID => 0;
    private StatsContainer _statContainer;
    private PGroundDetection _pGroundDetection;
    private PDirectionManager _pDirectionManager;
    private CMotor motor;
    private bool overriden;
    [Range(0.01f, 1)] public float minRotationSpeed = 0.1f;
    [Range(0.01f, 1)] public float maxRotationSpeed = 0.25f;
    private float MovementTime;

    protected override void Awake()
    {
        _statContainer = GetComponent<StatsContainer>(true);
        PassiveModule passiveModule = GetModule<PassiveModule>();
        _pGroundDetection = passiveModule.GetRoutine<PGroundDetection>(true);
        _pDirectionManager = passiveModule.GetRoutine<PDirectionManager>(true);
        motor = GetComponent<CMotor>(true);
    }
    public override void OnStartActive(InputValues input)
    {
        MovementTime = 0;
    }
    public void setVelocity(float targetValue)
    {
        _statContainer.SetDynamicSpeed(targetValue);
        overriden = true;
    }
    public override void UpdateActive(InputValues _input)
    {
        if(!overriden)
        {
            if(_input._wantRun || (MovementTime > TimeToRun && AutoRun))
            {
                setVelocity(_statContainer.MaximumSpeed); 
                MovementTime = TimeToRun;
            }
            else  if(_input._wantWalk)
                setVelocity(_statContainer.MinimumSpeed);
            else
                setVelocity(_statContainer.MiddleSpeed);
        }
        overriden = false;
        float clampedMagnitude = Mathf.Clamp(_input._inputVector.magnitude,0,1); 
        Vector3 movementDirection = _pDirectionManager.GetMoveDirection(_input);
        Vector3 speed = Vector3.ClampMagnitude(movementDirection * clampedMagnitude* _statContainer.DynamicMovingSpeed, _statContainer.DynamicMovingSpeed);
        var speedFactor = Mathf.InverseLerp(_statContainer.MiddleSpeed, _statContainer.MaximumSpeed, speed.magnitude);
        speedFactor = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, 1-speedFactor);
        _pDirectionManager.RotateBody(speedFactor);  
        motor.SetVelocity(speed);    
        MovementTime += Time.deltaTime;
        if(clampedMagnitude < .5f)
            MovementTime = 0;
    }
    public override bool WantActive(InputValues input)
    {
        if(input._inputVector != Vector2.zero && _pGroundDetection.Walkable)
            return true;
        else
            return false;
    }
    
}

