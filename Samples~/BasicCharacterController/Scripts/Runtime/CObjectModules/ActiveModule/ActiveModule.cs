using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class ActiveModule : AbstractModule<AbstractActive>
    {
        [SerializeReference]
        public AbstractActive currentAction;
        public UnityEvent whenNullAction;
        [SerializeReference]
        public List<AbstractActive> sortedShorterList = new List<AbstractActive>();
        public override void InitializeComponents(AbstractCObject controller)        {
            base.InitializeComponents(controller);
            GenerateSortedList();
        }
        public void GenerateSortedList()
        {
            for(int i = sortedShorterList.Count-1; i>= 0; i--)
            {
                AbstractActive component = sortedShorterList[i];
                if(!onlyEnabledComponents.Contains(component))
                {
                    sortedShorterList.RemoveAt(i);
                }
            }
            for(int i = 0; i<onlyEnabledComponents.Count; i++)
            {
                AbstractActive component = onlyEnabledComponents[i];
                if(!sortedShorterList.Contains(component))
                {
                    sortedShorterList.Add(component);
                }
            }
        }
        public void Run(InputValues _input, bool continuosCheck)
        {
            if(continuosCheck)
                InitializeComponents(this.controller);
            if(_input == null)
                return;
            bool foundCurrentAction = false;
            AbstractActive initialAction = currentAction;
            for(int i = sortedShorterList.Count-1; i >= 0; i--)
            {
                AbstractActive action = sortedShorterList[i];
                action.DoPassive();  
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
                action.DoAnimationParameters();
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
                if(finalAction == null && whenNullAction != null)
                    whenNullAction.Invoke();
            }
        }
        private void ExecuteAction(AbstractActive action, InputValues _input)
        {
            if(currentAction != action)
                currentAction = action;
            action.DoActive(_input);
        }
    }
}
