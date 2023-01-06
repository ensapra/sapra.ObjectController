using UnityEngine;
using sapra.ObjectController;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputContainer))]
[RequireComponent(typeof(StatsContainer))]
public class CEntity : AbstractCObject
{
    public bool continuosCheck;
    
    public ActiveModule activeModule = new ActiveModule();
    public PassiveModule passiveModule = new PassiveModule();
    public HelperModule helperModule = new HelperModule();
    private InputContainer inputHolder;
    private InputValues _input{get{
        if(inputHolder)
            return inputHolder.input;
        else
            return null;}}
    private void Start()
    {
        inputHolder = GetComponent<InputContainer>();
    }    
    void FixedUpdate()
    {      
        if(continuosCheck)
            InitializeController();
        passiveModule.Run(PassivePriority.FirstOfAll, _input);
        passiveModule.Run(PassivePriority.BeforeActive, _input );
        activeModule.Run(_input);
        passiveModule.Run(PassivePriority.AfterActive, _input);
        passiveModule.Run(PassivePriority.LastOne, _input);
    }
    private void LateUpdate() {
        passiveModule.RunLate(_input);   
    }

    protected override void addModules()
    {
        AddModule(passiveModule);
        AddModule(activeModule);
        AddModule(helperModule);
    }
}