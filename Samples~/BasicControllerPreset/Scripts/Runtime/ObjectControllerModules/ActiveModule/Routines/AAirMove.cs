using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Simple")]
public class AAirMove : ActiveRoutine
{
    public override int priorityID => 1;
    private PGroundDetection _pGroundDetection;
    private PDirectionManager _pDirectionManager;
    public float rotationSpeed;
    public float MovementSpeed = .8f;
    private Vector3 refHorizontal;
    private CMotor motor;

    public override void UpdateActive(InputValues input)
    {
        Vector3 simplified = _pDirectionManager.GetGlobalInput();
        Vector3 horizontal = motor.newVelocity - Vector3.Project(motor.newVelocity, -motor.gravityDirection);
        Vector3 originalHorizontal = horizontal;
        if(simplified != Vector3.zero) 
        {
            horizontal = Vector3.SmoothDamp(horizontal, simplified*horizontal.magnitude, ref refHorizontal, MovementSpeed);
            if(horizontal.magnitude < 1f)
                horizontal = simplified;
            horizontal = horizontal-Vector3.Project(horizontal, -motor.gravityDirection);
            horizontal = Vector3.ClampMagnitude(horizontal, Mathf.Max(originalHorizontal.magnitude, simplified.magnitude));
        }
        //float amount = Mathf.Clamp(1-_pGroundDetection.NormalizedDistance, 0.2f, 1);
        
        Vector3 finalVelocity = horizontal+Vector3.Project(motor.newVelocity,-motor.gravityDirection);
        _pDirectionManager.RotateBody(rotationSpeed);
        motor.SetVelocity(finalVelocity);
    }

    public override bool WantActive(InputValues input)
    {
        if(input._inputVector != Vector2.zero && !_pGroundDetection.Walkable)
            return true;
        else
        {
            refHorizontal = Vector3.zero;
            return false;        
        }
    }

    protected override void Awake()
    {
        PassiveModule passiveModule = GetModule<PassiveModule>();
        _pGroundDetection = passiveModule.GetRoutine<PGroundDetection>(true);
        _pDirectionManager = passiveModule.GetRoutine<PDirectionManager>(true);
        motor = GetComponent<CMotor>(true);
    }
}