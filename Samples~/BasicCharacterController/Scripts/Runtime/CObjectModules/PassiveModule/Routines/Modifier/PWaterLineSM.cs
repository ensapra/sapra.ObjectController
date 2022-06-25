using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PWaterLineSM : AbstractPassive
    {
        private PFloatDetection _pFloatDetection;
        private SDimensions _sDimensions;
        private Stat speedBeforeSwim;
        private Stat maximumVelocity;
        private Stat desiredVelocity;
        public override PassivePriority whenDo => PassivePriority.FirstOfAll;

        protected bool activeFactor()
        {
            if(_pFloatDetection == null)
                return false;
            if(_pFloatDetection.surfaceState == SurfaceStates.surface)
                return true;
            return false;
        }

        protected override void AwakeRoutine(AbstractCObject controller)        {
            PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
            StatModule statModule = controller.RequestModule<StatModule>();
            _pFloatDetection = passiveModule.RequestRoutine<PFloatDetection>(false);
            _sDimensions = statModule.RequestRoutine<SDimensions>(false);
            SForces forces = statModule.RequestRoutine<SForces>(true);
            speedBeforeSwim = forces.minimumSpeed.Select();
            maximumVelocity = forces.maximumSpeed.Select();
            desiredVelocity = forces.selectedSpeed.Select();
        }

        public override void DoPassive(Vector3 position, InputValues input)
        {
            if(activeFactor())
            {
                float _factor = 1-Mathf.InverseLerp(0, _sDimensions.shoulderLevel,_pFloatDetection.distance);
                _factor = Mathf.Clamp(_factor, 0 ,1);
                _factor = ((maximumVelocity.value-speedBeforeSwim.value)*_factor)+speedBeforeSwim.value;
                desiredVelocity.createBoundaries(0,_factor);        
            }
        }
    }
}
