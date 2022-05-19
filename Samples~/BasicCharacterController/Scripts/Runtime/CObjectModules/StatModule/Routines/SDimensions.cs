using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class SDimensions : AbstractStat
    {
        [Header("Player reachings")]
        [Tooltip("Character height from feet to head")]
        public float characterHeight = 2;
        [Tooltip("Normalized amount of body height to shoulder level")]
        [Range(0.1f,0.99f)] public float shoulderLevel = 0.75f;
        public float halfHeight{get{return characterHeight/2f;}}
        [Tooltip("Character basic radious of collider")]
        public float characterRadious = 0.5f;
        [Tooltip("Character maximum walkable angle")]
        public float maxWalkableAngle = 40;
        [Tooltip("Character minimum walkable angle")]
        public float startDecreaseVelAngle = 10;
        [Tooltip("Offset from object center to the foot, [DEBUG] Black line, should be on the foot")]
        public Vector3 footOffsetLocal;
        [Tooltip("Offset from object center to the centre of mass, [DEBUG] Purple Line, should be on the center")]
        public Vector3 forcesCenterLocalOffset;
        public Vector3 footOffset {get{ return this.transform.TransformVector(footOffsetLocal);}}
        public Vector3 forcesCenterOffset{get{return this.transform.TransformVector(forcesCenterLocalOffset);}}
        public float currentRadious{
            get{
                if(tempRadious != -1)
                    return tempRadious;
                else
                    return characterRadious;
            }
            set{
                tempRadious = value;
            }
        
        }
        public float currentHeight{
            get{
                if(tempHeight != -1)
                    return tempHeight;
                else
                    return halfHeight;
            }
            set{
                tempHeight = value;
            }
        }
        private float tempHeight = -1;
        private float tempRadious = -1;
        public bool debug;
        public override void DoExtra()
        {
            if(debug)
            {
                Debug.DrawRay(transform.position + footOffset, transform.forward, Color.black);
                Debug.DrawRay(transform.position + forcesCenterOffset, transform.forward, Color.magenta);
            }
        }

        protected override void AwakeComponent(CObject cObject)
        {
        }
    }
}