using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class Routine
    {
        public string name => this.GetType().ToString();
        protected Transform transform;

        [SerializeField] [HideInInspector] internal bool _isEnabled;
        [SerializeField] [HideInInspector] internal bool _isAwake;

        internal bool isEnabled{get{return _isEnabled;}}

        [SerializeField] [HideInInspector] protected ObjectController controller;
        internal void AwakeRoutine(ObjectController controller)
        {
            this.controller = controller;
            this.transform = controller.transform;
            this._isAwake = true;
            Awake();
            Enable();
        }

        internal void SleepRoutine()
        {
            this._isAwake = false;
            Disable();
        }

        private void Enable(){
            _isEnabled = true;
        }
        private void Disable(){
            _isEnabled = false;
        }


        public T GetComponent<T>(bool required = false) where T : Component
        {
            return controller.GetComponent<T>(required);
        }

        public T GetModule<T>() where T : Module
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
    }
}
