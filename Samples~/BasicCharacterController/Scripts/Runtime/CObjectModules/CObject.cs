using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class CObject : AbstractCObject 
    {
        [Tooltip("Enables Continuous check of new components")]
        [SerializeField] private bool continuosCheck = true;
        public ActiveModule activeModule = new ActiveModule();
        public PassiveModule passiveModule = new PassiveModule();
        public StatModule statModule = new StatModule();

        private InputValueHolder inputHolder;
        private InputValues _input{get{
        if(inputHolder)
            return inputHolder.input;
        else
            return null;}}

        void Start()
        {
            inputHolder = GetComponent<InputValueHolder>();
        }
        void FixedUpdate()
        {      
            if(continuosCheck)
                ReInitializeRoutines();

            statModule.Run();
            passiveModule.Run(PassivePriority.FirstOfAll, _input);
            passiveModule.Run(PassivePriority.BeforeActive, _input);
            activeModule.Run(_input);
            passiveModule.Run(PassivePriority.AfterActive, _input);
            passiveModule.Run(PassivePriority.LastOne, _input);
        }
        private void LateUpdate() {
            passiveModule.RunLate(_input);   
        }
        protected override void addModules()
        {
            AddModule(statModule);
            AddModule(passiveModule);
            AddModule(activeModule);
        }
    }
}

