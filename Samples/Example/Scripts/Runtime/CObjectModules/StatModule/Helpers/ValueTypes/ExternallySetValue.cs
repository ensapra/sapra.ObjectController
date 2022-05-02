namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class ExternallySetValue : AbstractInitialValue
    {
        protected float selectedValue;
        protected override float setValue => selectedValue;
        public override void changeValue(float value)
        {
            this.selectedValue = value;
        }
    }   
}
