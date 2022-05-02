using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractStat : ObjectComponent
    {
        public virtual void DoExtra(){}
    }
}
