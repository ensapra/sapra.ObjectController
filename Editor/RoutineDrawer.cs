using UnityEngine;
using UnityEditor;
using sapra.ObjectController;

namespace sapra.ObjectController.Editor
{
    [CustomPropertyDrawer(typeof(AbstractRoutine), true)]
    public class RoutineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.y += EditorGUIUtility.standardVerticalSpacing*2;
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty item = property.FindPropertyRelative("wantsAwake");
            var depth = item.depth;
            int count = 0;
            
            while(item.NextVisible(false) && item.depth >= depth)
            {
                SerializedProperty isUsed = item.FindPropertyRelative("isUsed");
                if(isUsed == null || isUsed.boolValue)
                {
                    position.height = EditorGUI.GetPropertyHeight(item);
                    EditorGUI.PropertyField(position, item, true);
                    position.y += EditorGUI.GetPropertyHeight(item)+EditorGUIUtility.standardVerticalSpacing;
                    count++;
                }
            }
            if(count == 0)
            {
                EditorGUI.indentLevel += 1;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, "No fields on this component");
                EditorGUI.indentLevel -= 1;
            }    
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty item = property.FindPropertyRelative("wantsAwake");
            var depth = item.depth;
            int count = 0;
            while(item.NextVisible(false) && item.depth >= depth)
            {
                SerializedProperty isUsed = item.FindPropertyRelative("isUsed");
                if(isUsed == null || isUsed.boolValue)
                {
                    height += EditorGUI.GetPropertyHeight(item)+EditorGUIUtility.standardVerticalSpacing;
                    count++;
                }
            }
            if(count == 0)
            {
                height = EditorGUIUtility.singleLineHeight*2;
            }  
            return height;
        }
    }
}
