using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace sapra.ObjectController
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AbstractCObject : MonoBehaviour
    {                
        [Tooltip("Enables Continuous check of new components")]
        public bool continuosCheck = true;
        
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
            return null;
        }
        #endregion

        public void InitializeObject(bool forcedRestart)
        {
            addModules();
            if(forcedRestart)
            {
                foreach(AbstractModule module in modules)
                {
                    module.SleepComponents(this);
                }
            }
            foreach(AbstractModule module in modules)
            {
                module.InitializeComponents(this);
            }
        }
        public void SwitchTo(bool showEnabled)
        {
            onlyEnabled = showEnabled;
            foreach(AbstractModule module in modules)
                module.onlyEnabled = showEnabled;
        } 
        public void GetAllComponents()
        {
            addModules();
            foreach(AbstractModule module in modules)
            {
                module.GetAllComponents();
            }
        }   
        protected void AddModule(AbstractModule module)
        {
            if(!modules.Contains(module))
                modules.Add(module);
        }

        #region Abstract
        protected abstract void addModules();

        #endregion

    }
}

