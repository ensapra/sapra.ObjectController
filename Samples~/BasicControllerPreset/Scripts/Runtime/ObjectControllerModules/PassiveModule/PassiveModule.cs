using sapra.ObjectController;

[System.Serializable]
public class PassiveModule : Module<AbstractPassive>
{
    public void Run(PassivePriority currentPriority, InputValues _input)
    {   
        for(int i = 0; i < onlyEnabledRoutines.Count; i++)
        {
            AbstractPassive passive = onlyEnabledRoutines[i];
            passive.DoPassive(currentPriority,controller.transform.position, _input);
        }
    }
    public void RunLate(InputValues _input)
    {   
        for(int i = 0; i < onlyEnabledRoutines.Count; i++)
        {
            AbstractPassive passive = onlyEnabledRoutines[i];
            passive.DoPassiveLate(controller.transform.position, _input);
        }
    }
    
}
