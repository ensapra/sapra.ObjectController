using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractRoutine
    {
        [SerializeField] [HideInInspector] private bool wantsAwake = false;
        [SerializeField] [HideInInspector] private bool isAwake;
        protected Transform transform;
        protected Rigidbody rb;
        internal bool wantsAwakened{get{return wantsAwake;}}
        internal bool awakened{get{return isAwake;}}
        protected AbstractCObject controller;
        
        internal void Awake(AbstractCObject controller)
        {
            this.controller = controller;
            wantsAwake = true;
            isAwake = true;
            transform = controller.transform;
            rb = controller.rb;
            AwakeRoutine(controller);
        }
        internal void Sleep(AbstractCObject controller)
        {
            SleepRoutine(controller);
            isAwake = false;
        }
        //protected virtual void AwakeObject(AbstractCObject controller){}
        protected virtual void AwakeRoutine(AbstractCObject controller){}
        protected virtual void SleepRoutine(AbstractCObject controller){}
    }
}
