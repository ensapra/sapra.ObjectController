using UnityEngine;
using sapra.ObjectController;

[System.Serializable]
public class HSurfaceDetection : HelperRoutine
{
    private StatsContainer _statContainer;
    
    protected override void AwakeRoutine()
    {
        _statContainer = GetComponent<StatsContainer>(true);
    }
    public DetectionResult DetectSolid(Vector3 position, Vector3 direction, LayerMask surfaceMask,  float maxDistance, bool limitAngle, bool insideCheck = false, bool debug = false)
    {
        Vector3 normal = Vector3.zero;
        Vector3 point = Vector3.zero;
        float distance = maxDistance;
        Rigidbody rb = null;
        float angle = 0;
        
        float radius = (_statContainer.currentRadious-0.05f);
        RaycastHit hit;
        if(debug)
            Debug.DrawRay(position, direction.normalized*maxDistance, Color.black);
        Collider[] coll = Physics.OverlapSphere(position, radius/10, surfaceMask, QueryTriggerInteraction.Collide);
        if(coll.Length > 0 && insideCheck)
            distance = -1;
        else 
        {
            int detects = 1;
            if(Physics.Raycast(position, direction.normalized, out hit, maxDistance, surfaceMask))
            {
                normal = hit.normal;
                point = hit.point;
                distance = hit.distance;
                rb = hit.rigidbody;
                detects ++;

            }
            if(Physics.SphereCast(position, radius, direction.normalized, out hit, maxDistance-radius, surfaceMask))
            {                    
                if((Vector3.Angle(hit.normal, -direction) < _statContainer.WalkableAngle && limitAngle) || !limitAngle)
                { 
                    normal = (normal+hit.normal)/detects;
                    point = (point+(hit.point))/detects;
                    Vector3 hitDirection = position-hit.point;
                    float dot = Vector3.Dot(hitDirection, -direction);
                    if(detects > 1)
                        distance = (distance+dot)/detects; 
                    else
                        distance = dot;
                    rb = hit.rigidbody;
                    if(debug)
                        Debug.DrawRay(hit.point, hit.normal, Color.blue);
                }
            }
        }
        normal = normal.normalized;
        angle = Vector3.Angle(normal, -direction);
        
        return new DetectionResult{
            distance = distance,
            angle = angle,
            normal = normal,
            point = point,
            rb = rb
        };
    }
}
[System.Serializable]
public struct DetectionResult
{
    [NoEdit]public float distance;
    [NoEdit]public float angle;
    [NoEdit]public Vector3 normal;
    [NoEdit]public Vector3 point;
    [NoEdit]public Rigidbody rb;
    public static DetectionResult Default(float maxDistance) =>
        new DetectionResult(){
            distance = maxDistance,
            angle = 0,
            normal = Vector3.zero,
            point = Vector3.zero,
            rb = null
        };
/*     public void ResetToDefault(float maxDistance)
    {
        distance = maxDistance;
        angle = 0;
        normal = Vector3.zero;
        point = Vector3.zero;
        rb = null;
    } */
}
/* using UnityEngine;
using sapra.ObjectController;

[System.Serializable]
public class PSolidDetection : AbstractPassive
{
    public override PassivePriority whenDo => passivePlace;
    [SerializeField] private PassivePriority passivePlace = PassivePriority.FirstOfAll;
    private SDimensions _sDimension;
    private NAnimatorParameters _nAnimatorParameters;

    [Tooltip("Ground Mask")]
    public LayerMask groundMask = 1<<0;
    [SerializeField] private bool debug = false;

    [Header("Results")]
    [NoEdit] public bool Walkable;
    [NoEdit] public float angleFront;
    [NoEdit] public float angle;
    [NoEdit] public float distance;
    [NoEdit] public Vector3 normal;
    [NoEdit] public Vector3 point;
    [NoEdit] public Rigidbody rbFound;
    private RaycastHit hit;

    private float AngleFrontLerped;
    
    protected override void AwakeRoutine()
    {
        AnimationModule animationModule = controller.RequestModule<AnimationModule>();
        StatModule statModule = controller.RequestModule<StatModule>();
        _sDimension = statModule.RequestRoutine<SDimensions>(true);
        _nAnimatorParameters = animationModule.RequestRoutine<NAnimatorParameters>(false);
    }
    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        Vector3 topPosition = position+_sDimension.footOffset+_sDimension.currentHeight*2*-motor.gravityDirection;
        float maxDistance = _sDimension.currentHeight*3;
        float radious = (_sDimension.currentRadious-0.05f);
        Vector3 hitPoint = position;
        float distanceToSurface = 0;
        Vector3 normalVector = Vector3.zero;
        RaycastHit hit;
        if(debug)
            Debug.DrawRay(topPosition, motor.gravityDirection*maxDistance, Color.black);
        if(Physics.Raycast(topPosition, motor.gravityDirection, out hit, maxDistance, groundMask))
        {
            normalVector = hit.normal;
            hitPoint = (hit.point);
            distanceToSurface = hit.distance; 
        }
        else
        {
            Walkable = false;
            distance = maxDistance;
            return;
        }
        rbFound = hit.rigidbody;
        if(debug)
            Debug.DrawRay(topPosition, motor.gravityDirection, Color.blue);
        if(Physics.SphereCast(topPosition, radious, motor.gravityDirection, out hit, maxDistance, groundMask))
        {
            if(Vector3.Angle(hit.normal, motor.gravityDirection) < _sDimension.maxWalkableAngle)
            {
                normalVector = (normalVector +hit.normal)/2;
                Vector3 direction = topPosition-hit.point;
                float dot = Vector3.Dot(direction, -motor.gravityDirection);
                distanceToSurface = (distanceToSurface+dot)/2; 
                hitPoint = (hitPoint+(hit.point))/2;
                if(debug)
                    Debug.DrawRay(hit.point, hit.normal, Color.blue);
            }
        }
        
        distanceToSurface = (distanceToSurface-_sDimension.currentHeight*2)/_sDimension.currentHeight;
        
        normal = normalVector.normalized;
        point = hitPoint;
        distance = distanceToSurface; 
        angleFront = Vector3.Angle(normal, transform.forward) - 90;
        angle = Vector3.Angle(normal, -motor.gravityDirection);  
        if(debug)
            Debug.DrawRay(topPosition, normal, Color.red);
        
        Walkable = distance <= 0.1f && angle < _sDimension.maxWalkableAngle;
        if(Walkable)
        {
            if(debug)
                Debug.DrawRay(hitPoint, normal*2, Color.blue);
        }
        else
        {
            angleFront = 0;
            angle = 0;
        }
        
    }
    public override void DoAnimationParameters()
    {
        if(_nAnimatorParameters == null)
            return;
        AngleFrontLerped = Mathf.Lerp(AngleFrontLerped, angleFront, Time.deltaTime*8);
        _nAnimatorParameters.SetValue(AngleFrontLerped, "Angle");
        _nAnimatorParameters.SetValue(distance, "DistanceGround");
        _nAnimatorParameters.SetValue(Walkable, "Ground");
    }
}
public class DetectionResult
{
    [NoEdit] public float distance;
    [NoEdit] public float angle;
    [NoEdit] public Vector3 normal;
    [NoEdit] public Vector3 point;
} */