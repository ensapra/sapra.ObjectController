using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace sapra.ObjectController.Editor
{
    public abstract class HolderDrawer : PropertyDrawer
    {
        protected GUIStyle errorText;
        protected GUIStyle warningText;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GenerateStyles();
            property.serializedObject.Update();
            EditorGUI.BeginProperty(position, label, property);

            position.y+=2;
            position.height = EditorGUIUtility.singleLineHeight;
            Rect topLineRect = position;
            Rect bottomLineRect = topLineRect;
            bottomLineRect.y += EditorGUIUtility.singleLineHeight+4;

            topLine(topLineRect, property);
            bottomLine(bottomLineRect, property);
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }
        
        protected abstract void topLine(Rect position, SerializedProperty property);
        protected abstract void bottomLine(Rect position, SerializedProperty property);
        protected FieldInfo[] getFields(SerializedProperty property, SForces sForces)
        {
            FieldInfo[] initials = (typeof(SForces)).GetFields();
            List<FieldInfo> finals = new List<FieldInfo>();
            if(sForces == null)
                return initials;
            foreach(FieldInfo field in initials)
            {
                if(field.FieldType.Equals(typeof(Stat)))
                {
                    Stat statFound = field.GetValue(sForces) as Stat;
                    if(statFound.getStatType().GetType().Equals(typeof(NormalValue)) && statFound._isUsed)
                        finals.Add(field);
                }
            }
            return finals.ToArray();
        }
        public void GenerateStyles()
        {
            if(warningText == null)
            {
                warningText = new GUIStyle(EditorStyles.boldLabel);
                warningText.normal.textColor = new Color(1,1,0,.8f);
            }
            if(errorText == null)
            {
                errorText = new GUIStyle(EditorStyles.boldLabel);
                errorText.normal.textColor = new Color(1,.2f,.2f,.8f);
            }
        }
    }
}