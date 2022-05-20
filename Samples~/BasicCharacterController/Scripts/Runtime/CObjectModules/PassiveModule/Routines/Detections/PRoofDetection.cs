using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PRoofDetection : AbstractPassive
    {
        public override PassivePriority whenDo => passivePlace;
        [SerializeField] private PassivePriority passivePlace = PassivePriority.FirstOfAll;
        private SDimensions _sDimensions;
        public LayerMask topWallLayer = 1 << 0;
        
        [Header("Result")]
        public bool topWall;
        public float distance;
        protected override void AwakeComponent(AbstractCObject cObject)        {
            _sDimensions = cObject.FindModule<StatModule>().RequestComponent<SDimensions>(true);
        }

        public override void DoPassive(Vector3 position, InputValues input)
        {        
            float radius = _sDimensions.currentRadious-0.1f;
            Vector3 point = new Vector3(1,0,1);
            RaycastHit hit;      
            float tempDistance = _sDimensions.characterHeight;
            if(Physics.Raycast(position, -cObject.gravityDirection, out hit,_sDimensions.characterHeight,topWallLayer))  
                tempDistance = hit.distance;
            
            if(rb)
                position += rb.velocity*Time.deltaTime; 

            if(Physics.SphereCast(position, radius, -cObject.gravityDirection, out hit, _sDimensions.characterHeight,topWallLayer))
            {
                if(hit.distance < tempDistance)
                    tempDistance = hit.distance;
            }

            topWall = tempDistance < _sDimensions.characterHeight;
            distance = (tempDistance/2);
        }

    }
}