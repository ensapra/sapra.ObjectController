using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace sapra.ObjectController
{
    [System.Serializable]
    public class PassiveModule : AbstractModule<AbstractPassive>
    {
        public void Run(PassivePriority wichOnes, InputValues _input)
        {   
            for(int i = 0; i < onlyEnabledRoutines.Count; i++)
            {
                AbstractPassive passive = onlyEnabledRoutines[i];
                if(passive.whenDo == wichOnes)
                {
                    passive.DoPassive(controller.transform.position, _input);
                    passive.DoAnimationParameters();
                }
            }
        }
        public void RunLate(InputValues _input)
        {   
            for(int i = 0; i < onlyEnabledRoutines.Count; i++)
            {
                AbstractPassive passive = onlyEnabledRoutines[i];
                passive.DoPassiveLate(controller.transform.position, _input);
            }
        }
        
    }
}
