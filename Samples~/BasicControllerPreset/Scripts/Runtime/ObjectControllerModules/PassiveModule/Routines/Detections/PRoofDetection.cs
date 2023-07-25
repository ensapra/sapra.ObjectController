using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Detections")]
public class PRoofDetection : AbstractPassive
{
    HSurfaceDetection surfaceDetection;
    private StatsContainer _statContainer;
    public LayerMask topWallLayer = 1 << 0;
    public bool debug;
    [Header("Result")]
    public DetectionResult detectionResult;
    private CMotor motor;
    protected override void Awake()
    {
        surfaceDetection = GetModule<HelperModule>().GetRoutine<HSurfaceDetection>(true);
        _statContainer = GetComponent<StatsContainer>(true);
        motor = GetComponent<CMotor>(true);
    }

    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {        
        if(currentPassivePriority == PassivePriority.FirstOfAll)
        {
            position += motor.velocity*Time.deltaTime; 
            
            if(debug)
                Debug.DrawRay(position, -motor.gravityDirection, Color.red);
            detectionResult = surfaceDetection.DetectSolid(position, -motor.gravityDirection, topWallLayer, _statContainer.CharacterHeight*1.2f, false, false);
        }
    }

}