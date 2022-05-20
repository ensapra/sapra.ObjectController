using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace sapra.ObjectController
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AbstractCObject : MonoBehaviour
    {        
        protected List<AbstractModule<AbstractRoutine>> modules = new List<AbstractModule<AbstractRoutine>>();
        [Tooltip("Enables Continuous check of new components")]
        public bool continuosCheck = true;
        public static float TimeScale = 1f;   
        [HideInInspector] public Rigidbody rb;
        [HideInInspector]
        [SerializeField] protected bool onlyEnabled = true;
        void Start() {
            onlyEnabled = true;
            InitializeObject(true);
        }
        public T RequestComponent<T>(bool required) where T : Component
        {
            T requested = GetComponent<T>();
            if(requested == null && required)        
                requested = this.gameObject.AddComponent<T>();
            return requested;
        }
        public T FindModule<T>(Type module) where T : AbstractModule<AbstractRoutine>
        {
            foreach(T moduleFound in modules)
            {
                if(moduleFound.GetType().IsEquivalentTo(module))
                {
                    return moduleFound;
                }
            }
            return null;
        }
        public void SwitchTo(bool showEnabled)
        {
            onlyEnabled = showEnabled;
            foreach(AbstractModule<AbstractRoutine> module in modules)
                module.onlyEnabled = showEnabled;
        }
        protected abstract void addModules();
        public void GetAllComponents()
        {
            addModules();
            foreach(AbstractModule module in modules)
            {
                module.GetAllComponents();
            }
        }    
        public void InitializeObject(bool forcedRestart)
        {
            addModules();
            if(forcedRestart)
            {
                foreach(AbstractModule<AbstractRoutine> module in modules)
                {
                    module.SleepComponents(this);
                }
            }
            foreach(AbstractModule<AbstractRoutine> module in modules)
            {
                module.InitializeComponents(this);
            }
        }

    }
}

