using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace sapra.ObjectController
{
    public abstract class AbstractCObject : MonoBehaviour
    {                        
        [HideInInspector] [SerializeField] protected bool onlyEnabled = true;
        
        private List<AbstractModule> modules = new List<AbstractModule>();
        void Awake() {
            onlyEnabled = true;
            InitializeController();
        }

        #region Object Requests
        /// <summary>
        /// Returns a component on the gameObject if exists. If required is True, if the component doesn't exist it will add it.
        /// <summary/>
        public T GetComponent<T>(bool required) where T : Component
        {
            T requested = GetComponent<T>();
            if(requested == null && required)        
                requested = this.gameObject.AddComponent<T>();
            return requested;
        }
        /// <summary>
        /// Returns a module of the current controller if it has been added, otherwise returns null
        /// <summary/>
        public T GetModule<T>() where T : AbstractModule
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
        public AbstractModule GetModule(System.Type moduleType)
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
        public List<AbstractModule> GetModules()
        {
            return modules;
        }
        #endregion

        public void InitializeController()
        {
            addModules();
            foreach(AbstractModule module in modules)
            {
                module.InitializeRoutines(this);
            }
        }
        /// <summary>
        /// If the module hasn't been added already, it hads it to the existing list of modules
        /// <summary/>
        protected void AddModule(AbstractModule module)
        {
            if(!modules.Contains(module))
                modules.Add(module);
        }

        #region Abstract
        /// <summary>
        /// Fill with AddModule() of all the modules that will be used on the controller, so that they can be found by routines
        /// <summar/>
        protected abstract void addModules();
        #endregion

    }
}

