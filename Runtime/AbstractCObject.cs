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
        /// <summary>
        /// Returns a component on the gameObject if exists. If required is True, if the component doesn't exist it will add it.
        /// <summary/>
        public T RequestComponent<T>(bool required) where T : Component
        {
            T requested = GetComponent<T>();
            if(requested == null && required)        
                requested = this.gameObject.AddComponent<T>();
            return requested;
        }
        /// <summary>
        /// Returns a module of the current controller if it has been added, otherwise returns null
        /// <summary/>
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
        /// <summary>
        /// Returns a module of the current controller if it has been added, otherwise returns null
        /// <summary/>
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
        /// <summary>
        /// Returns all the modules added
        /// <summary/>
        public List<AbstractModule> RequestModules()
        {
            return modules;
        }
        #endregion

        private void InitializeObject(bool forcedRestart)
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
        internal void LoadModuleRoutines()
        {
            addModules();
            foreach(AbstractModule module in modules)
            {
                module.LoadRoutines();
            }
            InitializeObject(true);
        }   
        /// <summary>
        /// If the module hasn't been added already, it hads it to the existing list of modules
        /// <summary/>
        protected void AddModule(AbstractModule module)
        {
            if(!modules.Contains(module))
                modules.Add(module);
        }
        /// <summary>
        /// ReInitializes all the routines to regrab dependancies, cleans errors of existing routines on the lists, and removes missing serialized references.
        /// <summary/>
        public void ReInitializeRoutines()
        {
            foreach(AbstractModule module in modules)
            {
                module.InitializeRoutines(this);
            }
        }

        #region Abstract
        /// <summary>
        /// Fill with AddModule() of all the modules that will be used on the controller, so that they can be found by routines
        /// <summar/>
        protected abstract void addModules();
        #endregion

    }
}

