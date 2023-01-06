using UnityEngine;
using sapra.ObjectController;

public enum SurfaceStates{inside, surface, none}
[System.Serializable][RoutineCategory("Detections")]
public class PWaterDetection : AbstractPassive
{
    private HSurfaceDetection _surfaceDetection;
    private StatsContainer _statContainer;
    private PGroundDetection _pGroundDetection;
    private PCustomGravity _pCustomGravity;
    public LayerMask waterMask = 1<<4;
    public bool CanFloat = true;
    [Range(1, 10)]public float waterResistance = 7f;
    [Range(0, 20)]public float buoyancy = 3f;
    [Tooltip("Normalized amount of body height to shoulder level")]
    [Range(0.03f,0.99f)] public float shoulderLevel = 0.75f;
    public Vector3[] offsetPositions = new Vector3[]{};
    [Header("Results")]
    public DetectionResult detectionResult;

    [NoEdit] public bool Floating;
    [NoEdit] public SurfaceStates surfaceState;
    [NoEdit] public float NormalizedDistance;
    public bool debug = false;
    protected override void AwakeRoutine(AbstractCObject controller)
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _statContainer = controller.RequestComponent<StatsContainer>(true);
        _surfaceDetection = controller.RequestModule<HelperModule>().RequestRoutine<HSurfaceDetection>(true);
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(false);
        _pCustomGravity = passiveModule.RequestRoutine<PCustomGravity>(true);
    }

    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.FirstOfAll)
        {
            NormalizedDistance = CalculteNormalDistance(position, out detectionResult);
            if(detectionResult.distance <= 0)
                surfaceState = SurfaceStates.inside;
            else
            {
                if(NormalizedDistance <= 0)
                    surfaceState = SurfaceStates.none;
                else if((surfaceState == SurfaceStates.inside && NormalizedDistance <= shoulderLevel*.8f) || (surfaceState != SurfaceStates.inside && NormalizedDistance <= shoulderLevel))
                    surfaceState = SurfaceStates.surface;
                else
                    surfaceState = SurfaceStates.inside;
            }

            Floating = surfaceState == SurfaceStates.inside || (_pGroundDetection?.Walkable != true && surfaceState == SurfaceStates.surface);

            if(debug)
            {
                switch(surfaceState)
                {
                    case SurfaceStates.surface:
                        Debug.DrawRay(position, Vector3.up, Color.yellow);
                        break;
                    case SurfaceStates.inside:
                        Debug.DrawRay(position, Vector3.up, Color.blue);
                        break;
                    case SurfaceStates.none:
                        Debug.DrawRay(position, Vector3.up, Color.red);
                        break;
                }
            }
            
            if(offsetPositions.Length > 0)
            {
                for(int i = 0; i < offsetPositions.Length; i++)
                {
                    Vector3 pos = transform.TransformPoint(offsetPositions[i]);
                    ApplyWaterGravity(pos, CalculteNormalDistance(pos), offsetPositions.Length);
                }
            }
            else
                ApplyWaterGravity(position, NormalizedDistance, 1f);
        }
/*         else
        if(currentPassivePriority == PassivePriority.BeforeActive)
        {
            if(offsetPositions.Length > 0)
            {
                for(int i = 0; i < offsetPositions.Length; i++)
                {
                    Vector3 pos = transform.TransformPoint(offsetPositions[i]);
                    ApplyWaterGravity(pos, offsetDistances[i], offsetPositions.Length);
                }
            }
            else
                ApplyWaterGravity(position, NormalizedDistance, 1f);
        } */
    }
    private float CalculteNormalDistance(Vector3 position)
    {
        DetectionResult temp = new DetectionResult();
        return CalculteNormalDistance(position, out temp);
    }
    private float CalculteNormalDistance(Vector3 position, out DetectionResult detectionResult)
    {
        float dotProduct = Vector3.Dot(transform.up, -controller.gravityDirection);
        Vector3 UpWard = dotProduct > 0 ? transform.up*dotProduct : Vector3.zero;
        Vector3 topPosition = position+_statContainer.FootOffset+(_statContainer.CharacterHeight*1.1f)*UpWard;
        detectionResult = _surfaceDetection.DetectSolid(topPosition, controller.gravityDirection, waterMask,(_statContainer.CharacterHeight)*1.2f, false, true, debug);
        float normalDistance = 0;

        if(detectionResult.distance <= 0)
            normalDistance = 1;
        else
            normalDistance = Mathf.Clamp(1-(detectionResult.distance-_statContainer.CharacterHeight*.1f)/(_statContainer.CharacterHeight),0,1f);

        return normalDistance;
    }
    private void ApplyWaterGravity(Vector3 position, float normalDistance, float factor)
    {
        Vector3 ForceVector = Vector3.zero;
        if(Floating)
        {
            ForceVector += waterGravity(normalDistance);
            float distance = Mathf.Clamp(normalDistance, 0, 1);
            ForceVector += distance*(-rb.velocity*waterResistance);
            rb.angularVelocity -= (rb.angularVelocity*distance*(waterResistance/20))/factor;
        }
        rb.AddForceAtPosition(ForceVector/factor, position, ForceMode.Acceleration);
    }
    public Vector3 waterGravity(float normalDistance)
    {
        _pCustomGravity.DisableGravity();    
        Vector3 temporalResult = Vector3.zero;
        var targetValue = (normalDistance-(shoulderLevel-0.5f));
        Vector3 gravity = controller.gravityDirection*controller.gravityMultiplier;
        if(CanFloat)
        {
            //El -1 em contraresta la gravetat
            //El 1-2*target value fa que sigui zero en 0.5
            temporalResult += gravity*((1-2*targetValue)*buoyancy);
        }
        else
            temporalResult += (controller.gravityDirection*buoyancy);
        
        return temporalResult;
    }
}