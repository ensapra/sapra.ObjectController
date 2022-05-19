using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using sapra.ObjectController;

namespace sapra.ObjectController.Editor
{
    public abstract class ModuleDrawer : PropertyDrawer
    {
        protected GUIStyle boxButtonStyle;
        protected GUIStyle buttonStyle;
        protected GUIStyle headerStyle;
        protected GUIStyle addItems;
        protected GUIStyle workingListStyle;

        protected virtual void ExtraModuleData(SerializedProperty property, GUIContent label){}
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.BeginProperty(position, label, property);
            CreateGUIStyles();
            SerializeModule(property, position);
            EditorGUI.indentLevel += 1;
            ExtraModuleData(property, label);
            EditorGUI.indentLevel -= 1;
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        private void CreateGUIStyles()
        {
            if(boxButtonStyle == null)
            {
                boxButtonStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
                boxButtonStyle.padding = new RectOffset(25,10,0,0);
            }
            if(buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                buttonStyle.alignment = TextAnchor.MiddleLeft;
            }
            if(headerStyle == null)
            {
                headerStyle = new GUIStyle(GUI.skin.GetStyle("Foldout"));
                headerStyle.fontStyle = FontStyle.Bold;
            }
            if(addItems == null)
            {
                addItems = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            }
            if(workingListStyle == null)
            {
                workingListStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                workingListStyle.fontStyle = FontStyle.Bold;
            }
        }
        public void SerializeModule(SerializedProperty module, Rect position)
        {
            if(module == null)
                return;

            ModuleHeader(position, module);
            ObjectList(module);
        }
        protected virtual void ObjectList(SerializedProperty module)
        {
            SerializedProperty prop = module.FindPropertyRelative("allComponents");
            bool onlyEnabled = module.FindPropertyRelative("onlyEnabled").boolValue;
            EditorGUI.indentLevel += 1;
            GUILayout.Space(5);
            if(prop.isExpanded)
            {
                bool anElement = false;
                for(int i = 0; i < prop.arraySize; i++)
                {
                    SerializedProperty item = prop.GetArrayElementAtIndex(i);
                    bool result = LoadAbstractRoutine(item, i, onlyEnabled);
                    anElement = anElement || result;
                }
                if(!anElement)     
                {       
                    EditorGUILayout.LabelField("No components enabled on this module");
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                }
            }
            EditorGUI.indentLevel -= 1;
        }
        private void ModuleHeader(Rect position, SerializedProperty module)
        {
            Rect boxRect = new Rect(position.x-15, position.y, position.width+15, position.height+5);
            Rect buttonRect = new Rect(position);
            buttonRect.xMax -= 20;
            buttonRect.xMin += 150;
            buttonRect.y += EditorGUIUtility.standardVerticalSpacing;
            Rect dropDownRect = new Rect(position);
            dropDownRect.xMin = buttonRect.xMax;
            dropDownRect.y += 2;
            Rect toggleRect = new Rect(position);
            toggleRect.xMax = buttonRect.xMin;
            toggleRect.y += EditorGUIUtility.standardVerticalSpacing;

            GUI.Box(boxRect, "");
            SerializedProperty prop = module.FindPropertyRelative("allComponents");
            prop.isExpanded = EditorGUI.Foldout(toggleRect, prop.isExpanded, UpperSplit(module.name), true, headerStyle);
            if(GUI.Button(buttonRect, "Clear"))
            {
                ClearList(prop, module);
            }
            if(GUI.Button(dropDownRect, "", addItems))
            {
                GenerateFoldoutMenu(prop, module);
            }
        }
        void ClearList(SerializedProperty list, SerializedProperty property)
        {        
            Undo.RecordObject(property.serializedObject.targetObject, "Clearing a list of enabled components");
            for(int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty item = list.GetArrayElementAtIndex(i);
                item.FindPropertyRelative("wantsAwake").boolValue = false;
            }
        }
        protected virtual void GenerateFoldoutMenu(SerializedProperty list, SerializedProperty property)
        {
            GenericMenu newMenu = new GenericMenu();
            for(int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty item = list.GetArrayElementAtIndex(i);
                GUIContent content = new GUIContent(ObjectName(item.managedReferenceFullTypename));
                bool enabled = item.FindPropertyRelative("wantsAwake").boolValue;
                if(enabled)            
                    newMenu.AddDisabledItem(content);
                else
                {
                    newMenu.AddItem(content, false, ()=>{
                        item.FindPropertyRelative("wantsAwake").boolValue = true;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            newMenu.ShowAsContext();
        }
        private bool LoadAbstractRoutine(SerializedProperty item, int index, bool onlyEnabled)
        {
            SerializedProperty enabledBool = item.FindPropertyRelative("wantsAwake");
            if(!onlyEnabled || enabledBool.boolValue)
            {     
                Rect position = EditorGUILayout.GetControlRect();           
                AbstractRoutineHeader(position, item);
                if(item.isExpanded)                
                    EditorGUILayout.PropertyField(item);
                GUILayout.Space(4);
                return true;
            }
            return false;
        }
        protected void AbstractRoutineHeader(Rect position, SerializedProperty AbstractRoutine)
        {
            string correctPropertyName = ObjectName(AbstractRoutine.managedReferenceFullTypename);
            SerializedProperty enabledBool = AbstractRoutine.FindPropertyRelative("wantsAwake");

            Rect boxPosition = position;
            boxPosition.height = 22;
            Rect togglePosition = boxPosition;
            togglePosition.xMax = 35;
            togglePosition.x += 5;
            Rect buttonPosition = boxPosition;
            buttonPosition.xMin = togglePosition.xMax;
            
            GUI.Box(boxPosition, "", boxButtonStyle);
            enabledBool.boolValue = GUI.Toggle(togglePosition,enabledBool.boolValue, "");
            if(GUI.Button(buttonPosition, correctPropertyName, buttonStyle)) {
                AbstractRoutine.isExpanded = !AbstractRoutine.isExpanded;
            }
        }
        string UpperSplit(string name)
        {
            name = name[0].ToString().ToUpper() + name.Substring(1);
            string result = "";
            bool foundLow = false;
            for (int i = 0; i < name.Length; i++)
            {
                if(char.IsLower(name[i]))
                    foundLow = true;
                
                if (char.IsUpper(name[i]) && foundLow)
                {
                    result += ' ';
                }
            
                result += name[i];
            }
            return result;
        }
        protected string ObjectName(string name)
        {
            string[] propertyName = name.Split('.');
            string lastBit = propertyName[propertyName.Length-1];
            propertyName = lastBit.Split(" ");
            lastBit = propertyName[propertyName.Length-1];
            string noFirst = lastBit.Substring(1);
            return UpperSplit(noFirst);
        }
    }
}