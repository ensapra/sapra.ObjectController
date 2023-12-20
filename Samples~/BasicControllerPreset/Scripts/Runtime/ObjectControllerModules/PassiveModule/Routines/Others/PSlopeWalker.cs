using UnityEngine;
using sapra.ObjectController;
using NaughtyAttributes;

[System.Serializable][RoutineCategory("WORK IN PROGRESS")]
public class PSlopeWalker : AbstractPassive
{
    private PGroundDetection _pGroundDetection;
    private PWaterDetection _pWaterDetection;

    private PColliderSettings _pColliderSettings;
    private CMotor motor;
    private StatsContainer _statContainer;
    [Tooltip("Max Slope teleport height")]
    public float slopeHeight = .5f;
    [Tooltip("Slope smoothness")]
    public float smoothness = .25f;
    
    private Vector3 refPosition;
    protected override void Awake()
    {
        PassiveModule passiveModule = GetModule<PassiveModule>();
        _statContainer = GetComponent<StatsContainer>(true);
        _pGroundDetection = passiveModule.GetRoutine<PGroundDetection>(true);
        _pWaterDetection = passiveModule.GetRoutine<PWaterDetection>(false);
        _pColliderSettings = passiveModule.GetRoutine<PColliderSettings>(true);
        motor = GetComponent<CMotor>(true);
    }

    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.BeforeActive)
        {
            if(_pWaterDetection?.Floating == true)
                return;
            if(!_pGroundDetection.Walkable)
            {
                _pColliderSettings.SetFactor(2-(_statContainer.currentRadious/(_statContainer.currentHeight/2f)));
                return;
            }
            Vector3 finalPos = position - Vector3.Project(position, -motor.gravityDirection) + -motor.gravityDirection*(_pGroundDetection.detectionResult.point.y-_statContainer.FootOffset.y);
            ///rb.MovePosition(Vector3.SmoothDamp(transform.position, finalPos, ref refPosition, smoothness));
            
            Vector3 targetPos = Vector3.SmoothDamp(transform.position, finalPos, ref refPosition, smoothness);
            transform.position = targetPos;
            
            if(_pGroundDetection.detectionResult.rb)
            {
                Vector3 deletedVelocity = Vector3.Project(-motor.newVelocity, _pGroundDetection.detectionResult.normal);
                _pGroundDetection.detectionResult.rb.AddForceAtPosition(-deletedVelocity*motor.rb.mass/Time.deltaTime, position);
                motor.AddVelocity(deletedVelocity);
            }

            float tempSlope = Mathf.Clamp(slopeHeight, 0.1f, _statContainer.currentHeight-_statContainer.CharacterRadius);
            _pColliderSettings.SetFactor(2-(tempSlope/(_statContainer.currentHeight/2f)));
        }
    }
}
