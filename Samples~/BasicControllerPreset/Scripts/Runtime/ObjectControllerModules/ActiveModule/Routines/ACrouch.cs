using UnityEngine;
using sapra.ObjectController;
using NaughtyAttributes;

[System.Serializable][RoutineCategory("Complex")]
public class ACrouch : ActiveRoutine
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
    [AllowNesting][ReadOnly] [SerializeField] private float refHeightSpeed;
    public override int priorityID => 10;
    protected override void Awake()
    {
        PassiveModule passiveModule = GetModule<PassiveModule>();
        ActiveModule activeModule = GetModule<ActiveModule>();
        _statContainer = GetComponent<StatsContainer>(true);
        _aMove = activeModule.GetRoutine<AMove>(true);
        _pGroundDetection = passiveModule.GetRoutine<PGroundDetection>(true);
        _pRoofDetection = passiveModule.GetRoutine<PRoofDetection>(true);
        _pColliderSettings = passiveModule.GetRoutine<PColliderSettings>(true);
        _pWaterDetection = passiveModule.GetRoutine<PWaterDetection>(false);
    }

    public override void UpdateActive(InputValues input)
    {
        _aMove.setVelocity(_statContainer.MinimumSpeed);
        _aMove.UpdateActive(input);
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
    
    public override void DoPassiveAfterAction()
    {
        float distanceRemaped = Mathf.InverseLerp(crouchHeight, _statContainer.CharacterHeight, _pRoofDetection.detectionResult.distance-crouchHeadDistanceOffset);
        if(isActive)
            heightLerped = Mathf.SmoothDamp(heightLerped, 0f, ref refHeightSpeed, .1f);
        else
            heightLerped = Mathf.SmoothDamp(heightLerped, distanceRemaped, ref refHeightSpeed, .1f);
    }
}