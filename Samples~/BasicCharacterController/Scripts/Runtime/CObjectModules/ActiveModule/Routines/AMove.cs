using UnityEngine;


namespace sapra.ObjectController
{
    [System.Serializable]
    public class AMove : AbstractActive
    {
        public override int priorityID => 0;
        private PWalkableDetection _pWalkableDetection;
        private PDirectionManager _pDirectionManager;
        private Stat normalVelocity;
        private Stat sprintVelocity;
        private Stat walkVelocity;
        private Stat desiredVelocity;
        private SForces _sForces;
        private bool overriden;
        protected override void AwakeComponent(CObject cObject)
        {
            _pWalkableDetection = cObject.passiveModule.RequestComponent<PWalkableDetection>(true);
            _pDirectionManager = cObject.passiveModule.RequestComponent<PDirectionManager>(true);
            cObject.passiveModule.RequestComponent<PVelocityCap>(true);
            SForces _sForces = cObject.statModule.RequestComponent<SForces>(true);
            sprintVelocity = _sForces.maximumSpeed;
            walkVelocity = _sForces.minimumSpeed.Select();
            normalVelocity = _sForces.passiveSpeed.Select();
            desiredVelocity = _sForces.selectedSpeed.Select();
        }
        public void setVelocity(Stat velocityValue)
        {
            desiredVelocity.changeValue(velocityValue.value);
            overriden = true;
        }
        public override void DoActive(InputValues _input)
        {
            if(!overriden)
            {
                if(_input._wantRun)
                    setVelocity(sprintVelocity); 
                else  if(_input._wantWalk)
                    setVelocity(walkVelocity);
                else
                    setVelocity(normalVelocity);
            }
            overriden = false;
            float clampedMagnitude = Mathf.Clamp(_input._inputVector.magnitude,0,1); 
            
            Vector3 movementDirection = _pDirectionManager.GetDirection(_input);
            rb.velocity =  Vector3.ClampMagnitude(movementDirection * clampedMagnitude*desiredVelocity.value,desiredVelocity.value);
            _pDirectionManager.RotateBody(-cObject.gravityDirection, _input);      
        }
        public override bool WantActive(InputValues input)
        {
            if(input._inputVector != Vector2.zero && _pWalkableDetection.Walkable)
                return true;
            else
                return false;
        }
    }
}
