using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace sapra.ObjectController
{
    public class SRelationship : AbstractStat
    {
        public RelationshipHolderList relationshipList = new RelationshipHolderList();
        private SForces forces;
        protected override void AwakeRoutine(AbstractCObject controller)        {
            forces = controller.RequestModule<StatModule>().RequestRoutine<SForces>(true);
            relationshipList.Initialize(forces);
        }
    }
}
