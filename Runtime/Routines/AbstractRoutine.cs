using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractRoutine<T> where T : AbstractCObject
    {
        protected T cObject;
        [SerializeField] [HideInInspector] private bool wantsAwake = false;
        [SerializeField] [HideInInspector] private bool isAwake;

        protected Transform transform;
        protected Rigidbody rb;

        public void Awake(T abstractCObject)
        {
            this.cObject = cObject;
            wantsAwake = true;
            isAwake = true;
            transform = cObject.transform;
            rb = cObject.rb;
            AwakeObject(cObject);
            AwakeComponent(cObject);
        }
        public void Sleep(T cObject)
        {
            SleepComponent(cObject);
            isAwake = false;
        }
        protected virtual void AwakeObject(T cObject){}
        protected abstract void AwakeComponent(T cObject);
        protected virtual void SleepComponent(T cObject){}
        public bool wantsAwakened{get{return wantsAwake;}}
        public bool awakened{get{return isAwake;}}
    }
}
