using UnityEngine;
using sapra.ObjectController;

public enum PassivePriority{FirstOfAll,BeforeActive, AfterActive, LastOne}
[System.Serializable]
public abstract class AbstractPassive : AbstractRoutine
{    
    public abstract void DoPassive(PassivePriority currentPassivePriority, Vector3 position, InputValues input);
    public virtual void DoPassiveLate(Vector3 position, InputValues input){}
}
