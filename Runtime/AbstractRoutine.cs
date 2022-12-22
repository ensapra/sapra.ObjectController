using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractRoutine
    {
        protected Rigidbody rb;
        protected Transform transform;

        [SerializeField] [HideInInspector] private bool _isEnabled;

        internal bool isEnabled{get{return _isEnabled;}}

        [SerializeField] [HideInInspector] protected AbstractCObject controller;
        internal void Awake(AbstractCObject controller)
        {
            this.controller = controller;
            this.transform = controller.transform;
            this.rb = controller.rb;

            AwakeRoutine(controller);
            Enable();
        }
        internal void Enable(){
            _isEnabled = true;
        }
        internal void Disable(){
            _isEnabled = false;
        }

        internal void Sleep(AbstractCObject controller)
        {
            SleepRoutine(controller);
            Disable();
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
