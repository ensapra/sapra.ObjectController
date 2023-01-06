using UnityEngine.Events;
using sapra.ObjectController;

[System.Serializable]
public abstract class AbstractActive : AbstractRoutine
{
    protected ActiveModule activeModule;
    public abstract int priorityID{get;}
    public bool isActive{get{
        if(activeModule == null)
            activeModule = this.controller.RequestModule<ActiveModule>();
        return activeModule.currentAction == this;}}
    public CurrentEvents events = new CurrentEvents();
    public abstract bool WantActive(InputValues input);
    public abstract void DoActive(InputValues input);
    public virtual void DoPassiveBeforeAction(){}
    public virtual void DoPassiveAfterAction(){}

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
