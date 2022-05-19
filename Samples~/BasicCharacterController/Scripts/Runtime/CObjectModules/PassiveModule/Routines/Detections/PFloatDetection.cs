using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace sapra.ObjectController
{
    public enum SurfaceStates{inside, surface, none}
    [System.Serializable]
    public class PFloatDetection : AbstractPassive
    {
        public override PassivePriority whenDo => passivePlace;
        [SerializeField] private PassivePriority passivePlace = PassivePriority.FirstOfAll;
        private SDimensions _sDimensions;
        private PWalkableDetection _pWalkableDetection;
        public LayerMask floatingMask = 1<<4;

        public bool debug = false;

        [Header("Results")]
        public float distance;

        public bool floating{get{return surfaceState == SurfaceStates.inside || (_pWalkableDetection?.Walkable != true && surfaceState == SurfaceStates.surface);}}

        [HideInInspector] public Vector3 normal;
        public SurfaceStates surfaceState;
        protected override void AwakeComponent(CObject cObject)
        {
            _sDimensions = cObject.statModule.RequestComponent<SDimensions>(true);
            _pWalkableDetection = cObject.passiveModule.RequestComponent<PWalkableDetection>(false);
        }

        public override void DoPassive(Vector3 positionIni, InputValues input)
        {
            float maxDistance = (_sDimensions.characterHeight)*1.2f;
            Vector3 position = positionIni+_sDimensions.forcesCenterOffset+(_sDimensions.halfHeight)*-cObject.gravityDirection;
            float radius = _sDimensions.characterRadious;
            Vector3 normalVector = Vector3.zero;
            float distanceToSurface = maxDistance;
            RaycastHit hit;

            if(debug)
                Debug.DrawRay(position,cObject.gravityDirection*maxDistance, Color.black);

            if(Physics.Raycast(position, cObject.gravityDirection, out hit, maxDistance, floatingMask))
            {
                if(hit.distance < distanceToSurface)
                {
                    normalVector = hit.normal;
                    distanceToSurface = hit.distance; 
                }     
            }
            distanceToSurface = (_sDimensions.characterHeight-distanceToSurface)/(_sDimensions.characterHeight);
            normal = normalVector;
            distance = distanceToSurface;

            //More stable with big objects
            if(distance > 0)  
            {      
                if(distance < _sDimensions.shoulderLevel*.9f)
                    surfaceState = SurfaceStates.surface;   
                else
                    surfaceState = SurfaceStates.inside;
            }                          
            else
            {
                Collider[] coll = Physics.OverlapSphere(position, radius/10, floatingMask, QueryTriggerInteraction.Collide);
                if(coll.Length > 0) 
                {                       
                    surfaceState = SurfaceStates.inside;
                    distance = 1;
                }
                else   
                {
                    if(distance <= -0.14)
                        surfaceState = SurfaceStates.none;
                    else
                        surfaceState = SurfaceStates.surface;      
                }  
            } 
            
            if(debug)
            {
                switch(surfaceState)
                {
                    case SurfaceStates.surface:
                        Debug.DrawRay(position, Vector3.up, Color.yellow);
                        break;
                    case SurfaceStates.inside:
                        Debug.DrawRay(position, Vector3.up, Color.blue);
                        break;
                    case SurfaceStates.none:
                        Debug.DrawRay(position, Vector3.up, Color.red);
                        break;
                }
            }
        }
    }
}

