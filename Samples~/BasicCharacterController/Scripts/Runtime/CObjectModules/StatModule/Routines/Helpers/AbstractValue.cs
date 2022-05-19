using UnityEngine;
using System.Reflection;
namespace sapra.ObjectController
{
    [System.Serializable]
    public abstract class AbstractValue
    {
        [SerializeReference] public NamedStat childStat = new NamedStat(null, "");
        [SerializeReference] public SForces forces;
        protected NamedStat findStat(string variableName)
        {
            if(variableName.Equals(""))
                return new NamedStat(null, variableName);
            if(forces != null)
            {
                FieldInfo statInfo = forces.GetType().GetField(variableName);
                if(statInfo != null)
                {
                    Stat found = statInfo.GetValue(forces) as Stat;
                    if(found != null)
                        return new NamedStat(found, variableName);
                }
            }
            return new NamedStat(null, variableName);
        }
        public void Initialize(SForces forces)
        {
            this.forces = forces;
            ReloadStat();
        }
        public void Remove()
        {
            if(this.childStat.getStat() != null)
                this.childStat.getStat().setDefault();
        }
        public abstract void ReloadStat();
    }
    [System.Serializable]
    public class NamedStat
    {
        [SerializeReference] private Stat stat;
        [SerializeField] private string path;
        public NamedStat(Stat stat, string path)
        {
            this.stat = stat;
            this.path = path;
        }
        public Stat getStat()
        {
            return this.stat;
        }
        public string getPath()
        {
            return this.path;
        }
    }
}