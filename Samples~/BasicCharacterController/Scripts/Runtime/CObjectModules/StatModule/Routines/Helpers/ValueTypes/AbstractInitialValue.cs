using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractInitialValue
    {
        public float value => applyBoundaries();
        protected abstract float setValue{get;}
        public virtual void changeValue(float value){}
        public virtual void resetInitialStat(){
            this.modifiedClamp = false;
        }
        
        private bool modifiedClamp = false;
        private float maxValue;
        private float minValue;

        public float applyBoundaries()
        {
            if(modifiedClamp)
            {
                changeValue(Mathf.Clamp(this.setValue, minValue, maxValue));
            }
            return this.setValue;
        }
        public void createBoundaries(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            modifiedClamp = true;
        }
    }  
}
