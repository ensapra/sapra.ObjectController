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
        
        /// <summary>
        /// Enabled components
        /// <summary/>

        [SerializeReference] [HideInInspector] protected List<T> onlyEnabledRoutines = new List<T>();
        [SerializeReference] [HideInInspector] private List<T> cachedRoutines = new List<T>();

        public override AbstractRoutine[] EnabledRoutinesObject => onlyEnabledRoutines.ToArray();
        [SerializeField] [HideInInspector] private bool RemoveUnused = false;

        #region  Initialization
        internal override sealed void SleepRoutines(AbstractCObject controller)
        {
            for(int i = onlyEnabledRoutines.Count-1; i>= 0; i--)
            {
                T routine = onlyEnabledRoutines[i];
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

            //Check cached routines to recover info
            for(int i = cachedRoutines.Count-1; i>= 0; i--)
            {
                T routine = cachedRoutines[i];
                if(routine.isEnabled)
                {
                    onlyEnabledRoutines.Add(routine);       
                    cachedRoutines.RemoveAt(i);     
                }  
            }  

            //Remove all the rest disabled routines if needed
            if(RemoveUnused)
                cachedRoutines.Clear();
            
            for(int i = onlyEnabledRoutines.Count-1; i>= 0; i--)
            {
                T routine = onlyEnabledRoutines[i];
                if(routine.isEnabled)      
                    routine.Awake(controller);
                if(!routine.isEnabled)  
                    routine.Sleep(controller);    
 
                if(!routine.isEnabled)
                {
                    //Store to cache if needed later
                    if(!RemoveUnused)
                        cachedRoutines.Add(routine);
                    onlyEnabledRoutines.RemoveAt(i);       
                }  
            }    

            onlyEnabledRoutines.Sort((a,b)=> a.GetType().ToString().CompareTo(b.GetType().ToString()));
            InitializeModule();
        }

        #endregion
        #region Components requests
        private object GenerateRoutine(Type type)
        {
            T newRoutine = Activator.CreateInstance(type) as T;
            newRoutine.Enable();
            onlyEnabledRoutines.Add(newRoutine);
            onlyEnabledRoutines.Sort((a,b)=> a.GetType().ToString().CompareTo(b.GetType().ToString()));
            return newRoutine;
        }

        internal override AbstractRoutine RequestRoutine(System.Type type, bool required){
            T foundRoutine = onlyEnabledRoutines.Find(x => x != null && x.GetType().IsEquivalentTo(type));
            if(foundRoutine != null)
                return foundRoutine;
            if(!required)
                return null;
            
            foundRoutine = cachedRoutines.Find(x => x != null && x.GetType().IsEquivalentTo(type));
            if(foundRoutine != null)
            {
                foundRoutine.Enable();
                cachedRoutines.Remove(foundRoutine);
                onlyEnabledRoutines.Add(foundRoutine);
                onlyEnabledRoutines.Sort((a,b)=> a.GetType().ToString().CompareTo(b.GetType().ToString()));
                return foundRoutine;
            }

            return GenerateRoutine(type) as AbstractRoutine;
        }

        /// <summary>
        /// Returns the requested Routine if it has been enabled, otherwise returns true. If required is True, and the component hasn't been enabled, it will automatically enable it
        /// <summary/>
        public Z RequestRoutine<Z>(bool required = false) where Z : T{
            return RequestRoutine(typeof(Z), required) as Z;
        }
        internal override List<Type> GetAssemblyRoutines()
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            List<Type> newList = new List<Type>();
            foreach(Assembly assembly in assemblies)
            {
                IEnumerable<Type> q = from t in assembly.GetTypes()
                    where t.IsSubclassOf(typeof(T))
                    select t;
                newList.AddRange(q);
            }

            return newList;
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
        //public abstract AbstractRoutine RequestRoutine(System.Type routineType, bool required);
        public abstract AbstractRoutine[] EnabledRoutinesObject{get;}
        internal abstract void InitializeRoutines(AbstractCObject controller);
        internal abstract void SleepRoutines(AbstractCObject controller);
        internal abstract List<Type> GetAssemblyRoutines();
        internal abstract AbstractRoutine RequestRoutine(System.Type type, bool required);
        //internal abstract void LoadRoutines();
        public abstract Type GetModuleType();
        /// <summary>
        /// Method called after all routines have been enabled. Equivalent to Awake of Monobehaviours
        /// <summary/>
        protected virtual void InitializeModule(){}

        
    }
}
