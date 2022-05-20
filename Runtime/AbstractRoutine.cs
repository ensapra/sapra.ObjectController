using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractRoutine
    {
        [SerializeField] [HideInInspector] protected bool wantsAwake = false;
        [SerializeField] [HideInInspector] protected bool isAwake;
        protected Transform transform;
        protected Rigidbody rb;
        public bool wantsAwakened{get{return wantsAwake;}}
        public bool awakened{get{return isAwake;}}
        protected AbstractCObject cObject;
        public void Awake(AbstractCObject abstractCObject)
        {
            this.cObject = abstractCObject;
            wantsAwake = true;
            isAwake = true;
            transform = cObject.transform;
            rb = cObject.rb;
            AwakeObject(abstractCObject);
            AwakeComponent(abstractCObject);
        }
        public void Sleep(AbstractCObject abstractCObject)
        {
            SleepComponent(abstractCObject);
            isAwake = false;
        }
        protected virtual void AwakeObject(AbstractCObject cObject){}
        protected abstract void AwakeComponent(AbstractCObject cObject);
        protected virtual void SleepComponent(AbstractCObject cObject){}
    }
}
