using UnityEngine.Events;
using sapra.ObjectController;

[System.Serializable]
public abstract class ActiveRoutine : Routine
{
    protected ActiveModule activeModule;
    public abstract int priorityID{get;}
    public bool isActive{get{
        if(activeModule == null)
            activeModule = this.GetModule<ActiveModule>();
        return activeModule.currentAction == this;}}
    public abstract bool WantActive(InputValues input);
    
    public abstract void UpdateActive(InputValues input);
    public virtual void OnStartActive(InputValues input){}
    public virtual void OnStopActive(InputValues input){}
    
    public virtual void DoPassiveBeforeAction(){}
    public virtual void DoPassiveAfterAction(){}

}
