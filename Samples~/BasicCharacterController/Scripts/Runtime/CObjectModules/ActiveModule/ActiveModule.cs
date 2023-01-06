using System.Collections.Generic;
using UnityEngine;
using sapra.ObjectController;

[System.Serializable]
public class ActiveModule : AbstractModule<AbstractActive>
{
    [SerializeReference] [HideInInspector]
    public AbstractActive currentAction;

    [SerializeReference]
    public List<AbstractActive> sortedShorterList = new List<AbstractActive>();
    protected override void InitializeModule() {
        GenerateSortedList();
    }
    
    public void GenerateSortedList()
    {
        for(int i = sortedShorterList.Count-1; i>= 0; i--)
        {
            AbstractActive component = sortedShorterList[i];
            if(!onlyEnabledRoutines.Contains(component))
            {
                sortedShorterList.RemoveAt(i);
            }
        }
        for(int i = 0; i<onlyEnabledRoutines.Count; i++)
        {
            AbstractActive component = onlyEnabledRoutines[i];
            if(!sortedShorterList.Contains(component))
            {
                sortedShorterList.Add(component);
            }
        }
    }
    public void Run(InputValues _input)
    {
        if(_input == null)
            return;
        bool foundCurrentAction = false;
        AbstractActive initialAction = currentAction;
        for(int i = sortedShorterList.Count-1; i >= 0; i--)
        {
            AbstractActive action = sortedShorterList[i];
            action.DoPassiveBeforeAction();  
            if(!foundCurrentAction)
            {
                if(action.WantActive(_input))
                {              
                    ExecuteAction(action, _input);
                    foundCurrentAction = true;
                }
                else            
                if(currentAction == action)                   
                    currentAction = null;
            }
            action.DoPassiveAfterAction();
        }
        EventCalls(initialAction, currentAction);
    }
    
    private void EventCalls(AbstractActive initialAction, AbstractActive finalAction)
    {
        if(initialAction != finalAction)
        {
            if(initialAction != null && initialAction.events.onDeactivateAction != null)
                initialAction.events.onDeactivateAction.Invoke();
            if(finalAction != null && finalAction.events.onActivateAction != null)
                finalAction.events.onActivateAction.Invoke();
/*                 if(finalAction == null && whenNullAction != null)
                whenNullAction.Invoke(); */
        }
    }
    private void ExecuteAction(AbstractActive action, InputValues _input)
    {
        if(currentAction != action)
            currentAction = action;
        action.DoActive(_input);
    }
}