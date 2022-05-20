using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    public class SForces : AbstractStat
    {
        [SerializeReference] public Stat maximumSpeed = new Stat(new NormalValue(13));
        [SerializeReference] public Stat maximumSlideSpeed = new Stat(new NormalValue(15));
        [SerializeReference] public Stat maximumWaterSpeed = new Stat(new NormalValue(16));
        [SerializeReference] public Stat maxFJump = new Stat(new NormalValue(15));
        [SerializeReference] public Stat minimumSpeed = new Stat(new NormalValue(6));
        [SerializeReference] public Stat passiveSpeed = new Stat(new NormalValue(10));
        [SerializeReference] public Stat minimumSlideSpeed = new Stat(new NormalValue(6));
        [SerializeReference] public Stat minimumWaterSpeed = new Stat(new NormalValue(13));

        [SerializeReference] public Stat selectedSpeed = new Stat(new ExternallySetValue());

        private List<Stat> allStats = new List<Stat>();
        [HideInInspector] public Vector3 currentRBSpeed;
        protected override void AwakeComponent(AbstractCObject cObject)        {
            allStats.Clear();
            allStats.Add(maximumSpeed);
            allStats.Add(maximumSlideSpeed);
            allStats.Add(maximumWaterSpeed);
            allStats.Add(maxFJump);
            allStats.Add(minimumSpeed);
            allStats.Add(passiveSpeed);
            allStats.Add(minimumSlideSpeed);
            allStats.Add(minimumWaterSpeed);
            allStats.Add(selectedSpeed);
            foreach(Stat stat in allStats)
            {
                stat.DeSelect();
            }
        }
        public override void DoExtra()
        {
            currentRBSpeed = cObject.rb.velocity;
            foreach(Stat stat in allStats)
            {
                stat.restartValue();
            }
        }
    }
}
