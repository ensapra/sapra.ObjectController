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

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            gravityDirection = Physics.gravity.normalized;
            gravityMultiplier = Physics.gravity.magnitude;
            inputHolder = GetComponent<InputValueHolder>();
            onlyEnabled = true;
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
        protected override void addModules()
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

