using System.Collections;
using UnityEngine;
using sapra.ObjectController;
using NaughtyAttributes;

[System.Serializable][RoutineCategory("Simple")]
public class AJump : ActiveRoutine
{
    public override int priorityID => 20;

    [Tooltip("Time frame before disallowing jumping when not touching the ground")]
    public float ghostJumpTreshold = 0.5f;
    private StatsContainer _statContainer;
    private PGroundDetection _pGroundDetection;
    private PRoofDetection _pRoofDetection;
    private PWaterDetection _pWaterDetection;
    private PDirectionManager _pDirectionManager;
    private CMotor motor;

    public enum JumpingState{ReadyToJump, StartedJump, Jumping, Falling}
    [DisableIf("True")] public JumpingState State = JumpingState.ReadyToJump;
    private Coroutine GhostJump;
    private Coroutine JumpCounter;
    private float jumpsMade = 0;
    private bool jumpFinished;
    private bool animationPlayed;
    private float StartedJumpTimer;


    protected override void Awake()
    {
        PassiveModule passiveModule = GetModule<PassiveModule>();
        _pGroundDetection = passiveModule.GetRoutine<PGroundDetection>(true);
        _pRoofDetection = passiveModule.GetRoutine<PRoofDetection>(true);
        _pWaterDetection =  passiveModule.GetRoutine<PWaterDetection>(false);
        _pDirectionManager = passiveModule.GetRoutine<PDirectionManager>(true);
        _statContainer = GetComponent<StatsContainer>(true);        
        motor = GetComponent<CMotor>(true);
    }
    public override void UpdateActive(InputValues _input)
    {
        if(State == JumpingState.ReadyToJump)
        {

            //Do the jump
            Vector3 desiredDirection = _pDirectionManager.GetMoveDirection(_input);
            Vector3 direction = JumpVector(desiredDirection,(1-Mathf.InverseLerp(0,5, jumpsMade)*0.7f));
            
            //Scale direction
            jumpsMade = Mathf.Min(jumpsMade+1, 5);
            jumpsMade = Mathf.Min(jumpsMade+1, 5);
            motor.SetVelocity(direction);
            State = JumpingState.StartedJump;
            StartedJumpTimer = 0;
            if(JumpCounter != null)
                StopCoroutine(JumpCounter);
        }
        if(State == JumpingState.StartedJump)
        {
            StartedJumpTimer += Time.deltaTime;
            if(StartedJumpTimer >= .2f)
            {
                State = JumpingState.Jumping;
                JumpCounter = StartCoroutine(JumpsClean());
            }
        }
    }
    IEnumerator JumpsClean()
    {
        yield return new WaitForSeconds(.4f);
        jumpsMade = 0;
        JumpCounter = null;
    }

    public override bool WantActive(InputValues input)
    {
        if(_pWaterDetection?.Floating == true)
            return false;
        if(_pRoofDetection.detectionResult.distance < _statContainer.CharacterHeight/2f)
            return false;

        if(input._wantToJump)
        {
            input._wantToJump = false;
            if(State == JumpingState.ReadyToJump)
                return true;
        }
        if(State == JumpingState.StartedJump)
            return true;

        return false;
    }

    public override void DoPassiveBeforeAction()
    {
        //Evaluate states
        if(_pGroundDetection.Walkable)
        {
            if(State == JumpingState.Jumping || State == JumpingState.Falling)
                State = JumpingState.ReadyToJump;
        }
        else
        {
            float velVal = Vector3.Dot(motor.velocity, -motor.gravityDirection);
            if(GhostJump == null && State == JumpingState.ReadyToJump)
                GhostJump = StartCoroutine(GhostJumpDelay());
            
            if(State == JumpingState.Jumping && velVal <= 0)
                State = JumpingState.Falling;
        }
    }
    IEnumerator GhostJumpDelay()
    {
        yield return new WaitForSeconds(ghostJumpTreshold);
        State = JumpingState.Falling;
        GhostJump = null;
    }
    private Vector3 JumpVector(Vector3 jumpDirection, float atenuator)
    {
        //Velocitat i direcció del salt
        Vector3 horizontal = Vector3.ProjectOnPlane(motor.velocity, -motor.gravityDirection);
        
        float reference = horizontal.magnitude;
        float lowVel = _statContainer.MinimumSpeed+1;
        float highVel = _statContainer.MaximumSpeed+5;
        float minVel = lowVel/2;

        //Set the velocity between the two possible values
        float desiredVelocity = Mathf.Clamp(reference, lowVel, highVel);
        if(reference < minVel)
            desiredVelocity = minVel;

        Vector3 finalVector = Vector3.ProjectOnPlane(jumpDirection, -motor.gravityDirection).normalized*desiredVelocity;
        
        if(_pGroundDetection.AngleFront > _statContainer.WalkableAngle)
            finalVector = Vector3.ProjectOnPlane(_pGroundDetection.detectionResult.normal, -motor.gravityDirection).normalized*minVel;
        
        //Add vertical component
        finalVector *= atenuator;
        finalVector += _statContainer.MaximumJumpForce * -motor.gravityDirection;
        return finalVector;
    }
}

