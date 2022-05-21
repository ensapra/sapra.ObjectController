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
        protected AbstractCObject controller;
        
        [SerializeReference] [HideInInspector] public List<T> allComponents = new List<T>();
        public List<T> onlyEnabledComponents = new List<T>();
        private List<T> GetComponentsInAssembly(Assembly assem)
        {
            IEnumerable<Type> q = from t in assem.GetTypes()
                    where t.IsSubclassOf(typeof(T))
                    select t;
            List<T> temp = new List<T>();
            foreach (Type item in q)
            {
                T ObjectFound = allComponents.Find(x => x != null && x.GetType() == item);
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
        public override void SleepComponents(AbstractCObject controller)
        {
            for(int i = allComponents.Count-1; i>= 0; i--)
            {
                T component = allComponents[i];
                component.Sleep(controller);                
            }
        }
        public override void InitializeComponents(AbstractCObject controller)
        {
            this.controller = controller;
            if(this.controller == null)
            {
                Debug.Log("Error initializing components, no CObject was set");
                return;
            }
            onlyEnabledComponents.Clear();
            for(int i = allComponents.Count-1; i>= 0; i--)
            {
                T component = allComponents[i];
                if(component.wantsAwakened && !component.awakened)      
                    component.Awake(controller);
                if(!component.wantsAwakened && component.awakened)  
                    component.Sleep(controller);    

                if(component.wantsAwakened && component.awakened)
                    onlyEnabledComponents.Add(component);          
            }            
        }
        public override void GetAllComponents()
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            List<T> newList = new List<T>();
            foreach(Assembly assembly in assemblies)
            {
                newList.AddRange(GetComponentsInAssembly(assembly));
            }
            allComponents = newList;
        }
        #endregion
        #region Components requests
        public Z RequestRoutine<Z>(bool required) where Z : T
        {
            foreach (T component in allComponents)
            {
                if(component is Z)
                {
                    if(required)
                    {
                        if(!component.awakened)
                            component.Awake(controller);
                        return component as Z;
                    }
                    else if(component.wantsAwakened)
                    {
                        if(!component.awakened)
                            component.Awake(controller);
                        return component as Z;
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
        public abstract void InitializeComponents(AbstractCObject controller);
        public abstract void SleepComponents(AbstractCObject controller);
        public abstract void GetAllComponents();
        public bool onlyEnabled = true;
    }
}
