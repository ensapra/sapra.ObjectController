using System.Collections;
using UnityEngine;


namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class AJump : AbstractActive
    {
        public override int priorityID => 20;

        [Tooltip("Time frame before disallowing jumping when not touching the ground")]
        public float ghostJumpTreshold = 0.5f;
        private SDimensions _sDimensions;
        private PWalkableDetection _pWalkableDetection;
        private PRoofDetection _pRoofDetection;
        private PFloatDetection _pFloatDetection;
        private PDirectionManager _pDirectionManager;

        public enum jumpingSequence{readyToJump, jumping, falling}
        public jumpingSequence sequence = jumpingSequence.readyToJump;
        private Coroutine ghostJumpTimerCor;
        private Coroutine jumpingTimerCor;
        private float jumpsMade = 0;
        private bool jumpFinished;
        private bool animationPlayed;
        private Stat maxFJump;
        private Stat maxJumpVelocity;
        private Stat minJumpVelocity;

        private Vector3 latestVelocity;
        protected override void AwakeComponent(CObject cObject)
        {
            _sDimensions = cObject.statModule.RequestComponent<SDimensions>(true);
            _pWalkableDetection = cObject.passiveModule.RequestComponent<PWalkableDetection>(true);
            _pRoofDetection = cObject.passiveModule.RequestComponent<PRoofDetection>(true);
            _pFloatDetection =  cObject.passiveModule.RequestComponent<PFloatDetection>(false);
            _pDirectionManager = cObject.passiveModule.RequestComponent<PDirectionManager>(true);
            SForces _sForces = cObject.statModule.RequestComponent<SForces>(true);
            maxFJump = _sForces.maxFJump.Select();
            maxJumpVelocity = _sForces.maximumSpeed.Select();
            minJumpVelocity = _sForces.minimumSpeed.Select();
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
                    cObject.StopCoroutine(jumpingTimerCor);
                //Ghost Jump
                if(ghostJumpTimerCor != null)
                {
                    cObject.StopCoroutine(ghostJumpTimerCor);
                    ghostJumpTimerCor = null;
                }
                Vector3 direction = _pDirectionManager.getLocalDirection();
                Vector3 jumpVector = JumpVector(direction.magnitude*transform.forward)*(1-jumpsMade.Remap(0,5,0,0.7f));
                jumpingTimerCor = cObject.StartCoroutine(jumpingTimer());
                jumpVector += maxFJump.value * -cObject.gravityDirection;
                rb.velocity = jumpVector;
                sequence = jumpingSequence.jumping;     
                _pWalkableDetection.Walkable = false;   
            }
            //transform.RotateBody(1, jumpVector, transform.up, RotationMode.ForwardAndUpward);                   
        }

        public override bool WantActive(InputValues input)
        {
            if(_pFloatDetection?.floating == true)
                return false;
            bool final = false;
            if(input._wantToJump)
            {
                input._wantToJump = false;                
                if(sequence == jumpingSequence.readyToJump)
                    final = true;
            }
            if(_pRoofDetection.distance < _sDimensions.halfHeight)
                final = false;
            if(animationPlayed)
                final = true;
            return final;
        }

        public override void DoPassive()
        {
            if(_pWalkableDetection.Walkable)
            {
                if(sequence == jumpingSequence.falling || sequence == jumpingSequence.jumping ||jumpingTimerCor == null)
                    sequence = jumpingSequence.readyToJump;
                if(animationPlayed)
                    sequence = jumpingSequence.jumping;
            }
            else
            {
                float velVal = Vector3.Dot(rb.velocity, -cObject.gravityDirection);
                if(ghostJumpTimerCor == null && sequence == jumpingSequence.readyToJump)
                    ghostJumpTimerCor = cObject.StartCoroutine(ghostJump());
                if(sequence == jumpingSequence.jumping && velVal <= 0)
                    sequence = jumpingSequence.falling;
            }
        }
        IEnumerator jumpingTimer()
        {
            jumpsMade += 1;
            jumpsMade = Mathf.Clamp(jumpsMade, 0, 5);
            yield return new WaitForSeconds(0.2f);
            if(_pWalkableDetection.Walkable)
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
            Vector3 horizontal = latestVelocity- Vector3.Project(latestVelocity, -cObject.gravityDirection);
            float reference = horizontal.magnitude;
            float minVel = minJumpVelocity.value+1;
            float maxVel = maxJumpVelocity.value+5;
            float minimumVel = minVel/2;

            float desiredVelocity = Mathf.Clamp(reference, minVel, maxVel);
            if(reference < minimumVel)
                desiredVelocity = minimumVel;
            Vector3 finalVector = (desiredDirectionSimple-Vector3.Project(desiredDirectionSimple, -cObject.gravityDirection)).normalized*desiredVelocity;
            float angleFront = Vector3.Angle(transform.forward, _pWalkableDetection.normal)-90;
            if(angleFront > _sDimensions.maxWalkableAngle)
                finalVector = (_pWalkableDetection.normal-Vector3.Project(_pWalkableDetection.normal, -cObject.gravityDirection)).normalized*minimumVel;
            return finalVector;
        }
    }
}
