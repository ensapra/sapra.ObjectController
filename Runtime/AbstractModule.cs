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
        
        [SerializeReference] [HideInInspector] public List<T> allRoutines = new List<T>();
        public List<T> onlyEnabledRoutines = new List<T>();
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
        public override void SleepRoutines(AbstractCObject controller)
        {
            for(int i = allRoutines.Count-1; i>= 0; i--)
            {
                T routine = allRoutines[i];
                routine.Sleep(controller);                
            }
        }
        public override void InitializeRoutines(AbstractCObject controller)
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
        }
        public override void GetAllRoutines()
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
        public override AbstractRoutine RequestRoutine(System.Type routineType, bool required)
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
        #endregion
    }
    
    [System.Serializable]
    public abstract class AbstractModule
    {
        public abstract AbstractRoutine RequestRoutine(System.Type routineType, bool required);
        public abstract void InitializeRoutines(AbstractCObject controller);
        public abstract void SleepRoutines(AbstractCObject controller);
        public abstract void GetAllRoutines();
        public bool onlyEnabled = true;
    }
}
