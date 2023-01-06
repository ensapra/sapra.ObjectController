using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Complex")]
public class ACrouch : AbstractActive
{
    [Tooltip("Character crouch height")]
    public float crouchHeight = 1f;
    [Tooltip("Character crouch radius")]
    public float crouchRadius = 0.5f;
    public float crouchHeadDistanceOffset = 0.3f;

    private AMove _aMove;
    private PRoofDetection _pRoofDetection;
    private PGroundDetection _pGroundDetection;
    private PColliderSettings _pColliderSettings;
    private PWaterDetection _pWaterDetection;
    private StatsContainer _statContainer;
    private float heightLerped; 
    [NoEdit] [SerializeField] private float refHeightSpeed;
    public override int priorityID => 10;
    protected override void AwakeRoutine(AbstractCObject controller)
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        ActiveModule activeModule = controller.RequestModule<ActiveModule>();
        _statContainer = controller.RequestComponent<StatsContainer>(true);
        _aMove = activeModule.RequestRoutine<AMove>(true);
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
        _pRoofDetection = passiveModule.RequestRoutine<PRoofDetection>(true);
        _pColliderSettings = passiveModule.RequestRoutine<PColliderSettings>(true);
        _pWaterDetection = passiveModule.RequestRoutine<PWaterDetection>(false);
    }

    public override void DoActive(InputValues input)
    {
        _aMove.setVelocity(_statContainer.MinimumSpeed);
        _aMove.DoActive(input);
        if(_pColliderSettings != null)
            _pColliderSettings.ChangeSettings(crouchHeight/2f, crouchRadius);    
    }

    public override void DoPassiveBeforeAction()
    {
        float clampedDistance = Mathf.Clamp(_pRoofDetection.detectionResult.distance-crouchHeadDistanceOffset, crouchHeight, _statContainer.CharacterHeight);
        float normalRadious = Mathf.Lerp(crouchRadius, _statContainer.CharacterRadius,Mathf.InverseLerp(crouchHeight, _statContainer.CharacterHeight, clampedDistance));

        if(clampedDistance < _statContainer.CharacterHeight && clampedDistance > crouchHeight*1.1f)
        {
            _pColliderSettings.ChangeSettings(clampedDistance/2f, normalRadious); 
        }
    }
    public override bool WantActive(InputValues input)
    {
        if(!_pGroundDetection.Walkable)
            return false;
        if(_pWaterDetection != null && _pWaterDetection.NormalizedDistance > crouchHeight*0.8f/_statContainer.CharacterHeight)
            return false;
        if(input._wantCrouch && !input._wantRun)
            return true;

        if(_pRoofDetection.detectionResult.distance-crouchHeadDistanceOffset <= crouchHeight*1.1f)
            return true;          
        
        return false;
    }
    
}