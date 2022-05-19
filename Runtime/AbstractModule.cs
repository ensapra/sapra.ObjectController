using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractModule<T, Z> : AbstractModule<Z> where T : AbstractRoutine<Z> where Z : AbstractCObject
    {
        public Z cObject;
        [SerializeReference]
        public List<T> allComponents = new List<T>();
        public List<T> onlyEnabledComponents = new List<T>();
        public AbstractRoutine<Z> FindComponent(Type component)
        {
            foreach(AbstractRoutine<Z> abstractRoutine in onlyEnabledComponents)
            {
                if(abstractRoutine.GetType().IsEquivalentTo(component))
                {
                    return abstractRoutine;
                }
            }
            return null;
        }
        public override void GetAllComponents()
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            allComponents.Clear();
            foreach(Assembly assembly in assemblies)
            {
                GetComponentsInAssembly(assembly);
            }
        }
        private void GetComponentsInAssembly(Assembly assem)
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
            allComponents.AddRange(temp);
        }
        public override void SleepComponents(Z cObject)
        {
            for(int i = allComponents.Count-1; i>= 0; i--)
            {
                T component = allComponents[i];
                component.Sleep(cObject);                
            }
        }
        public override void InitializeComponents(Z cObject)
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
    [System.Serializable]
    public abstract class AbstractModule<T> where T : AbstractCObject
    {
        public abstract void InitializeComponents(T cObject);
        public abstract void SleepComponents(T cObject);
        public abstract void GetAllComponents();
        public bool onlyEnabled = true;
    }
}
