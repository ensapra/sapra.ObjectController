using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public abstract class AbstractStat : AbstractRoutine<CObject>
    {
        public virtual void DoExtra(){}
    }
}
