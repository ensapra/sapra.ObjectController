using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PEasyWalk : AbstractPassive
    {
        private PWalkableDetection _pWalkableDetection;
        private PFloatDetection _pFloatDetection;

        private PColliderSettings _pColliderSettings;
        private SDimensions _sDimensions;
        [Tooltip("Max Slope teleport height")]
        public float slopeHeight = .5f;
        [Tooltip("Slope smoothness")]
        public float smoothness = 10;
        public override PassivePriority whenDo => PassivePriority.BeforeActive;
        
        protected override void AwakeComponent(AbstractCObject cObject)        {
            PassiveModule passiveModule = cObject.FindModule<PassiveModule>();
            _pWalkableDetection = passiveModule.RequestComponent<PWalkableDetection>(true);
            _pFloatDetection = passiveModule.RequestComponent<PFloatDetection>(false);
            _pColliderSettings = passiveModule.RequestComponent<PColliderSettings>(true);
            _sDimensions = cObject.FindModule<StatModule>().RequestComponent<SDimensions>(true);
        }

        public override void DoPassive(Vector3 position, InputValues input)
        {
            if(!_pWalkableDetection.Walkable)
            {
                _pColliderSettings.SetFactor(2-(_sDimensions.currentRadious/_sDimensions.currentHeight));
                return;
            }
            if(_pFloatDetection?.floating == true)
                return;
            Vector3 finalPos = position - Vector3.Project(position, -cObject.gravityDirection) + -cObject.gravityDirection*(_pWalkableDetection.point.y-_sDimensions.footOffset.y);
            transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime*smoothness);
            Vector3 deletedVelocity = Vector3.Project(-rb.velocity, _pWalkableDetection.normal);
            rb.velocity += deletedVelocity;
            if(_pWalkableDetection.rbFound)
                _pWalkableDetection.rbFound.AddForceAtPosition(-deletedVelocity*rb.mass/Time.deltaTime, position);
            
            float tempSlope = Mathf.Clamp(slopeHeight, 0.1f, _sDimensions.currentHeight*2-_sDimensions.characterRadious);
            _pColliderSettings.SetFactor(2-(tempSlope/_sDimensions.currentHeight));
        }
    }
}
