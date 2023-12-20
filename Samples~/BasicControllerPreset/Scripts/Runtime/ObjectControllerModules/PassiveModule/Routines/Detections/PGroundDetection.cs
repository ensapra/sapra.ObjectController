using System.Collections;
using System.Collections.Generic;
using sapra.ObjectController;
using UnityEngine;
using NaughtyAttributes;

[RoutineCategory("Detections")]
public class PGroundDetection : AbstractPassive
{
    HSurfaceDetection _surfaceDetection;
    StatsContainer _statContainer;
    public LayerMask groundMask = 1<<0;

    public bool debug;
    [Header("Results")]
    [SerializeField] public DetectionResult detectionResult;
    [Tooltip("Ground Mask")]
    [DisableIf("True")] public bool Walkable;
    [DisableIf("True")] public float AngleFront;
    [DisableIf("True")] public float NormalizedDistance;

    private float AngleFrontLerped;
    private float refAngleFront;
    private CMotor motor;

    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.FirstOfAll)
        {
            Vector3 topPosition = position+_statContainer.FootOffset+_statContainer.currentHeight*-motor.gravityDirection;
            detectionResult = _surfaceDetection.DetectSolid(topPosition, motor.gravityDirection, groundMask, _statContainer.currentHeight*1.5f, true, false, debug);
            if(debug)
                Debug.DrawRay(detectionResult.point, detectionResult.normal, Color.red);
            
            NormalizedDistance = (detectionResult.distance-_statContainer.currentHeight)/(_statContainer.currentHeight/2f);
            AngleFront = Vector3.Angle(detectionResult.normal, transform.forward) - 90;
            Walkable = NormalizedDistance <= 0.1f && detectionResult.angle < _statContainer.WalkableAngle;
            if(Walkable && debug)
                Debug.DrawRay(detectionResult.point, detectionResult.normal*2, Color.blue);


            AngleFrontLerped = Mathf.SmoothDamp(AngleFrontLerped, AngleFront, ref refAngleFront, .1f);
        }
    }

    protected override void Awake()
    {
        _surfaceDetection = GetModule<HelperModule>().GetRoutine<HSurfaceDetection>(true);
        _statContainer = GetComponent<StatsContainer>(true);
        motor = GetComponent<CMotor>(true);
    }
}
