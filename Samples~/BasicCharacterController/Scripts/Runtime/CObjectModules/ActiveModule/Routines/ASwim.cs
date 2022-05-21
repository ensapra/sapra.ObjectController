using UnityEngine;

using sapra.ObjectController;
namespace sapra.ObjectController
{
    [System.Serializable]
    public class ASwim : AbstractActive
    {
        public override int priorityID => 2;

        private PFloatDetection _pFloatDetection;
        private PWalkableDetection _pWalkableDetection;
        private PDirectionManager _pDirectionManager;
        private SDimensions _sDim;
        private SForces _sForces;
        private PColliderSettings _pColliderSettings;
        private Stat swimVelocity;
        private Stat sprintSwimVelocity;
        private Stat currentSpeed;
        private Stat desiredVelocity;

        private Vector3 finalDirection;
        private bool insideWater;
        protected override void AwakeRoutine(AbstractCObject controller)        {
            PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
            StatModule statModule = controller.RequestModule<StatModule>();
            _pFloatDetection = passiveModule.RequestRoutine<PFloatDetection>(true);
            _pWalkableDetection = passiveModule.RequestRoutine<PWalkableDetection>(true);
            _pColliderSettings = passiveModule.RequestRoutine<PColliderSettings>(true);
            _pDirectionManager = passiveModule.RequestRoutine<PDirectionManager>(true);
            passiveModule.RequestRoutine<PExtraGravity>(true);
            _sDim = statModule.RequestRoutine<SDimensions>(true);
            _sForces = statModule.RequestRoutine<SForces>(true);
            swimVelocity = _sForces.minimumWaterSpeed.Select();
            sprintSwimVelocity = _sForces.maximumWaterSpeed.Select();
            desiredVelocity = _sForces.selectedSpeed.Select();
        }
        private void setVelocity(Stat velocityValue)
        {
            desiredVelocity.changeValue(velocityValue.value);
        }
        public override void DoActive(InputValues _input)
        {            
            rb.velocity = finalDirection;
            _pDirectionManager.RotateBody(-controller.gravityDirection, _input);
        }
        public Vector3 removeOnBoundary(Vector3 direction, Vector3 boundaryNormal, float amount)
        {
            float dot = Vector3.Dot(direction.normalized, boundaryNormal);
            if(dot < 0)               
                direction = direction - Vector3.Project(direction, boundaryNormal)*amount;
            return direction;
        }
        public Vector3 simplifyVertical(Vector3 direction, bool alsoApply)
        {
            Vector3 finalProj = Vector3.Project(direction, _pFloatDetection.normal);
            Vector3 velProj = Vector3.Project(rb.velocity, _pFloatDetection.normal);
            float dir = Vector3.Dot(finalProj, velProj);
            if(velProj.sqrMagnitude > finalProj.sqrMagnitude && (_pFloatDetection.surfaceState == SurfaceStates.surface || alsoApply))
            {
                return direction-finalProj+velProj;
            }
            else
            {
               return direction;
            }
        }
        public Vector3 getWaterDirection(InputValues input)
        {
            float upDown = input._upDownRaw;
            float cameraInducedVertical = input._extraCameraInducedVertical;
            Vector3 direction;
            direction = _pDirectionManager.getLocalDirection();
            direction += (upDown+cameraInducedVertical)*-controller.gravityDirection;
            direction = removeExtras(direction);
            return direction.normalized;
        }
        public Vector3 removeExtras(Vector3 initialDirection)
        {
            Vector3 finalVector = initialDirection;
            if(_pFloatDetection.distance != 1)
            {
                finalVector = removeOnBoundary(finalVector, controller.gravityDirection, 1);
                finalVector = removeOnBoundary(finalVector, -_pFloatDetection.normal, 1);
            }
            return finalVector;
        }
        public void ProcessFinalDirection(InputValues input, Vector3 inputDirection)
        {
            if(input._wantRun)
                setVelocity(sprintSwimVelocity);
            else
                setVelocity(swimVelocity);

            float inputAmount = Mathf.Clamp(input._inputVectorRaw.magnitude + Mathf.Abs(input._upDownRaw), 0,1);
            inputDirection = Vector3.ClampMagnitude(inputDirection.normalized*inputAmount*desiredVelocity.value, desiredVelocity.value);
            finalDirection = Vector3.Lerp(finalDirection, inputDirection, Time.deltaTime*10);
            Vector3 position = transform.position-_sDim.currentRadious*controller.gravityDirection;
            position += inputDirection.normalized*_sDim.currentRadious*0.8f;
            RaycastHit hit;
            if(Physics.Raycast(position, inputDirection, out hit, 1, _pWalkableDetection.groundMask)) 
            {finalDirection = removeOnBoundary(finalDirection, hit.normal, (1-hit.distance));}
            finalDirection = simplifyVertical(finalDirection, hit.point != Vector3.zero);
        }
        public override bool WantActive(InputValues input)
        {
            Vector3 inputDirection = getWaterDirection(input);
            if(_pFloatDetection.surfaceState == SurfaceStates.inside)
            {
                if(_pColliderSettings != null)
                    ChangeCollider();
                ProcessFinalDirection(input, inputDirection);
                if(inputDirection.sqrMagnitude > 0)
                    return true;
            }
            else
                finalDirection = rb.velocity;
            return false;
        }
        public void ChangeCollider()
        {
            _pColliderSettings.ChangeSettings(_sDim.halfHeight, _sDim.halfHeight);
        }
    }
}
