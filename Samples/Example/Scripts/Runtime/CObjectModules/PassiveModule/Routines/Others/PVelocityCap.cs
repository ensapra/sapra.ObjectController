using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class PVelocityCap : AbstractPassive
    {
        public override PassivePriority whenDo => PassivePriority.LastOne;
        private PWalkableDetection _pWalkableDetection;
        [Tooltip("Maximum allowed velocity on the Object")]
        public float maxVelocity = -50;
        InputValues _input;

        protected override void AwakeComponent(CObject cObject)
        {
            _pWalkableDetection = cObject.passiveModule.RequestComponent<PWalkableDetection>(false);
            _input = cObject.RequestComponent<InputValueHolder>(true).input;
        }
        public override void DoPassive(Vector3 position, InputValues input)
        {
            float value = Vector3.Dot(rb.velocity, -cObject.gravityDirection);
            if (value < maxVelocity)        
                rb.velocity = rb.velocity - Vector3.Project(rb.velocity, cObject.gravityDirection) - maxVelocity*cObject.gravityDirection;
            
            if(_pWalkableDetection == null || _input == null)
                return;        
                
            if(_pWalkableDetection != null &&_pWalkableDetection.Walkable && cObject.activeModule.currentAction == null)
                if(!_pWalkableDetection.rbFound)                
                    rb.velocity = Vector3.zero;                
        }

    }
}
