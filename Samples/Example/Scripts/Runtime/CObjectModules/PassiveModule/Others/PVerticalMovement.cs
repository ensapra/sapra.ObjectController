using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class PVerticalMovement : AbstractPassive
    {
        public override PassivePriority whenDo => PassivePriority.BeforeActive;
        private SDimensions _sDimensions;
        private PWalkableDetection _pWalkableDetection;
        public LayerMask ladderMask;

        public bool canVerticalMove{get{return _canVerticalMove;}}
        [SerializeField]private bool _canVerticalMove;
        public Vector3 upVector;
        public Vector3 normalVector;
        public Vector3 pointOnTheLadder;
        private Vector3 ladderPosition;
        

        public float maxDetectionDistance = 2;
        public bool debug;
        

        public override void DoPassive(Vector3 position, InputValues input)
        {
            position = position+_sDimensions.footOffset+_sDimensions.halfHeight*-cObject.gravityDirection;
            RaycastHit hit;
            if(Physics.Raycast(position, transform.forward,out hit, maxDetectionDistance+_sDimensions.characterRadious, ladderMask))
            {
                _canVerticalMove = true;
                normalVector = hit.transform.forward;
                upVector = hit.transform.up;
                ladderPosition = hit.transform.position;
                pointOnTheLadder = hit.point;
                if(Vector3.Dot(hit.normal, normalVector) < 0)
                {
                    Vector3 dir = hit.point-ladderPosition;
                    dir = Vector3.ProjectOnPlane(dir, upVector);
                    pointOnTheLadder -= dir*2;
                }
                if(debug)
                    Debug.DrawRay(hit.point, upVector, Color.black);
            }
            else
            {
                _canVerticalMove = false;
            }
        }
        public void ClearParameters()
        {
            normalVector = Vector3.zero;
            upVector = Vector3.zero;
            ladderPosition = Vector3.zero;
        }
        public bool AmITop(out Vector3 topPosition)
        {
            Vector3 raycastPosition = transform.position+_sDimensions.footOffset+_sDimensions.characterHeight*upVector;
            raycastPosition += -normalVector*_sDimensions.characterRadious*3;
            RaycastHit topHit;
            if(debug)
                Debug.DrawRay(raycastPosition, -upVector.normalized*_sDimensions.characterHeight, Color.green);
            if(Physics.Raycast(raycastPosition, -upVector, out topHit, _sDimensions.characterHeight, _pWalkableDetection.groundMask))
            {
                if(topHit.distance > _sDimensions.halfHeight)
                {                
                    topPosition = topHit.point-_sDimensions.footOffset;
                    return true;        
                }        
            }
            topPosition = Vector3.zero;
            return false;
        }
        public bool DoIStartTop()
        {
            Vector3 difference = transform.position-ladderPosition;
            if(Vector3.Dot(difference, upVector) > 0)
                return true;
            else
                return false;
        }
        public Vector3 changePosition(Vector3 position)
        {
            if(!_canVerticalMove)
                return position;

            Plane wall = new Plane(normalVector, pointOnTheLadder);
            Vector3 ladderPositionOnWall = wall.ClosestPointOnPlane(ladderPosition);
            Vector3 cross = Vector3.Cross(normalVector, upVector);
            Plane middlePoint = new Plane(cross, ladderPositionOnWall);
            return middlePoint.ClosestPointOnPlane(wall.ClosestPointOnPlane(position))+_sDimensions.characterRadious*normalVector;
        }
        protected override void AwakeComponent(CObject cObject)
        {
            _sDimensions = cObject.statModule.RequestComponent<SDimensions>(true);
            _pWalkableDetection = cObject.passiveModule.RequestComponent<PWalkableDetection>(true);
        }
    }
}