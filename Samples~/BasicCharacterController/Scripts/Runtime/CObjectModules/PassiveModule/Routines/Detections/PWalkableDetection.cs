using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PWalkableDetection : AbstractPassive
    {
        public override PassivePriority whenDo => passivePlace;
        [SerializeField] private PassivePriority passivePlace = PassivePriority.FirstOfAll;
        private SDimensions _sDimension;

        [Tooltip("Ground Mask")]
        public LayerMask groundMask = 1<<0;
        [SerializeField] private bool debug = false;

        [Header("Results")]
        public bool Walkable;
        public float angleFront;
        public float angle;
        public float distance;
        [HideInInspector] public Vector3 normal;
        [HideInInspector] public Vector3 point;
        [HideInInspector] public Rigidbody rbFound;
        private RaycastHit hit;
        
        protected override void AwakeComponent(AbstractCObject cObject)        {
            _sDimension = cObject.FindModule<StatModule>().RequestComponent<SDimensions>(true);
        }
        public override void DoPassive(Vector3 position, InputValues input)
        {
            Vector3 topPosition = position+_sDimension.footOffset+_sDimension.currentHeight*2*-cObject.gravityDirection;
            float maxDistance = _sDimension.currentHeight*3;
            float radious = (_sDimension.currentRadious-0.05f);
            Vector3 hitPoint = position;
            float distanceToSurface = 0;
            Vector3 normalVector = Vector3.zero;
            RaycastHit hit;
            if(debug)
                Debug.DrawRay(topPosition, cObject.gravityDirection*maxDistance, Color.black);
            if(Physics.Raycast(topPosition, cObject.gravityDirection, out hit, maxDistance, groundMask))
            {
                normalVector = hit.normal;
                hitPoint = (hit.point);
                distanceToSurface = hit.distance; 
            }
            else
            {
                Walkable = false;
                distance = maxDistance;
                return;
            }
            rbFound = hit.rigidbody;
            if(debug)
                Debug.DrawRay(topPosition, cObject.gravityDirection, Color.blue);
            if(Physics.SphereCast(topPosition, radious, cObject.gravityDirection, out hit, maxDistance, groundMask))
            {
                if(Vector3.Angle(hit.normal, cObject.gravityDirection) < _sDimension.maxWalkableAngle)
                {
                    normalVector = (normalVector +hit.normal)/2;
                    Vector3 direction = topPosition-hit.point;
                    float dot = Vector3.Dot(direction, -cObject.gravityDirection);
                    distanceToSurface = (distanceToSurface+dot)/2; 
                    hitPoint = (hitPoint+(hit.point))/2;
                    if(debug)
                        Debug.DrawRay(hit.point, hit.normal, Color.blue);
                }
            }
            
            distanceToSurface = (distanceToSurface-_sDimension.currentHeight*2)/_sDimension.currentHeight;
            
            normal = normalVector.normalized;
            point = hitPoint;
            distance = distanceToSurface; 
            angleFront = Vector3.Angle(normal, transform.forward) - 90;
            angle = Vector3.Angle(normal, -cObject.gravityDirection);  
            if(debug)
                Debug.DrawRay(topPosition, normal, Color.red);
            
            Walkable = distance <= 0.1f && angle < _sDimension.maxWalkableAngle;
            if(Walkable)
            {
                if(debug)
                    Debug.DrawRay(hitPoint, normal*2, Color.blue);
            }
            else
            {
                angleFront = 0;
                angle = 0;
            }
            
        }
/*         public Vector3 groundNormalizedForward(InputValues _input)
        {
        return Vector3.ProjectOnPlane(_input.normalDirectionRaw, normal).normalized;
        } */
    }
}