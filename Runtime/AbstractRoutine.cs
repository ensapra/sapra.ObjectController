using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractRoutine
    {
        protected Transform transform;
        protected Rigidbody rb;

        [SerializeField] [HideInInspector] private bool wantsAwake = false;
        [SerializeField] [HideInInspector] private bool isAwake;
        internal bool wantsAwakened{get{return wantsAwake;}}
        internal bool awakened{get{return isAwake;}}

        [SerializeField] [HideInInspector] protected AbstractCObject controller;
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
        /// <summary>
        /// Automatically called once the Routine is initialized. Equivalent of Awake on MonoBehaviour
        /// <summary/>
        protected virtual void AwakeRoutine(AbstractCObject controller){}

        /// <summary>
        /// Automatically called once the Routine is disabled.
        /// <summary/>
        protected virtual void SleepRoutine(AbstractCObject controller){}
    }
}
