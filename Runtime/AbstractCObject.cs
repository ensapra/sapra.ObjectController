using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace sapra.ObjectController
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AbstractCObject : MonoBehaviour
    {        
        protected List<AbstractModule> modules = new List<AbstractModule>();
        [Tooltip("Enables Continuous check of new components")]
        public bool continuosCheck = true;
        public static float TimeScale = 1f;   
        [HideInInspector] public Rigidbody rb;
        [HideInInspector]
        [SerializeField] protected bool onlyEnabled = true;
        [HideInInspector] public Vector3 gravityDirection;
        [HideInInspector] public float gravityMultiplier;
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
        public T FindModule<T>() where T : AbstractModule
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
        public void SwitchTo(bool showEnabled)
        {
            onlyEnabled = showEnabled;
            foreach(AbstractModule module in modules)
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

    }
}

