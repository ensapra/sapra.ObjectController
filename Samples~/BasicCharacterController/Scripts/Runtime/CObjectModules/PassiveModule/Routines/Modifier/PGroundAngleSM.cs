using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PGroundAngleSM : AbstractPassive
    {
        private PWalkableDetection _pWalkableDetection;
        private SDimensions _sDimensions;
        private Stat desiredVelocity;
        private Stat maximumVelocity;

        public override PassivePriority whenDo => PassivePriority.FirstOfAll;

        protected bool activeFactor()
        {
            if(_pWalkableDetection == null)
                return false;

            if(_pWalkableDetection.Walkable)
                if(_pWalkableDetection.angle != 0)
                    return true;                
            return false;
        }
        protected override void AwakeComponent(AbstractCObject cObject)        {
            PassiveModule passiveModule = cObject.FindModule<PassiveModule>();
            StatModule statModule = cObject.FindModule<StatModule>();
            _pWalkableDetection = passiveModule.RequestComponent<PWalkableDetection>(false);
            _sDimensions = statModule.RequestComponent<SDimensions>(false);
            maximumVelocity = statModule.RequestComponent<SForces>(true).maximumSpeed.Select();
            desiredVelocity = statModule.RequestComponent<SForces>(true).selectedSpeed.Select();
        }

        public override void DoPassive(Vector3 position, InputValues input)
        {
            if(activeFactor())
            {
                float _factor = maximumVelocity.value;
                if (_pWalkableDetection.angleFront > _sDimensions.startDecreaseVelAngle)        
                    _factor = _factor*(1-_pWalkableDetection.angle.Remap(_sDimensions.startDecreaseVelAngle,_sDimensions.maxWalkableAngle,0,1));
                desiredVelocity.createBoundaries(0,_factor);        
            }
        }
    }
}
