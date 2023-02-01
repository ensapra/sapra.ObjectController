using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractRoutine
    {
        public string name => this.GetType().ToString();
        protected Transform transform;

        [SerializeField] [HideInInspector] private bool _isEnabled;

        internal bool isEnabled{get{return _isEnabled;}}

        [SerializeField] [HideInInspector] protected AbstractCObject controller;
        internal void AwakeRoutine(AbstractCObject controller)
        {
            this.controller = controller;
            this.transform = controller.transform;
            Awake();
            Enable();
        }
        internal void Enable(){
            _isEnabled = true;
        }
        internal void Disable(){
            _isEnabled = false;
        }

        internal void SleepRoutine()
        {
            Sleep();
            Disable();
        }

        public T GetComponent<T>(bool required = false) where T : Component
        {
            return controller.GetComponent<T>(required);
        }

        public T GetModule<T>() where T : AbstractModule
        {
            return controller.GetModule<T>();
        }
        
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return controller.StartCoroutine(routine);
        }
        public void StopCoroutine(Coroutine routine)
        {
            controller.StopCoroutine(routine);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            controller.StopCoroutine(routine);
        }

        /// <summary>
        /// Automatically called once the Routine is initialized. Equivalent of Awake on MonoBehaviour
        /// <summary/>
        protected virtual void Awake(){}

        /// <summary>
        /// Automatically called once the Routine is disabled.
        /// <summary/>
        protected virtual void Sleep(){}
    }
}
