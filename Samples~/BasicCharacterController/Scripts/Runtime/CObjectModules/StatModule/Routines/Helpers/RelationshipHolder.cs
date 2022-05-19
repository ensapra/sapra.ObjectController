using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace sapra.ObjectController
{
    public enum ValidationResult{SameComponents, MissingChild, MissingParent, Loop, Other, Valid, RequiresInitializing}
    [System.Serializable]
    public class RelationshipHolder : AbstractValue
    {
        [SerializeReference] public NamedStat parentStat = new NamedStat(null, "");
        [SerializeReference] private RelatioshipValue relationshipValue = null;
        public RelationshipHolder(SForces forces, string parentPath, string childPath, float ratio)
        {
            this.parentStat = new NamedStat(null, parentPath);
            this.childStat = new NamedStat(null, childPath);
            this.relationshipValue = null;
            this.forces = forces;
        }        
        public override void ReloadStat()
        {
            UpdateChild();
            UpdateParent();
            GenerateRelationShip();
        }
        private void UpdateChild()
        {
            if(childStat == null)
                childStat = new NamedStat(null, "");
            string childStatName = childStat.getPath();
            NamedStat found = findStat(childStatName);
            if(parentStat.getStat() != childStat.getStat() || !parentStat.getPath().Equals(found.getPath()))
            {
                if(childStat.getStat() != null)            
                    childStat.getStat().setDefault();
                childStat = found;     
            }   
        }
        private void UpdateParent()
        {
            if(parentStat == null)
                parentStat = new NamedStat(null, "");
            string parentStatName = parentStat.getPath();
            NamedStat found = findStat(parentStatName);
            if(parentStat.getStat() != childStat.getStat() || !parentStat.getPath().Equals(found.getPath()))
                parentStat = found;    
        }

        private void GenerateRelationShip()
        {
            float existingRatio = this.relationshipValue != null ? this.relationshipValue.ratio : 1;
            if(validateRelationship() == ValidationResult.Valid)
            {
                relationshipValue = new RelatioshipValue(parentStat.getStat(), existingRatio, RelationShipType.M);
                childStat.getStat().ChangeInitialStatType(relationshipValue);
                relationshipValue.startLoop();
            }
            else
                relationshipValue = null;
        }
        
        public ValidationResult validateRelationship()
        {
            if(forces == null)
                return ValidationResult.RequiresInitializing;
            if(childStat.getStat() == (null))
                return ValidationResult.MissingChild;
            if(parentStat.getStat() == (null))
                return ValidationResult.MissingParent;
            if(parentStat.getPath().Equals(childStat.getPath()))
                return ValidationResult.SameComponents;
            if(this.relationshipValue != null)
                if(this.relationshipValue.loop)
                    return ValidationResult.Loop;

            return ValidationResult.Valid;
        }
        public void reload(float value)
        {
            if(relationshipValue != null)
            {
                relationshipValue.ratio = value;
            }
        }
    }

    [System.Serializable]
    public class RelationshipHolderList
    {
        public List<RelationshipHolder> statRelationships = new List<RelationshipHolder>();
        [SerializeReference] private SForces forcesReferences;
        public void Initialize(SForces forces) 
        {
            this.forcesReferences = forces;
            for(int i = statRelationships.Count-1; i >= 0; i--)
            {
                RelationshipHolder selected = statRelationships[i];
                if(selected != null)
                    selected.Initialize(forcesReferences);
                else
                    statRelationships.RemoveAt(i);
            }
        }
        public void CreateNewItem()
        {
            statRelationships.Add(new RelationshipHolder(forcesReferences, "", "", 1));
        }
        public void RemoveItem(int index)
        {
            RelationshipHolder relationship = statRelationships[index];
            relationship.Remove();
            statRelationships.RemoveAt(index);
        }
    }

}