using UnityEngine;
using UnityEditor;

namespace sapra.ObjectController.Samples.Editor
{
    [CustomPropertyDrawer(typeof(Stat), true)]
    public class StatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty finalValue = property.FindPropertyRelative("initialStatType");
            if(finalValue == null)
                return;
            
            AbstractInitialValue statType = finalValue.managedReferenceValue as AbstractInitialValue;
            if(statType == null)
                return;
            if(statType.GetType() == (typeof(NormalValue)))
            {
                NormalValue casted = statType as NormalValue;
                casted.baseValue = EditorGUI.FloatField(position, label, casted.baseValue);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.FloatField(position, label, statType.value);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
