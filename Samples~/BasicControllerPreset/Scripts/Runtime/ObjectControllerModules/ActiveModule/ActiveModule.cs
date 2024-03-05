using System.Collections.Generic;
using UnityEngine;
using sapra.ObjectController;
using NaughtyAttributes;

[System.Serializable]
public class ActiveModule : Module<ActiveRoutine>
{
    [SerializeReference] [HideInInspector]
    public ActiveRoutine currentAction;

    [SerializeReference]
    public List<ActiveRoutine> sortedShorterList = new List<ActiveRoutine>();
    protected override void InitializeModule() {
        GenerateSortedList();
    }
    
    public void GenerateSortedList()
    {
        for(int i = sortedShorterList.Count-1; i>= 0; i--)
        {
            ActiveRoutine component = sortedShorterList[i];
            if(!baseEnabledRoutines.Contains(component))
            {
                sortedShorterList.RemoveAt(i);
            }
        }
        for(int i = 0; i<baseEnabledRoutines.Count; i++)
        {
            ActiveRoutine component = baseEnabledRoutines[i];
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
        ActiveRoutine initialAction = currentAction;
        for(int i = sortedShorterList.Count-1; i >= 0; i--)
        {
            ActiveRoutine action = sortedShorterList[i];
            action.DoPassiveBeforeAction();  
            if(!foundCurrentAction)
            {
                if(action.WantActive(_input))
                {     
                    if(action != currentAction)
                    {
                        if(currentAction != null)
                            currentAction.OnStopActive(_input);
                        
                        action.OnStartActive(_input);
                        currentAction = action;
                    }
                    action.UpdateActive(_input);
                    foundCurrentAction = true;
                }
                else            
                if(currentAction == action)   
                {                
                    currentAction.OnStopActive(_input);
                    currentAction = null;
                }
            }
            action.DoPassiveAfterAction();
        }
    }
    
}