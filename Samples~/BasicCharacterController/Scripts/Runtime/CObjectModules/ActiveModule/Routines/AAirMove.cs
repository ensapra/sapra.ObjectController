using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class AAirMove : AbstractActive
    {
        public override int priorityID => 1;
        private PWalkableDetection _pWalkableDetection;
        private PDirectionManager _pDirectionManager;
        public override void DoActive(InputValues input)
        {
            Vector3 simplified = _pDirectionManager.GetDirection(input);
            Vector3 horizontal = rb.velocity - Vector3.Project(rb.velocity, -controller.gravityDirection);
            if(simplified != Vector3.zero) 
            {
                horizontal = Vector3.Lerp(horizontal, simplified*horizontal.magnitude, Time.deltaTime*4);
                if(horizontal.magnitude < 1f)
                    horizontal = simplified;
                horizontal = horizontal-Vector3.Project(horizontal, -controller.gravityDirection);
            }
            rb.velocity = horizontal+Vector3.Project(rb.velocity,-controller.gravityDirection);
            float amount = Mathf.Clamp(1-_pWalkableDetection.distance, 0.2f, 1);
            _pDirectionManager.RotateBody(-controller.gravityDirection, input);
            //transform.RotateBody(amount, RotationMode.ForwardAndUpward, horizontal, -controller.gravityDirection);
        }

        public override bool WantActive(InputValues input)
        {
            if(input._inputVector != Vector2.zero && !_pWalkableDetection.Walkable)
                return true;
            else
                return false;        
        }

        protected override void AwakeRoutine(AbstractCObject controller)        {
            PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
            _pWalkableDetection = passiveModule.RequestRoutine<PWalkableDetection>(true);
            _pDirectionManager = passiveModule.RequestRoutine<PDirectionManager>(true);
        }
    }
}
