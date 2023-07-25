using UnityEngine;
using sapra.InputHandler;
using sapra.ObjectController;

[RequireComponent(typeof(InputContainer))]
[RequireComponent(typeof(StatsContainer))]
[RequireComponent(typeof(CMotor))]
public class CEntity : ObjectController
{
    public bool continuosCheck;
    
    public ActiveModule activeModule = new ActiveModule();
    public PassiveModule passiveModule = new PassiveModule();
    public HelperModule helperModule = new HelperModule();
    public CMotor motor;
    private InputContainer inputHolder;
    private InputValues _input{get{
        if(inputHolder)
            return inputHolder.input;
        else
            return null;}}
    private void Start()
    {
        inputHolder = GetComponent<InputContainer>();
        motor = GetComponent<CMotor>();
    }    
    void Update()
    {      
        if(continuosCheck)
            InitializeController();
        motor.AfterSimulation();
        passiveModule.Run(PassivePriority.FirstOfAll, _input);
        passiveModule.Run(PassivePriority.BeforeActive, _input );
        activeModule.Run(_input);
        passiveModule.Run(PassivePriority.AfterActive, _input);
        passiveModule.Run(PassivePriority.LastOne, _input);
        motor.UpdateMotor();
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