using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PVelocityCap : AbstractPassive
    {
        public override PassivePriority whenDo => PassivePriority.LastOne;
        private PWalkableDetection _pWalkableDetection;
        [Tooltip("Maximum allowed velocity on the Object")]
        public float maxVelocity = -50;
        InputValues _input;
        ActiveModule activeModule;
        protected override void AwakeRoutine(AbstractCObject controller)        {
            activeModule = controller.RequestModule<ActiveModule>();
            _pWalkableDetection = controller.RequestModule<PassiveModule>().RequestRoutine<PWalkableDetection>(false);
            _input = controller.RequestComponent<InputValueHolder>(true).input;
        }
        public override void DoPassive(Vector3 position, InputValues input)
        {
            float value = Vector3.Dot(rb.velocity, -controller.gravityDirection);
            if (value < maxVelocity)        
                rb.velocity = rb.velocity - Vector3.Project(rb.velocity, controller.gravityDirection) - maxVelocity*controller.gravityDirection;
            
            if(_pWalkableDetection == null || _input == null)
                return;        
                
            if(_pWalkableDetection != null &&_pWalkableDetection.Walkable && activeModule.currentAction == null)
                if(!_pWalkableDetection.rbFound)                
                    rb.velocity = Vector3.zero;                
        }

    }
}
