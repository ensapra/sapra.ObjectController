using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Simple")]
public class AAirMove : AbstractActive
{
    public override int priorityID => 1;
    private PGroundDetection _pGroundDetection;
    private PDirectionManager _pDirectionManager;
    public float rotationSpeed;

    private Vector3 refHorizontal;
    public override void DoActive(InputValues input)
    {
        Vector3 simplified = _pDirectionManager.GetMoveDirection(input);
        Vector3 horizontal = rb.velocity - Vector3.Project(rb.velocity, -controller.gravityDirection);
        if(simplified != Vector3.zero) 
        {
            horizontal = Vector3.SmoothDamp(horizontal, simplified*horizontal.magnitude, ref refHorizontal, .04f);
            if(horizontal.magnitude < 1f)
                horizontal = simplified;
            horizontal = horizontal-Vector3.Project(horizontal, -controller.gravityDirection);
        }
        rb.velocity = horizontal+Vector3.Project(rb.velocity,-controller.gravityDirection);
        float amount = Mathf.Clamp(1-_pGroundDetection.NormalizedDistance, 0.2f, 1);
        _pDirectionManager.RotateBody(rotationSpeed);
    }

    public override bool WantActive(InputValues input)
    {
        if(input._inputVector != Vector2.zero && !_pGroundDetection.Walkable)
            return true;
        else
            return false;        
    }

    protected override void AwakeRoutine(AbstractCObject controller)
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
        _pDirectionManager = passiveModule.RequestRoutine<PDirectionManager>(true);
    }
}