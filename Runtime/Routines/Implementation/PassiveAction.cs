using UnityEngine;

namespace sapra.ObjectController
{
    public enum PassivePriority{FirstOfAll,BeforeActive, AfterActive, LastOne, never}
    [System.Serializable]
    public abstract class AbstractPassive : AbstractRoutine<AbstractCObject>
    {    
        [SerializeField]
        public abstract PassivePriority whenDo{get;}
        public abstract void DoPassive(Vector3 position, InputValues input);
        public virtual void DoPassiveLate(Vector3 position, InputValues input){}
        public virtual void DoAnimationParameters(){}
    }
}
