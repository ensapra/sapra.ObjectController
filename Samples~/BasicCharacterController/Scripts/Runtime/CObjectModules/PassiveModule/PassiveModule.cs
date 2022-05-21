using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace sapra.ObjectController
{
    [System.Serializable]
    public class PassiveModule : AbstractModule<AbstractPassive>
    {
        public void Run(PassivePriority wichOnes, InputValues _input, bool continuosCheck)
        {   
            if(continuosCheck)
                InitializeComponents(this.controller);
            for(int i = 0; i < onlyEnabledComponents.Count; i++)
            {
                AbstractPassive passive = onlyEnabledComponents[i];
                if(passive.whenDo == wichOnes)
                {
                    passive.DoPassive(controller.transform.position, _input);
                    passive.DoAnimationParameters();
                }
            }
        }
        public void RunLate(InputValues _input)
        {   
            for(int i = 0; i < onlyEnabledComponents.Count; i++)
            {
                AbstractPassive passive = onlyEnabledComponents[i];
                passive.DoPassiveLate(controller.transform.position, _input);
            }
        }
        
    }
}
