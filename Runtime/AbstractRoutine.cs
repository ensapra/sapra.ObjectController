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
        protected AbstractCObject controller;
        
        public void Awake(AbstractCObject controller)
        {
            this.controller = controller;
            wantsAwake = true;
            isAwake = true;
            transform = controller.transform;
            rb = controller.rb;
            //AwakeObject(controller);
            AwakeComponent(controller);
        }
        public void Sleep(AbstractCObject controller)
        {
            SleepComponent(controller);
            isAwake = false;
        }
        //protected virtual void AwakeObject(AbstractCObject controller){}
        protected virtual void AwakeComponent(AbstractCObject controller){}
        protected virtual void SleepComponent(AbstractCObject controller){}
    }
}
