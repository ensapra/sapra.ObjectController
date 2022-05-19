namespace sapra.ObjectController
{
    [System.Serializable]
    public class NormalValue : AbstractInitialValue
    {
        protected override float setValue => baseValue;
        public float baseValue;
        public NormalValue(float reference)
        {
            this.baseValue = reference;
        }
    }  
}