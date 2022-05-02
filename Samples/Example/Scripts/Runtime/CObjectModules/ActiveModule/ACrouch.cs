using UnityEngine;

namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class ACrouch : AbstractActive
    {
        [Tooltip("Character crouch height")]
        public float crouchHeight = 0.5f;
        [Tooltip("Character crouch radius")]
        public float crouchRadius = 0.5f;
        private Stat crouchVelocity;
        private Stat desiredVelocity;

        private AMove _aMove;
        private PRoofDetection _pRoofDetection;
        private PWalkableDetection _pWalkableDetection;
        private PColliderSettings _pColliderSettings;
        private PFloatDetection _pFloatDetection;
        private SDimensions _sDimensions;
        private float heightLerped; 
        public override int priorityID => 10;
        protected override void AwakeComponent(CObject cObject)
        {
            _aMove = cObject.activeModule.RequestComponent<AMove>(true);
            _pWalkableDetection = cObject.passiveModule.RequestComponent<PWalkableDetection>(true);
            _pRoofDetection = cObject.passiveModule.RequestComponent<PRoofDetection>(true);
            _pColliderSettings = cObject.passiveModule.RequestComponent<PColliderSettings>(true);
            _pFloatDetection = cObject.passiveModule.RequestComponent<PFloatDetection>(false);
            _sDimensions = cObject.statModule.RequestComponent<SDimensions>(true);
            crouchVelocity = cObject.statModule.RequestComponent<SForces>(true).minimumSpeed.Select();
            desiredVelocity = cObject.statModule.RequestComponent<SForces>(true).selectedSpeed.Select();
        }

        public override void DoActive(InputValues input)
        {
            _aMove.setVelocity(crouchVelocity);
            _aMove.DoActive(input);
            if(_pColliderSettings != null)
                _pColliderSettings.ChangeSettings(crouchHeight, crouchRadius);    
        }

        public override void DoPassive()
        {
            if(_pRoofDetection.topWall && _pRoofDetection.distance > crouchHeight*1.2f)
            {
                float finalDistance = Mathf.Clamp(_pRoofDetection.distance, crouchHeight, _sDimensions.characterHeight);
                _pColliderSettings.ChangeSettings(finalDistance, crouchRadius); 
            }
        }
        public override bool WantActive(InputValues input)
        {
            if(!_pWalkableDetection.Walkable)
                return false;
            if(_pFloatDetection != null && _pFloatDetection.distance > crouchHeight*0.8f)
                return false;
            if(input._wantCrouch && !input._wantRun)
                return true;

            if(_pRoofDetection.topWall && _pRoofDetection.distance < crouchHeight*1.2f)
                return true;          
            
            return false;
        }
        
        public override void DoAnimationParameters()
        {
            float distanceRemaped = _pRoofDetection.distance.Remap(crouchHeight,1,0, 1);
            if(isActive)
                heightLerped = Mathf.Lerp(heightLerped, 0, Time.deltaTime*10);
            else
                heightLerped = Mathf.Lerp(heightLerped, distanceRemaped, Time.deltaTime*10);
        }
    }
}
