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
        protected override void AwakeComponent(AbstractCObject cObject)        {
            sForces = cObject.FindModule<StatModule>().RequestComponent<SForces>(true);
            if(levelList == null)
                levelList = new LevelHolderList();
            levelList.Initialize(sForces);
        }
    }
}
