using System.Collections.Generic;
using UnityEngine;
namespace sapra.ObjectController.Samples
{    
    public enum RelationShipType{M, A}
    [System.Serializable]
    public class RelatioshipValue : AbstractInitialValue
    {
        protected override float setValue => processRelationShip();
        public float ratio;
        [SerializeReference] private Stat parentValue;
        public bool loop;
        public RelationShipType relationType;
        private float processRelationShip()
        {
           if(loop)
                return 0;
            else
            {
                switch(relationType)
                {
                    case RelationShipType.A:
                        return parentValue.value+ratio;
                    case RelationShipType.M:
                        return parentValue.value*ratio;
                    default:
                        return parentValue.value*ratio;
                }
            }
        }
        public RelatioshipValue(Stat parentValue, float ratio, RelationShipType relationType)
        {
            this.parentValue = parentValue;
            this.ratio = ratio;
            this.relationType = relationType;
        }
        public bool checkLoop(List<RelatioshipValue> currentList)
        {
            if(currentList.Contains(this))
                return true;
            else
            {
                AbstractInitialValue initial = parentValue.getStatType();
                if(initial.GetType().Equals(typeof(RelatioshipValue)))
                {
                    currentList.Add(this);
                    return (initial as RelatioshipValue).checkLoop(currentList);;
                }
                else
                    return false;
            }
        }
        public bool startLoop()
        {
            AbstractInitialValue initial = parentValue.getStatType();
            if(initial.GetType().Equals(typeof(RelatioshipValue)))
            {
                loop = (initial as RelatioshipValue).checkLoop(new List<RelatioshipValue>(){this});
                return loop;
            }
            else
                return false;
        }
    }
}