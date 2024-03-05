using sapra.ObjectController;

[System.Serializable]
public class PassiveModule : Module<AbstractPassive>
{
    public void Run(PassivePriority currentPriority, InputValues _input)
    {   
        for(int i = 0; i < baseEnabledRoutines.Count; i++)
        {
            AbstractPassive passive = baseEnabledRoutines[i];
            passive.DoPassive(currentPriority,controller.transform.position, _input);
        }
    }
    public void RunLate(InputValues _input)
    {   
        for(int i = 0; i < baseEnabledRoutines.Count; i++)
        {
            AbstractPassive passive = baseEnabledRoutines[i];
            passive.DoPassiveLate(controller.transform.position, _input);
        }
    }
    
}
