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
        public CObject cObject;
        [SerializeReference]
        public List<T> allComponents = new List<T>();
        public List<T> onlyEnabledComponents = new List<T>();
        private List<string> allCompoenntsName = new List<string>();
        public override AbstractRoutine FindComponent(Type component)
        {
            foreach(AbstractRoutine abstractRoutine in onlyEnabledComponents)
            {
                if(abstractRoutine.GetType().IsEquivalentTo(component))
                {
                    return abstractRoutine;
                }
            }
            return null;
        }
        public override void GetAllComponents(string nmspace)
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            allComponents.Clear();
            foreach(Assembly assembly in assemblies)
            {
                GetComponentsInAssembly(assembly, nmspace);
            }
        }
        private void GetComponentsInAssembly(Assembly assem, string nmspace)
        {
            IEnumerable<Type> q = from t in assem.GetTypes()
                    where t.IsSubclassOf(typeof(T)) && t.Namespace == nmspace
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
            allComponents.AddRange(temp);
        }
        public override void SleepComponents(CObject cObject)
        {
            for(int i = allComponents.Count-1; i>= 0; i--)
            {
                T component = allComponents[i];
                component.Sleep(cObject);                
            }
        }
        public override void InitializeComponents(CObject cObject)
        {
            this.cObject = cObject;
            if(this.cObject == null)
            {
                Debug.Log("Error initializing components, no CObject was set");
                return;
            }
            onlyEnabledComponents.Clear();
            for(int i = allComponents.Count-1; i>= 0; i--)
            {
                T component = allComponents[i];
                if(component.wantsAwakened && !component.awakened)      
                    component.Awake(cObject);
                if(!component.wantsAwakened && component.awakened)  
                    component.Sleep(cObject);    

                if(component.wantsAwakened && component.awakened)
                    onlyEnabledComponents.Add(component);          
            }            
        }
        public TComponent RequestComponent<TComponent>(bool required) where TComponent : T
        {
            foreach (T component in allComponents)
            {
                if(component is TComponent)
                {
                    if(required)
                    {
                        if(!component.awakened)
                            component.Awake(cObject);
                        return component as TComponent;
                    }
                    else if(component.wantsAwakened)
                    {
                        if(!component.awakened)
                            component.Awake(cObject);
                        return component as TComponent;
                    }
                }
            }
            return null;
        }
    }
    public abstract class AbstractModule
    {
        public abstract void InitializeComponents(CObject cObject);
        public abstract void SleepComponents(CObject cObject);
        public abstract void GetAllComponents(string nmspace);
        public bool onlyEnabled = true;
        public abstract AbstractRoutine FindComponent(Type component);
    }
}
