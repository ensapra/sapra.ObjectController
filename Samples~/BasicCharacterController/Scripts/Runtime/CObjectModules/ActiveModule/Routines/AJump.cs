using System.Collections;
using UnityEngine;
using sapra.ObjectController;

[System.Serializable][RoutineCategory("Simple")]
public class AJump : AbstractActive
{
    public override int priorityID => 20;

    [Tooltip("Time frame before disallowing jumping when not touching the ground")]
    public float ghostJumpTreshold = 0.5f;
    private StatsContainer _statContainer;
    private PGroundDetection _pGroundDetection;
    private PRoofDetection _pRoofDetection;
    private PWaterDetection _pWaterDetection;
    private PDirectionManager _pDirectionManager;

    public enum jumpingSequence{readyToJump, jumping, falling}
    [NoEdit] public jumpingSequence sequence = jumpingSequence.readyToJump;
    private Coroutine ghostJumpTimerCor;
    private Coroutine jumpingTimerCor;
    private float jumpsMade = 0;
    private bool jumpFinished;
    private bool animationPlayed;

    private Vector3 latestVelocity;

    protected override void AwakeRoutine(AbstractCObject controller)
    {
        PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
        _pGroundDetection = passiveModule.RequestRoutine<PGroundDetection>(true);
        _pRoofDetection = passiveModule.RequestRoutine<PRoofDetection>(true);
        _pWaterDetection =  passiveModule.RequestRoutine<PWaterDetection>(false);
        _pDirectionManager = passiveModule.RequestRoutine<PDirectionManager>(true);
        _statContainer = controller.RequestComponent<StatsContainer>(true);        
    }

    public override void DoActive(InputValues _input)
    {  
        if(!animationPlayed)
        {
            animationPlayed = true;
            latestVelocity = rb.velocity;
            sequence = jumpingSequence.jumping;     
        }
        else
        {
            animationPlayed = false;
            //Reduce jump forward speed on multiple jumps, for bunny hopping
            if(jumpingTimerCor != null)
                controller.StopCoroutine(jumpingTimerCor);
            //Ghost Jump
            if(ghostJumpTimerCor != null)
            {
                controller.StopCoroutine(ghostJumpTimerCor);
                ghostJumpTimerCor = null;
            }
            Vector3 direction = _pDirectionManager.GetGlobalInput();
            Vector3 jumpVector = JumpVector(direction.magnitude*transform.forward)*(1-Mathf.InverseLerp(0,5, jumpsMade)*0.7f);
            jumpingTimerCor = controller.StartCoroutine(jumpingTimer());
            jumpVector += _statContainer.MaximumJumpForce * -controller.gravityDirection;
            rb.velocity = jumpVector;
            sequence = jumpingSequence.jumping;     
            _pGroundDetection.Walkable = false;   
        }
        //transform.RotateBody(1, jumpVector, transform.up, RotationMode.ForwardAndUpward);                   
    }

    public override bool WantActive(InputValues input)
    {
        if(_pWaterDetection?.Floating == true)
            return false;
        bool final = false;
        if(input._wantToJump)
        {
            input._wantToJump = false;                
            if(sequence == jumpingSequence.readyToJump)
                final = true;
        }
        if(_pRoofDetection.detectionResult.distance < _statContainer.CharacterHeight/2f)
            final = false;
        if(animationPlayed)
            final = true;
        return final;
    }

    public override void DoPassiveBeforeAction()
    {
        if(_pGroundDetection.Walkable)
        {
            if(sequence == jumpingSequence.falling || sequence == jumpingSequence.jumping ||jumpingTimerCor == null)
                sequence = jumpingSequence.readyToJump;
            if(animationPlayed)
                sequence = jumpingSequence.jumping;
        }
        else
        {
            float velVal = Vector3.Dot(rb.velocity, -controller.gravityDirection);
            if(ghostJumpTimerCor == null && sequence == jumpingSequence.readyToJump)
                ghostJumpTimerCor = controller.StartCoroutine(ghostJump());
            if(sequence == jumpingSequence.jumping && velVal <= 0)
                sequence = jumpingSequence.falling;
        }
    }
    IEnumerator jumpingTimer()
    {
        jumpsMade += 1;
        jumpsMade = Mathf.Clamp(jumpsMade, 0, 5);
        yield return new WaitForSeconds(0.2f);
        if(_pGroundDetection.Walkable)
            sequence = jumpingSequence.readyToJump;
        yield return new WaitForSeconds(0.4f);
        jumpsMade = 0;
        jumpingTimerCor = null;
    }
    IEnumerator ghostJump()
    {
        yield return new WaitForSeconds(ghostJumpTreshold);
        sequence = jumpingSequence.falling;
        ghostJumpTimerCor = null;
    }
    private Vector3 JumpVector(Vector3 desiredDirectionSimple)
    {
        //Velocitat i direcció del salt
        Vector3 horizontal = latestVelocity- Vector3.Project(latestVelocity, -controller.gravityDirection);
        float reference = horizontal.magnitude;
        float minVel = _statContainer.MinimumSpeed+1;
        float maxVel = _statContainer.MaximumSpeed+5;
        float minimumVel = minVel/2;

        float desiredVelocity = Mathf.Clamp(reference, minVel, maxVel);
        if(reference < minimumVel)
            desiredVelocity = minimumVel;
        Vector3 finalVector = (desiredDirectionSimple-Vector3.Project(desiredDirectionSimple, -controller.gravityDirection)).normalized*desiredVelocity;
        float angleFront = Vector3.Angle(transform.forward, _pGroundDetection.detectionResult.normal)-90;
        if(angleFront > _statContainer.WalkableAngle)
            finalVector = (_pGroundDetection.detectionResult.normal-Vector3.Project(_pGroundDetection.detectionResult.normal, -controller.gravityDirection)).normalized*minimumVel;
        return finalVector;
    }
}

