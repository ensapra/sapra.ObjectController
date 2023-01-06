using UnityEngine;
using UnityEditor;
public class NoEditAttribute : PropertyAttribute
{ }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NoEditAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var previousGUIState = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = previousGUIState;
    }
}
#endif