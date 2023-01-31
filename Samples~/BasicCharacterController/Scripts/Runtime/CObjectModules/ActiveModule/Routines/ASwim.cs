using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Complex")]
public class ASwim : AbstractActive
{
    public override int priorityID => 2;

    private PWaterDetection _pWaterDetection;
    private PGroundDetection _pGroundDetection;
    private PDirectionManager _pDirectionManager;
    private StatsContainer _statContainer;
    private PColliderSettings _pColliderSettings;

    private Vector3 finalDirection;
    private Vector3 refFinalDirection;
    private Vector3 inputedDirection;
    [Range(0.01f, 1)] public float minRotationSpeed = 0.1f;
    [Range(0.01f, 1)] public float maxRotationSpeed = 0.25f;
    protected override void AwakeRoutine()
    {
        _statContainer = GetComponent<StatsContainer>(true);
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _pWaterDetection = passiveModule.RequestRoutine<PWaterDetection>(true);
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
        _pColliderSettings = passiveModule.RequestRoutine<PColliderSettings>(true);
        _pDirectionManager = passiveModule.RequestRoutine<PDirectionManager>(true);
        passiveModule.RequestRoutine<PExtraGravity>(true);

    }
    private void setVelocity(float targetValue)
    {
        _statContainer.SetDynamicSpeed(targetValue);
    }
    public override void DoActive(InputValues _input)
    {            
        if(_pColliderSettings != null)
            ChangeCollider();

        if(_input._wantRun)
            setVelocity(_statContainer.MaximumSwimSpeed);
        else
            setVelocity(_statContainer.MinimumSwimSpeed);
        inputedDirection = Vector3.ClampMagnitude(inputedDirection*_statContainer.DynamicMovingSpeed, _statContainer.DynamicMovingSpeed);
        finalDirection = Vector3.SmoothDamp(finalDirection, inputedDirection, ref refFinalDirection, .1f);
        rb.velocity = finalDirection;
        var speedFactor = Mathf.InverseLerp(_statContainer.MinimumSwimSpeed, _statContainer.MaximumSwimSpeed, rb.velocity.magnitude);
        speedFactor = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, 1-speedFactor);
        _pDirectionManager.RotateBody(speedFactor);
    }
    public override bool WantActive(InputValues input)
    {
        inputedDirection = GetWaterDirection(input);
        if(_pWaterDetection.surfaceState == SurfaceStates.inside && inputedDirection.sqrMagnitude > 0.1f)
            return true;                    
        else
        {
            finalDirection = rb.velocity;
            return false;
        }
    }
    public Vector3 GetWaterDirection(InputValues input)
    {
        float upDown = input._upDownRaw;
        Vector3 direction;
        direction = _pDirectionManager.GetReferencedInput();
        direction += (upDown)*-motor.gravityDirection;
        direction = GetFinalDirection(direction);
        return direction;
    }
    private Vector3 GetFinalDirection(Vector3 inputDirection)
    {
        if(_pGroundDetection.NormalizedDistance < 1f)
            inputDirection -= Vector3.Project(inputDirection, -_pGroundDetection.detectionResult.normal)*(1-_pGroundDetection.NormalizedDistance);
        if(_pWaterDetection.NormalizedDistance < 1 && Vector3.Dot(inputDirection, _pWaterDetection.detectionResult.normal) > 0)
            inputDirection -= Vector3.Project(inputDirection, _pWaterDetection.detectionResult.normal);
        return inputDirection;
    }
    public void ChangeCollider()
    {
        _pColliderSettings.ChangeSettings(_statContainer.CharacterHeight/2f, _statContainer.CharacterHeight/2f);
    }
}