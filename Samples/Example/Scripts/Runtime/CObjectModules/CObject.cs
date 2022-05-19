using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class CObject : AbstractCObject 
    {
        private List<AbstractModule<CObject>> modules = new List<AbstractModule<CObject>>();
        public ActiveModule activeModule = new ActiveModule();
        public PassiveModule passiveModule = new PassiveModule();
        public StatModule statModule = new StatModule();

        private InputValueHolder inputHolder;
        private InputValues _input{get{
        if(inputHolder)
            return inputHolder.input;
        else
            return null;}}
        [HideInInspector]
        [SerializeField] private bool onlyEnabled = true;
        [HideInInspector] public Vector3 gravityDirection;
        [HideInInspector] public float gravityMultiplier;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            gravityDirection = Physics.gravity.normalized;
            gravityMultiplier = Physics.gravity.magnitude;
            inputHolder = GetComponent<InputValueHolder>();
            onlyEnabled = true;
        }
        public override void InitializeObject(bool forcedRestart)
        {
            addModules();
            if(forcedRestart)
            {
                foreach(AbstractModule<CObject> module in modules)
                {
                    module.SleepComponents(this);
                }
            }
            foreach(AbstractModule<CObject> module in modules)
            {
                module.InitializeComponents(this);
            }
        }

        public void SwitchTo(bool showEnabled)
        {
            onlyEnabled = showEnabled;
            foreach(AbstractModule<CObject> module in modules)
                module.onlyEnabled = showEnabled;
        }
        public AbstractModule<CObject> FindModule(Type module)
        {
            foreach(AbstractModule<CObject>  moduleFound in modules)
            {
                if(moduleFound.GetType().IsEquivalentTo(module))
                {
                    return moduleFound;
                }
            }
            return null;
        }
        void FixedUpdate()
        {      
            Time.timeScale = TimeScale;
            statModule.Run(continuosCheck);
            passiveModule.Run(PassivePriority.FirstOfAll, _input, continuosCheck);
            passiveModule.Run(PassivePriority.BeforeActive, _input, continuosCheck);
            activeModule.Run(_input, continuosCheck);
            passiveModule.Run(PassivePriority.AfterActive, _input, continuosCheck);
            passiveModule.Run(PassivePriority.LastOne, _input, continuosCheck);
        }
        private void LateUpdate() {
            passiveModule.RunLate(_input);   
        }
        private void OnAnimatorIK(int layerIndex) {
        }
        public override void GetAllComponents()
        {
            addModules();
            foreach(AbstractModule<CObject> module in modules)
            {
                module.GetAllComponents();
            }
        }
        private void addModules()
        {
            if(!modules.Contains(statModule))
                modules.Add(statModule);
            if(!modules.Contains(passiveModule))
                modules.Add(passiveModule);
            if(!modules.Contains(activeModule))
                modules.Add(activeModule);
        }
    }
}

