using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    public enum RotationMode {ForwardAndUpward, DesiredAxisAndTarget}
    public enum MovementMode{ThirdPerson, FirstPerson}
    [System.Serializable]
    public class PDirectionManager : AbstractPassive
    {
        public override PassivePriority whenDo => PassivePriority.AfterActive;
        public MovementMode currentMovementType = MovementMode.ThirdPerson;
        private PWalkableDetection _pWalkableDetection;
        private Rigidbody rigidbody;
        [Tooltip("Direction reference to specify what is forward and what is right")]
        [SerializeField] private Transform directionalReference;
        private float xRotation;
        [Tooltip("Only active if it's in first person")]
        public Vector2 firstPersonCameraBoundary = new Vector2(-90, 90);

        private Vector3 localDirection;
        private Vector3 localDirectionRaw;
        public override void DoPassive(Vector3 position, InputValues input)
        {
            if(directionalReference == null)
                directionalReference = this.transform;
            Vector3 up = _pWalkableDetection != null ? _pWalkableDetection.normal : -cObject.gravityDirection;
            Vector3 forward = -Vector3.Cross(up, directionalReference.right);
            localDirectionRaw = input._inputVectorRaw.y*forward +input._inputVectorRaw.x*directionalReference.right;
            localDirection = input._inputVector.y*forward +input._inputVector.x*directionalReference.right;
        }
        public override void DoPassiveLate(Vector3 position, InputValues _input)
        {
            if(currentMovementType == MovementMode.FirstPerson)
            {
                Vector2 input = _input.mouseSensed;
                xRotation -= input.y;
                xRotation = Mathf.Clamp(xRotation, firstPersonCameraBoundary.x, firstPersonCameraBoundary.y);
                transform.Rotate(Vector3.up*input.x);
                directionalReference.transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
            }            
        }
        public void setReference(Transform transform)
        {
            directionalReference = transform;
        }
        protected override void AwakeComponent(AbstractCObject cObject)        {
            _pWalkableDetection = cObject.FindModule<PassiveModule>().RequestComponent<PWalkableDetection>(false);
            rigidbody = cObject.RequestComponent<Rigidbody>(true);
        }
        public Vector3 GetDirection(InputValues _input)
        {
            switch(currentMovementType)
            {
                //We are always moving forward
                case MovementMode.ThirdPerson:   
                {       
                    return correctForward();
                }
                //We move as the direction says
                case MovementMode.FirstPerson:  
                {  
                    return getLocalDirection();
                }
            }
            return transform.forward;
        }
        public Vector3 correctForward()
        {
            Vector3 groundNormal = _pWalkableDetection != null ? _pWalkableDetection.normal : -cObject.gravityDirection;      
            Vector3 normalForward = -Vector3.Cross(groundNormal, transform.right).normalized;
            return normalForward;
        }
        public Vector3 getLocalDirection()
        {
/*             Vector3 groundNormal = _pWalkableDetection != null ? _pWalkableDetection.normal : -cObject.gravityDirection; 
            Vector3 finalDir = Vector3.ProjectOnPlane(localDirection, groundNormal).normalized;        */         
            return localDirection;
        }
        public Vector3 getLocalDirectionRaw()
        {
/*             Vector3 groundNormal = _pWalkableDetection != null ? _pWalkableDetection.normal : -cObject.gravityDirection; 
            Vector3 finalDir = Vector3.ProjectOnPlane(localDirectionRaw, groundNormal).normalized;     */            
            return localDirectionRaw;
        }
        public void RotateBody(Vector3 upVector, InputValues _input)
        {
            switch(currentMovementType)
            {
                case MovementMode.ThirdPerson:
                    ForcedRotation(rb, RotationMode.ForwardAndUpward,  localDirectionRaw, -cObject.gravityDirection);
                    break;
    /*             case MovementMode.FirstPerson:
                    break;
                    Vector3 forward = Vector3.Cross(upVector, TargetPivot.right);
                    ForcedRotation(1, RotationMode.ForwardAndUpward,forward ,upVector);
                    break; */
            }
        }        
        public void ForcedRotation(Rigidbody rb,  RotationMode rotationMode, Vector3 vector1, Vector3 vector2)
        {
            //RotateBody so it changes a vector to a certain other vector with the smoothness set to velocity
            float sWalkRot = GetSWalKRot(rb);
            ForcedRotation(sWalkRot, rotationMode, vector1, vector2);
        }
        private Quaternion getRotation(Vector3 vector1, Vector3 vector2, RotationMode rotationMode)
        {
            Quaternion rotation = transform.rotation;
            switch(rotationMode)
            {
                case RotationMode.ForwardAndUpward:
                {
                    Vector3 projected = Vector3.ProjectOnPlane(vector1, vector2);
                    rotation = Quaternion.LookRotation(projected, vector2);
                    break;
                }
                case RotationMode.DesiredAxisAndTarget:
                {
                    rotation = Quaternion.FromToRotation(vector1, vector2)*transform.rotation;
                    break;
                }            
            }
            return rotation;
        }
            
        public void ForcedRotation(float speed, RotationMode rotationMode, Vector3 vector1, Vector3 vector2)
        {
            float sWalkRot = speed;
            if(vector1 == Vector3.zero)
                return;
            Quaternion rotation = getRotation(vector1, vector2, rotationMode);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, sWalkRot);
        }

        private float GetSWalKRot(Rigidbody rb)
        {           
            float pointSWalkRot = rb.velocity.magnitude.Remap(10,13, 0, 1);
            pointSWalkRot = Mathf.Clamp(pointSWalkRot, 0, 1);
            return (12-Mathf.Lerp(5f, 10f, pointSWalkRot))*Time.deltaTime;
        }
    }
}