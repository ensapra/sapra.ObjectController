using UnityEngine;
using System.Collections.Generic;

namespace sapra.ObjectController.Samples
{
    [System.Serializable]
    public class LevelValue : AbstractInitialValue
    {
        protected override float setValue => processValue();
        public float valueAtLevel1 = 10;
        [SerializeReference] public LevelCurvesList levelCurvesList = new LevelCurvesList();
        public float processValue()
        {
            if(levelCurvesList == null)
                levelCurvesList = new LevelCurvesList();
            List<LevelCurves> levelCurves = levelCurvesList.levelCurves;
            float finalValue = 0;
            for(int i = 0; i<levelCurves.Count; i++)
            {
                LevelCurves levelCurve = levelCurves[i];
                finalValue += (levelCurve.levelUpCurve.Evaluate(levelCurve.currentLevel)*valueAtLevel1)/levelCurves.Count;
            }
            return finalValue;
        }
        public LevelValue(){}
        public LevelValue(LevelValue previous)
        {
            if(previous != null)
            {
                levelCurvesList = previous.levelCurvesList;
                this.valueAtLevel1 = previous.valueAtLevel1;
            }
        }
    }
    [System.Serializable]
    public class LevelCurves
    {
        public StatType statType;
        public float currentLevel = 1;
        public AnimationCurve levelUpCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0f,0.7f), new Keyframe(1f,1f), new Keyframe(3f,3f)});
        public LevelCurves()
        {
            this.currentLevel = 1;
            this.levelUpCurve = new AnimationCurve(new Keyframe[]{new Keyframe(0f,0.7f), new Keyframe(1f,1f), new Keyframe(3f,3f)});
        }
    }
    [System.Serializable]
    public class LevelCurvesList
    {
        public List<LevelCurves> levelCurves = new List<LevelCurves>();
        public void GenerateLevel()
        {
            LevelCurves levelCurve = new LevelCurves();
            levelCurves.Add(levelCurve);
        }
    }
}