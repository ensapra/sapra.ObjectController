using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractModule<T> : AbstractModule where T : AbstractRoutine
    {
        [SerializeReference] [HideInInspector] protected AbstractCObject controller;
        
        [SerializeReference] [HideInInspector] internal List<T> allRoutines = new List<T>();
        /// <summary>
        /// Enabled components
        /// <summary/>
        protected List<T> onlyEnabledRoutines = new List<T>();
            
        public override object[] EnabledRoutinesObject => onlyEnabledRoutines.Cast<object>().ToArray();

        private List<T> GetComponentsInAssembly(Assembly assem)
        {
            IEnumerable<Type> q = from t in assem.GetTypes()
                    where t.IsSubclassOf(typeof(T))
                    select t;
            List<T> temp = new List<T>();
            foreach (Type item in q)
            {
                T ObjectFound = allRoutines.Find(x => x != null && x.GetType() == item);
                if(ObjectFound == null)
                {
                    try{
                        T generated = Activator.CreateInstance(item) as T;
                        temp.Add(generated); 
                    }
                    catch{
                        Debug.Log("Error creating "+item);
                        continue;
                    }
                }
                else
                    temp.Add(ObjectFound);
            }
            return temp;
        }
        #region  Initialization
        internal override sealed void SleepRoutines(AbstractCObject controller)
        {
            for(int i = allRoutines.Count-1; i>= 0; i--)
            {
                T routine = allRoutines[i];
                routine.Sleep(controller);                
            }
        }
        internal override sealed void InitializeRoutines(AbstractCObject controller)
        {
            this.controller = controller;
            if(this.controller == null)
            {
                Debug.Log("Error initializing components, no CObject was set");
                return;
            }
            onlyEnabledRoutines.Clear();
            for(int i = allRoutines.Count-1; i>= 0; i--)
            {
                T routine = allRoutines[i];
                if(routine.wantsAwakened && !routine.awakened)      
                    routine.Awake(controller);
                if(!routine.wantsAwakened && routine.awakened)  
                    routine.Sleep(controller);    

                if(routine.wantsAwakened && routine.awakened)
                    onlyEnabledRoutines.Add(routine);          
            }    
            InitializeModule();
        }
        internal override sealed void LoadRoutines()
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            List<T> newList = new List<T>();
            foreach(Assembly assembly in assemblies)
            {
                newList.AddRange(GetComponentsInAssembly(assembly));
            }
            allRoutines = newList;
        }
        #endregion
        #region Components requests
        /// <summary>
        /// Returns list of enabled routines
        /// <summary/>
        public List<T> RequestEnabledRoutines()
        {
            return onlyEnabledRoutines;
        }
        /// <summary>
        /// Returns the requested Routine if it has been enabled, otherwise returns true. If required is True, and the component hasn't been enabled, it will automatically enable it
        /// <summary/>
        public Z RequestRoutine<Z>(bool required) where Z : T
        {
            foreach (T routine in allRoutines)
            {
                if(routine.GetType().IsEquivalentTo(typeof(Z)))
                {
                    if(required)
                    {
                        if(!routine.awakened)
                            routine.Awake(controller);
                        return routine as Z;
                    }
                    else if(routine.wantsAwakened)
                    {
                        if(!routine.awakened)
                            routine.Awake(controller);
                        return routine as Z;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the requested Routine if it has been enabled, otherwise returns true. If required is True, and the component hasn't been enabled, it will automatically enable it
        /// <summary/>
        public override sealed AbstractRoutine RequestRoutine(System.Type routineType, bool required)
        {
            foreach (AbstractRoutine routine in allRoutines)
            {
                if(routine.GetType().IsEquivalentTo(routineType))
                {
                    if(required)
                    {
                        if(!routine.awakened)
                            routine.Awake(controller);
                        return routine;
                    }
                    else if(routine.wantsAwakened)
                    {
                        if(!routine.awakened)
                            routine.Awake(controller);
                        return routine;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the type of the current module
        /// <summary/>
        public override Type GetModuleType()
        {
            return typeof(T);
        }
        #endregion
    }
    
    [System.Serializable]
    public abstract class AbstractModule
    {
        public abstract AbstractRoutine RequestRoutine(System.Type routineType, bool required);
        public abstract object[] EnabledRoutinesObject{get;}
        internal abstract void InitializeRoutines(AbstractCObject controller);
        internal abstract void SleepRoutines(AbstractCObject controller);
        internal abstract void LoadRoutines();
        public abstract Type GetModuleType();
        /// <summary>
        /// Method called after all routines have been enabled. Equivalent to Awake of Monobehaviours
        /// <summary/>
        protected virtual void InitializeModule(){}
    }
}
