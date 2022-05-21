using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    public enum StatType{Speed}

    [System.Serializable]
    public class SLeveler : AbstractStat
    {
        public LevelHolderList levelList = new LevelHolderList();
        private SForces sForces;
        protected override void AwakeRoutine(AbstractCObject controller)        {
            sForces = controller.RequestModule<StatModule>().RequestRoutine<SForces>(true);
            if(levelList == null)
                levelList = new LevelHolderList();
            levelList.Initialize(sForces);
        }
    }
}
