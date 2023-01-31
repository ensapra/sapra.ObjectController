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
    protected override void AwakeRoutine()
    {
        surfaceDetection = controller.RequestModule<HelperModule>().RequestRoutine<HSurfaceDetection>(true);
        _statContainer = GetComponent<StatsContainer>(true);
    }

    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {        
        if(currentPassivePriority == PassivePriority.FirstOfAll)
        {
            if(rb)
                position += rb.velocity*Time.deltaTime; 
            if(debug)
                Debug.DrawRay(position, -motor.gravityDirection, Color.red);
            detectionResult = surfaceDetection.DetectSolid(position, -motor.gravityDirection, topWallLayer, _statContainer.CharacterHeight*1.2f, false, false);
        }
    }

}