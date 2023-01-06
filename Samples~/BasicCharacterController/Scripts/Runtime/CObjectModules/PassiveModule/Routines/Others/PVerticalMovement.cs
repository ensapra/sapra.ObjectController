using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Others")]
public class PVerticalMovement : AbstractPassive
{
    private StatsContainer _statContainer;
    private PGroundDetection _pGroundDetection;
    public LayerMask ladderMask;

    public bool canVerticalMove{get{return _canVerticalMove;}}
    [SerializeField]private bool _canVerticalMove;
    [Header("Results")]
    [NoEdit] public Vector3 upVector;
    [NoEdit] public Vector3 normalVector;
    [NoEdit] public Vector3 pointOnTheLadder;
    private Vector3 ladderPosition;
    

    public float maxDetectionDistance = 2;
    public bool debug;
    

    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority == PassivePriority.BeforeActive)
        {
            position = position+_statContainer.FootOffset+_statContainer.CharacterHeight/2f*-controller.gravityDirection;
            RaycastHit hit;
            if(Physics.Raycast(position, transform.forward,out hit, maxDetectionDistance+_statContainer.CharacterRadius, ladderMask))
            {
                _canVerticalMove = true;
                normalVector = hit.transform.forward;
                upVector = hit.transform.up;
                ladderPosition = hit.transform.position;
                pointOnTheLadder = hit.point;
                if(Vector3.Dot(hit.normal, normalVector) < 0)
                {
                    Vector3 dir = hit.point-ladderPosition;
                    dir = Vector3.ProjectOnPlane(dir, upVector);
                    pointOnTheLadder -= dir*2;
                }
                if(debug)
                    Debug.DrawRay(hit.point, upVector, Color.black);
            }
            else
            {
                _canVerticalMove = false;
            }
        }
    }
    public void ClearParameters()
    {
        normalVector = Vector3.zero;
        upVector = Vector3.zero;
        ladderPosition = Vector3.zero;
    }
    public bool AmITop(out Vector3 topPosition)
    {
        Vector3 raycastPosition = transform.position+_statContainer.FootOffset+_statContainer.CharacterHeight*upVector;
        raycastPosition += -normalVector*_statContainer.CharacterRadius*3;
        RaycastHit topHit;
        if(debug)
            Debug.DrawRay(raycastPosition, -upVector.normalized*_statContainer.CharacterHeight, Color.green);
        if(Physics.Raycast(raycastPosition, -upVector, out topHit, _statContainer.CharacterHeight, _pGroundDetection.groundMask))
        {
            if(topHit.distance > _statContainer.CharacterHeight/2f)
            {                
                topPosition = topHit.point-_statContainer.FootOffset;
                return true;        
            }        
        }
        topPosition = Vector3.zero;
        return false;
    }
    public bool DoIStartTop()
    {
        Vector3 difference = transform.position-ladderPosition;
        if(Vector3.Dot(difference, upVector) > 0)
            return true;
        else
            return false;
    }
    public Vector3 changePosition(Vector3 position)
    {
        if(!_canVerticalMove)
            return position;

        Plane wall = new Plane(normalVector, pointOnTheLadder);
        Vector3 ladderPositionOnWall = wall.ClosestPointOnPlane(ladderPosition);
        Vector3 cross = Vector3.Cross(normalVector, upVector);
        Plane middlePoint = new Plane(cross, ladderPositionOnWall);
        return middlePoint.ClosestPointOnPlane(wall.ClosestPointOnPlane(position))+_statContainer.CharacterRadius*normalVector;
    }
    protected override void AwakeRoutine(AbstractCObject controller)
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _statContainer = controller.RequestComponent<StatsContainer>(true);
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
    }
}