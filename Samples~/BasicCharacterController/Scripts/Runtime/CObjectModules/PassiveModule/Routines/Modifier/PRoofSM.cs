using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PRoofSM : AbstractPassive
    {
        private PRoofDetection _pRoofDetection;
        private SDimensions _sDimensions;
        private Stat minimumVelocity;
        private Stat maximumVelocity;
        private Stat desiredVelocity;

        public override PassivePriority whenDo => throw new System.NotImplementedException();

        protected bool activeFactor()
        {
            if(_pRoofDetection == null)
                return false;
            if(_pRoofDetection.topWall)
                return true;
            return false;
        }

        protected override void AwakeRoutine(AbstractCObject controller)        {
            StatModule statModule = controller.RequestModule<StatModule>();
            _pRoofDetection = controller.RequestModule<PassiveModule>().RequestRoutine<PRoofDetection>(false);
            _sDimensions = statModule.RequestRoutine<SDimensions>(false);
            SForces forces = statModule.RequestRoutine<SForces>(true);
            minimumVelocity = forces.minimumSpeed.Select();
            maximumVelocity = forces.maximumSpeed.Select();
            desiredVelocity = forces.selectedSpeed.Select();
        }

        public override void DoPassive(Vector3 position, InputValues input)
        {
            if(activeFactor())
            {
                float _factor = 1-Mathf.InverseLerp(0.6f, _sDimensions.halfHeight, _pRoofDetection.distance);
                _factor = Mathf.Clamp(_factor, 0, 1);
                _factor = _factor*(maximumVelocity.value-minimumVelocity.value) + minimumVelocity.value;
                desiredVelocity.createBoundaries(0,_factor);        
            }
        }
    }
}
