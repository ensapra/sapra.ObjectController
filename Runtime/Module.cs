using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using sapra.ObjectController;
namespace sapra.ObjectController
{

    [System.Serializable]
    public abstract class Module<T> : Module where T : Routine
    {       
        /// <summary>
        /// Enabled components
        /// <summary/>
        public List<T> EnabledRoutines = new List<T>();
        
        #region  Initialization
        public Z GetComponent<Z>(bool required = false) where Z : Component
        {
            return controller.GetComponent<Z>(required);
        }
        internal override void EnableRoutines()
        {
            EnabledRoutines.Clear();
            EnabledRoutines = baseAllRoutines.FindAll(a => a._isEnabled).Cast<T>().ToList();
        }
        #endregion
        #region Components requests
        public override Routine GenerateRoutine(Type type)
        {
            T newRoutine = Activator.CreateInstance(type) as T;
            newRoutine._isEnabled = true;
            AddRoutine(newRoutine);
            baseAllRoutines.Sort((a,b)=> a.GetType().ToString().CompareTo(b.GetType().ToString()));
            return newRoutine;
        }

        /// <summary>
        /// Returns the requested Routine if it has been enabled, otherwise returns true. If required is True, and the component hasn't been enabled, it will automatically enable it
        /// <summary/>
        public Z GetRoutine<Z>(bool required = false) where Z : T{
            return GetRoutine(typeof(Z), required) as Z;
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
    public abstract class Module
    {
        [SerializeReference] [HideInInspector] protected ObjectController controller;
        [SerializeField] [HideInInspector] protected bool RemoveUnused = false;
        [SerializeReference] public List<Routine> baseAllRoutines = new List<Routine>();
        internal abstract void EnableRoutines();
        internal void InitializeRoutines(ObjectController controller){
            this.controller = controller;
            if(this.controller == null)
            {
                Debug.Log("Error initializing components, no CObject was set");
                return;
            }
            UpdateList();
            InitializeModule();
        }
        internal abstract List<Type> GetAssemblyRoutines();
        public abstract Type GetModuleType();
        public abstract Routine GenerateRoutine(Type type);
        public Routine GetRoutine(Type type, bool required){
            Routine foundRoutine = baseAllRoutines.FirstOrDefault(a => type.IsEquivalentTo(a.GetType()));
            if(foundRoutine != default){
                if(!foundRoutine._isEnabled)
                    foundRoutine.AwakeRoutine(controller);
                return foundRoutine;
            }
            else if(required){
                foundRoutine = GenerateRoutine(type);
                foundRoutine.AwakeRoutine(controller);
                return foundRoutine;
            }
            else
                return null;
        
        }

        internal void UpdateList(){
            baseAllRoutines.RemoveAll(a => a == null);
            List<Routine> Copy = new List<Routine>(baseAllRoutines);
            foreach(Routine target in Copy){
                if(!target.isEnabled){
                    target.SleepRoutine();
                    if(RemoveUnused)
                        baseAllRoutines.Remove(target);
                }
                else{
                    target.AwakeRoutine(controller);
                }
                
            }
            
            baseAllRoutines.Sort((a,b)=> a.GetType().ToString().CompareTo(b.GetType().ToString()));
            EnableRoutines();
        } 

        internal void SleepRoutines(ObjectController controller)
        {
            for(int i = baseAllRoutines.Count-1; i>= 0; i--)
            {
                Routine routine = baseAllRoutines[i];
                routine.SleepRoutine();                
            }
        }

        public void AddRoutine(Routine routine){
            if(!baseAllRoutines.Contains(routine))
                baseAllRoutines.Add(routine);
        }

        public void InternalSort(){
            baseAllRoutines.Sort((a,b)=> a.GetType().ToString().CompareTo(b.GetType().ToString()));
        }
        /// <summary>
        /// Method called after all routines have been enabled. Equivalent to Awake of Monobehaviours
        /// <summary/>
        protected virtual void InitializeModule(){}
    }
}

