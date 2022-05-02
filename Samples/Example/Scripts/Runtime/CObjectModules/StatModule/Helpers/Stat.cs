using UnityEngine;

namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class Stat
    {
        [SerializeField] protected bool isUsed = false;
        public bool _isUsed{get{return isUsed;}}
        [SerializeReference] private AbstractInitialValue initialStatType = new NormalValue(0);
        [SerializeReference] private AbstractInitialValue defaultStatType;
        public virtual float value => initialStatType.value;
        public void ChangeInitialStatType(AbstractInitialValue initialValue)
        {
            this.initialStatType = initialValue;
        }
        public AbstractInitialValue getStatType(){return initialStatType;}
        public Stat(AbstractInitialValue initialValue)
        {
            this.initialStatType = initialValue;
            this.defaultStatType = initialStatType;
        }
        public Stat() : this(new NormalValue(0)){}
        
        public Stat Select()
        {
            this.isUsed = true;
            return this;
        }
        public void DeSelect()
        {
            isUsed = false;
            setDefault();
        }
        public void setDefault()
        {
            if(defaultStatType == null)
                this.defaultStatType = new NormalValue(0);
            this.initialStatType = defaultStatType;
        }
        public void restartValue()
        {
            initialStatType.resetInitialStat();
        }
        public void changeValue(float value)
        {
            initialStatType.changeValue(value);
        }
        public void createBoundaries(float minValue, float maxValue)
        {
            initialStatType.createBoundaries(minValue, maxValue);
        }
    }
}