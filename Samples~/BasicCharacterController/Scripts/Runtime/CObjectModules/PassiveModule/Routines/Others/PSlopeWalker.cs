using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Others")]
public class PSlopeWalker : AbstractPassive
{
    private PGroundDetection _pGroundDetection;
    private PWaterDetection _pWaterDetection;

    private PColliderSettings _pColliderSettings;
    private StatsContainer _statContainer;
    [Tooltip("Max Slope teleport height")]
    public float slopeHeight = .5f;
    [Tooltip("Slope smoothness")]
    public float smoothness = .25f;
    
    private Vector3 refPosition;
    protected override void AwakeRoutine()
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _statContainer = GetComponent<StatsContainer>(true);
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
        _pWaterDetection = passiveModule.RequestRoutine<PWaterDetection>(false);
        _pColliderSettings = passiveModule.RequestRoutine<PColliderSettings>(true);
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
            rb.MovePosition( Vector3.SmoothDamp(transform.position, finalPos, ref refPosition, smoothness));
            //transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref refPosition, smoothness);
            Vector3 deletedVelocity = Vector3.Project(-rb.velocity, _pGroundDetection.detectionResult.normal);
            rb.velocity += deletedVelocity;
            
            if(_pGroundDetection.detectionResult.rb)
            {
                _pGroundDetection.detectionResult.rb.AddForceAtPosition(-deletedVelocity*rb.mass/Time.deltaTime, position);
            }
            float tempSlope = Mathf.Clamp(slopeHeight, 0.1f, _statContainer.currentHeight-_statContainer.CharacterRadius);
            _pColliderSettings.SetFactor(2-(tempSlope/(_statContainer.currentHeight/2f)));
        }
    }
}
