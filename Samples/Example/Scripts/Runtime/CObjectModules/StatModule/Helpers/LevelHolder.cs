using UnityEngine;
using System.Collections.Generic;
namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class LevelHolder : AbstractValue
    {
        [SerializeReference] private LevelValue levelValue = null;
        public float randomValue;
        public override void ReloadStat()
        {
            if(childStat == null)
                childStat = new NamedStat(null, "");
            NamedStat found = findStat(childStat.getPath());
            if(found.getStat() != childStat.getStat())
            {
                if(childStat.getStat() != null)
                    childStat.getStat().setDefault();
                childStat = found;
            }
            if(validateRelationship() == ValidationResult.Valid)
            {
                this.levelValue = new LevelValue(this.levelValue);
                childStat.getStat().ChangeInitialStatType(levelValue);
            }
            else
                this.levelValue = null;
        }
        public LevelHolder(SForces sForces)
        {
            this.forces = sForces;
            this.levelValue = null;
            this.childStat = new NamedStat(null, "");
        }
        public ValidationResult validateRelationship()
        {
            if(forces == null)
                return ValidationResult.RequiresInitializing;
            if(childStat.getStat() == (null))
                return ValidationResult.MissingChild;
            return ValidationResult.Valid;
        }
    }
    [System.Serializable]
    public class LevelHolderList
    {
        public List<LevelHolder> statLevels = new List<LevelHolder>();
        [SerializeReference] private SForces forcesReferences;
        public void Initialize(SForces forces) 
        {
            this.forcesReferences = forces;
            for(int i = statLevels.Count-1; i >= 0; i--)
            {
                LevelHolder selected = statLevels[i];
                if(selected != null)
                    selected.Initialize(forcesReferences);
                else
                    statLevels.RemoveAt(i);
            }
        }
        public void CreateNewItem()
        {
            statLevels.Add(new LevelHolder(forcesReferences));
        }
        public void RemoveItem(int index)
        {
            LevelHolder relationship = statLevels[index];
            relationship.Remove();
            statLevels.RemoveAt(index);
        }
    }
}