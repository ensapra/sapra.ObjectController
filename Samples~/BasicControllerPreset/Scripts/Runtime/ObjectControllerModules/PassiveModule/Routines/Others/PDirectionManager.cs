using UnityEngine;
using NaughtyAttributes;

public enum RotationMode {ForwardAndUpward, DesiredAxisAndTarget}

[System.Serializable][RoutineCategory("Others")]
public class PDirectionManager : AbstractPassive
{
    public enum RotateWith{
        RotateWithMouse,
        RotateWithDirection,
        NoRotation
    }
    public enum VerticalAxis{
        WithGround,
        WithGravity,
        NoTouch
    }

    public enum MovementMode{
        AlwaysForward,
        OnlyForwardAxis,
        AllDirections
    }

    public MovementMode movementMode = MovementMode.AlwaysForward;
    public RotateWith rotateType = RotateWith.RotateWithDirection;
    public VerticalAxis verticalAxis = VerticalAxis.NoTouch;

    private PGroundDetection _pGroundDetection;
    private float xRotation;
    [Tooltip("Only active RotateType is RotateWithMouse")]
    public Vector2 firstPersonCameraBoundary = new Vector2(-90, 90);

    private Vector3 globalDirection;
    private Vector3 globalDirectionRaw;
    private Vector3 referencedDirection;
    CMotor motor;

    [Tooltip("Direction reference to specify what is forward and what is right")]
    [AllowNesting][ReadOnly] public Transform directionalReference;
    
    public override void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input)
    {
        if(currentPassivePriority != PassivePriority.AfterActive)
            return;
        if(directionalReference == null)
            directionalReference = this.transform;
        Vector3 up = _pGroundDetection != null && _pGroundDetection.Walkable ? _pGroundDetection.detectionResult.normal : -motor.gravityDirection;
        Vector3 forward = -Vector3.Cross(up, directionalReference.right);
        globalDirectionRaw = input._inputVectorRaw.y*forward +input._inputVectorRaw.x*directionalReference.right;
        globalDirection = input._inputVector.y*forward +input._inputVector.x*directionalReference.right;
        referencedDirection = input._inputVector.y*directionalReference.forward +input._inputVector.x*directionalReference.right;
        switch(verticalAxis)
        {
            case VerticalAxis.WithGround:
                if(_pGroundDetection.Walkable)
                    ForcedRotation(0.1f,  RotationMode.DesiredAxisAndTarget, transform.up,_pGroundDetection.detectionResult.normal);
                break;
            case VerticalAxis.WithGravity:
                ForcedRotation(0.1f,  RotationMode.DesiredAxisAndTarget, transform.up,-motor.gravityDirection);
                break;
        }
    }
    public void setReference(Transform transform)
    {
        directionalReference = transform;
    }
    protected override void Awake()
    {
        PassiveModule passiveModule = GetModule<PassiveModule>();
        _pGroundDetection = passiveModule.GetRoutine<PGroundDetection>(false);
        motor = GetComponent<CMotor>(true);
    }

    public Vector3 GetMoveDirection(InputValues _input)
    {
        switch(movementMode)
        {
            case MovementMode.AlwaysForward:
                return CorrectForward();
            case MovementMode.AllDirections:
                return GetGlobalInput();
            case MovementMode.OnlyForwardAxis:
                return CorrectForward()*_input._inputVector.y;
        }
        return transform.forward;
    }
    public Vector3 CorrectForward()
    {
        Vector3 groundNormal = (_pGroundDetection != null && _pGroundDetection.Walkable) ? _pGroundDetection.detectionResult.normal : -motor.gravityDirection;      
        Vector3 normalForward = -Vector3.Cross(groundNormal, transform.right).normalized;
        return normalForward;
    }

    public Vector3 GetGlobalInput()
    {
        return globalDirection;
    }
    public Vector3 GetReferencedInput()
    {
        return referencedDirection;
    }
    public Vector3 GetGlobalInputRaw()
    {
        return globalDirectionRaw;
    }

    #region  RotationHandeling
    public void RotateBody(float speed)
    {
        switch(rotateType)
        {
            case RotateWith.RotateWithDirection:
                    ForcedRotation(speed, RotationMode.ForwardAndUpward,  globalDirectionRaw, -motor.gravityDirection);
                break;
        }
    }        
    private Quaternion getRotation(Vector3 vector1, Vector3 vector2, RotationMode rotationMode)
    {
        Quaternion rotation = transform.rotation;
        switch(rotationMode)
        {
            case RotationMode.ForwardAndUpward:
            {
                Vector3 projected = Vector3.ProjectOnPlane(vector1, vector2);
                rotation = Quaternion.LookRotation(projected, vector2);
                break;
            }
            case RotationMode.DesiredAxisAndTarget:
            {
                rotation = Quaternion.FromToRotation(vector1, vector2)*transform.rotation;
                break;
            }            
        }
        return rotation;
    }
    public void ForcedRotation(float speed, RotationMode rotationMode, Vector3 vector1, Vector3 vector2)
    {
        if(vector1 == Vector3.zero)
            return;
        Quaternion rotation = getRotation(vector1, vector2, rotationMode);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed);
    }

    public override void DoPassiveLate(Vector3 position, InputValues _input)
    {
        if(rotateType == RotateWith.RotateWithMouse)
        {
            Vector2 input = _input.mouseSensed;
            xRotation -= input.y;
            xRotation = Mathf.Clamp(xRotation, firstPersonCameraBoundary.x, firstPersonCameraBoundary.y);
            transform.Rotate(Vector3.up*input.x);
            directionalReference.transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        }            
    }
    #endregion
}