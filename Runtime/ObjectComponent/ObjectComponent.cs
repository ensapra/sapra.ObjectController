using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class ObjectComponent
    {
        protected CObject cObject;
        [SerializeField] [HideInInspector] private bool wantsAwake = false;
        [SerializeField] [HideInInspector] private bool isAwake;

        protected Transform transform;
        protected Rigidbody rb;

        public void Awake(CObject cObject)
        {
            this.cObject = cObject;
            wantsAwake = true;
            isAwake = true;
            transform = cObject.transform;
            rb = cObject.rb;
            //RegisterComponent(cObject);
            AwakeObject(cObject);
            AwakeComponent(cObject);
        }
        public void Sleep(CObject cObject)
        {
            SleepComponent(cObject);
            isAwake = false;
        }
        protected virtual void AwakeObject(CObject cObject){}
        protected abstract void AwakeComponent(CObject cObject);
        protected virtual void SleepComponent(CObject cObject){}
        //protected abstract void RegisterComponent(CObject cObject);
        public bool wantsAwakened{get{return wantsAwake;}}
        public bool awakened{get{return isAwake;}}
    }
}
