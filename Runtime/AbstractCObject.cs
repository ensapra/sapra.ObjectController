using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace sapra.ObjectController
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AbstractCObject : MonoBehaviour
    {                        
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] [SerializeField] protected bool onlyEnabled = true;

        [HideInInspector] public Vector3 gravityDirection;
        [HideInInspector] public float gravityMultiplier;
        
        private List<AbstractModule> modules = new List<AbstractModule>();
        
        void Awake() {
            rb = GetComponent<Rigidbody>();
            gravityDirection = Physics.gravity.normalized;
            gravityMultiplier = Physics.gravity.magnitude;
            onlyEnabled = true;
            InitializeObject(true);
        }

        #region Object Requests
        public T RequestComponent<T>(bool required) where T : Component
        {
            T requested = GetComponent<T>();
            if(requested == null && required)        
                requested = this.gameObject.AddComponent<T>();
            return requested;
        }

        public T RequestModule<T>() where T : AbstractModule
        {
            foreach(AbstractModule moduleFound in modules)
            {
                if(moduleFound.GetType().IsEquivalentTo(typeof(T)))
                {
                    return moduleFound as T;
                }
            }
            Debug.LogError("Missing " + typeof(T).ToString());
            return null;
        }
        public AbstractModule RequestModule(System.Type moduleType)
        {
            foreach(AbstractModule moduleFound in modules)
            {
                if(moduleFound.GetType().IsEquivalentTo(moduleType))
                {
                    return moduleFound;
                }
            }
            Debug.LogError("Missing " + moduleType.GetType().ToString());
            return null;
        }
        #endregion

        internal void InitializeObject(bool forcedRestart)
        {
            addModules();
            if(forcedRestart)
            {
                foreach(AbstractModule module in modules)
                {
                    module.SleepRoutines(this);
                }
            }
            ReInitializeRoutines();
        }
        internal void SwitchTo(bool showEnabled)
        {
            onlyEnabled = showEnabled;
            foreach(AbstractModule module in modules)
                module.onlyEnabled = showEnabled;
        } 
        internal void LoadModuleRoutines()
        {
            addModules();
            foreach(AbstractModule module in modules)
            {
                module.LoadRoutines();
            }
        }   
        protected void AddModule(AbstractModule module)
        {
            if(!modules.Contains(module))
                modules.Add(module);
        }
        public void ReInitializeRoutines()
        {
            foreach(AbstractModule module in modules)
            {
                module.InitializeRoutines(this);
            }
        }

        #region Abstract
        protected abstract void addModules();
        #endregion

    }
}

