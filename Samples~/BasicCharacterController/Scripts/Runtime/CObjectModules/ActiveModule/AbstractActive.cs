using UnityEngine;
using UnityEngine.Events;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractActive : AbstractRoutine<CObject>
    {
        public abstract int priorityID{get;}
        public bool isActive{get{return cObject.activeModule.currentAction == this;}}
        public CurrentEvents events = new CurrentEvents();
        public abstract bool WantActive(InputValues input);
        public abstract void DoActive(InputValues input);
        public virtual void DoPassive(){}
        public virtual void DoAnimationParameters(){}

        [System.Serializable]
        public class CurrentEvents
        {
            public UnityEvent onActivateAction;
            public UnityEvent onDeactivateAction;
            public CurrentEvents()
            {
                onActivateAction = new UnityEvent();
                onDeactivateAction = new UnityEvent();
            }
        }
    }
}
