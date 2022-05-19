using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class PExtraGravity : AbstractPassive
    {
        public override PassivePriority whenDo => passivePlace;
        [SerializeField] private PassivePriority passivePlace = PassivePriority.BeforeActive;
        [Header("Gravity multipliers")]
        [Range(1, 10)] public float fallSpeed = 8;
        [Tooltip("-1 does't float, falls full speed, 0 stays static, 1 floats at full speed if water resistance = 0)")]
        [Range(-1, 1)] public float buoyancy = 0.7f;

        [Header("Resistances")]
        [Range(1, 10)]public float airResistance = 5;
        [Range(1, 10)]public float waterResistance = 7f;
        private Vector3 gravity;
        private PFloatDetection _pFloatDetection;
        private PWalkableDetection _pWalkableDetection;
        private PDirectionManager _pDirectionManager;
        private PCustomGravity _pCustomGravity;
        private SDimensions _sDimensions;
        [Tooltip("Points of interest to modify Gravity")]
        public Vector3[] positions = new Vector3[]{Vector3.zero};
        protected override void AwakeComponent(CObject cObject)
        {
            _pFloatDetection = cObject.passiveModule.RequestComponent<PFloatDetection>(true);
            _pWalkableDetection = cObject.passiveModule.RequestComponent<PWalkableDetection>(true);
            _pCustomGravity = cObject.passiveModule.RequestComponent<PCustomGravity>(true);
            _sDimensions = cObject.statModule.RequestComponent<SDimensions>(true);
            _pDirectionManager = cObject.passiveModule.RequestComponent<PDirectionManager>(true);
        }
        public override void DoPassive(Vector3 position, InputValues input)
        {
            if(!_pCustomGravity.useGravity)
                return;
            gravity = cObject.gravityDirection*cObject.gravityMultiplier;
            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 pos = transform.TransformPoint(positions[i]);
                if(positions.Length > 1)
                    _pFloatDetection.DoPassive(pos, input);
                Gravities(pos, input);
            }
        }
        private void Gravities(Vector3 position, InputValues _input)
        {
            Vector3 ForceVector = Vector3.zero; 
            Vector3 hor = rb.velocity - Vector3.Project(rb.velocity,-cObject.gravityDirection);

            if(_pFloatDetection != null && _pFloatDetection.floating)
            {
                ForceVector += waterGravity(position);
                
                //Resistances
                float distance = Mathf.Clamp(_pFloatDetection.distance, 0, 1);
                ForceVector += (-hor*airResistance/10f)/positions.Length;
                ForceVector += distance*(-rb.velocity*waterResistance)/positions.Length;
                rb.angularVelocity -= (rb.angularVelocity*distance*(waterResistance/20))/positions.Length;
                
            }
            else if(_pWalkableDetection?.Walkable == true && _pWalkableDetection.rbFound == null)
            {
                if(_input == null)
                    return;
                GroundGravity(_pDirectionManager.getLocalDirection());
            }
            else
            {
                _pCustomGravity.useGravity = true;        
                ForceVector += (gravity*fallSpeed)/positions.Length;
                ForceVector += (-hor*airResistance/10f)/positions.Length;
            } 
            rb.AddForceAtPosition(ForceVector, position, ForceMode.Acceleration);
        }
        public Vector3 waterGravity(Vector3 position)
        {
            _pCustomGravity.useGravity = true;        
            Vector3 temporalResult = Vector3.zero;
            var targetValue = (_pFloatDetection.distance-(_sDimensions.shoulderLevel-0.5f));
            
            if(buoyancy > 0)
            {
                if(_pFloatDetection.surfaceState == SurfaceStates.surface && _pFloatDetection.distance < 0)
                {
                    float factor = (-1*_pFloatDetection.distance)/(_sDimensions.halfHeight*0.3f);
                    temporalResult += (gravity*factor*fallSpeed)/positions.Length;
                }
                else
                    temporalResult += (gravity*(1-targetValue)*fallSpeed*buoyancy)/positions.Length;
                
                temporalResult += (-gravity*targetValue*fallSpeed*buoyancy-gravity)/positions.Length;
            }
            else
                temporalResult += (-gravity*fallSpeed*buoyancy-gravity)/positions.Length;
            
            return temporalResult;
        }
        private void GroundGravity(Vector3 playerInput)
        {
            _pCustomGravity.useGravity = !(playerInput == Vector3.zero && rb.velocity.magnitude < 0.1f);        
        }
    }
}