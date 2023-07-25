using UnityEngine;
using sapra.ObjectController;

[System.Serializable]
public class HSurfaceDetection : HelperRoutine
{
    private StatsContainer _statContainer;
    
    protected override void Awake()
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

}