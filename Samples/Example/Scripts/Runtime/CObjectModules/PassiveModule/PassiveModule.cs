using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class PassiveModule : AbstractModule<AbstractPassive, CObject>
    {
        public void Run(PassivePriority wichOnes, InputValues _input, bool continuosCheck)
        {   
            if(continuosCheck)
                InitializeComponents(this.cObject);
            for(int i = 0; i < onlyEnabledComponents.Count; i++)
            {
                AbstractPassive passive = onlyEnabledComponents[i];
                if(passive.whenDo == wichOnes)
                {
                    passive.DoPassive(cObject.transform.position, _input);
                    passive.DoAnimationParameters();
                }
            }
        }
        public void RunLate(InputValues _input)
        {   
            for(int i = 0; i < onlyEnabledComponents.Count; i++)
            {
                AbstractPassive passive = onlyEnabledComponents[i];
                passive.DoPassiveLate(cObject.transform.position, _input);
            }
        }
        
    }
}
