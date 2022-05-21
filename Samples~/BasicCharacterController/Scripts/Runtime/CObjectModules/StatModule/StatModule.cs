using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class StatModule : AbstractModule<AbstractStat>
    {        
        public void Run(bool continuosCheck)
        {
            if(continuosCheck)
                InitializeRoutines(this.controller);
            foreach(AbstractStat stat in onlyEnabledRoutines)
            {
                stat.DoExtra();
            }
        }
    }
}
