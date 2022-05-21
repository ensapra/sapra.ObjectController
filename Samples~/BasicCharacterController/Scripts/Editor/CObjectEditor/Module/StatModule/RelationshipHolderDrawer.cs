using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;

namespace sapra.ObjectController.Editor
{
    [CustomPropertyDrawer(typeof(RelationshipHolder))]
    public class RelationshipHolderDrawer : HolderDrawer
    {
        protected override void topLine(Rect position, SerializedProperty property)
        {
            RelationshipHolder relationshipHolder = property.GetSerializedObject() as RelationshipHolder;
            SerializedProperty childStatStruct = property.FindPropertyRelative("childStat");
            SerializedProperty parentStatStruct = property.FindPropertyRelative("parentStat");
            SerializedProperty childName = childStatStruct.FindPropertyRelative("path");
            SerializedProperty parentName = parentStatStruct.FindPropertyRelative("path");
            GUIContent childContent = new GUIContent(childName.stringValue);
            GUIContent parentContent = new GUIContent(parentName.stringValue);
            float spacing = 5;
            float totalWidth = position.width;
            float smallWidht = 16; 
            float bigWidth = (totalWidth-spacing-smallWidht*2)/2;
            Rect labelC = position;
            labelC.width = smallWidht;
            Rect dropC = position;
            dropC.x = labelC.x+labelC.width;
            dropC.width = bigWidth;
            Rect labelP = labelC;
            labelP.x = dropC.x+ dropC.width+spacing;
            Rect dropP = dropC;
            dropP.x = labelP.x+labelP.width;

            EditorGUI.LabelField(labelC, "C:");
            if(EditorGUI.DropdownButton(dropC, childContent, FocusType.Passive))
            {
                getVariableNames(childStatStruct, relationshipHolder, parentStatStruct, true).DropDown(dropC);
            }
            EditorGUI.LabelField(labelP, "P:");
            if(EditorGUI.DropdownButton(dropP, parentContent, FocusType.Passive))
            {
                getVariableNames(parentStatStruct, relationshipHolder, childStatStruct, false).DropDown(dropP);
            }
        }

        protected override void bottomLine(Rect position, SerializedProperty property)
        {
            RelationshipHolder relationshipHolder = property.GetSerializedObject() as RelationshipHolder;
            ValidationResult result = relationshipHolder.validateRelationship();
            switch(result)
            {
                case ValidationResult.Loop:
                    EditorGUI.LabelField(position, "Loop on variables", errorText);
                break;
                case ValidationResult.MissingChild:
                    EditorGUI.LabelField(position, "No child stat selected",warningText);
                break;
                case ValidationResult.MissingParent:
                    EditorGUI.LabelField(position, "No parent stat selected",warningText);
                break;
                case ValidationResult.Other:
                    EditorGUI.LabelField(position, "Unkown Error", errorText);
                break;
                case ValidationResult.SameComponents:
                    EditorGUI.LabelField(position, "The same componets have been selected", errorText);
                break;
                case ValidationResult.RequiresInitializing:
                    EditorGUI.LabelField(position, "Needs to be Initialized", warningText);
                break;
                case ValidationResult.Valid:
                    ValidBottomLine(position, property);
                break;
            }        
        }
        private void ValidBottomLine(Rect position, SerializedProperty property)
        {
            SerializedProperty childStatStruct = property.FindPropertyRelative("childStat");
            SerializedProperty parentStatStruct = property.FindPropertyRelative("parentStat");
            SerializedProperty childStat = childStatStruct.FindPropertyRelative("stat");
            SerializedProperty parentStat = parentStatStruct.FindPropertyRelative("stat");

            SerializedProperty relationshipValue = property.FindPropertyRelative("relationshipValue");
            SerializedProperty ratio = relationshipValue != null ? relationshipValue.FindPropertyRelative("ratio") : null;
            SerializedProperty method = relationshipValue != null ? relationshipValue.FindPropertyRelative("relationType") : null;
            int initialLabelWidth = 80;
            Rect InitialLabel = position;
            InitialLabel.width = initialLabelWidth;
            float childStatWidth = (position.width/2)-initialLabelWidth;
            int ratioWidth = 40;
            int dropWidth = 30;
            int equalWidth= 15;
            int spacing = 8;
            float statWidth = ((position.width/2)-ratioWidth-dropWidth-equalWidth-spacing*2);
            Rect childStatRect = position;
            childStatRect.x = InitialLabel.x+InitialLabel.width;
            childStatRect.width = childStatWidth;
            Rect equalRect = childStatRect;
            equalRect.x = childStatRect.x+childStatRect.width;
            equalRect.width = equalWidth;
            Rect parentStatRect = position;
            parentStatRect.x = equalRect.x+equalRect.width;
            parentStatRect.width = statWidth;
            Rect methodRect = position;
            methodRect.x = parentStatRect.x+parentStatRect.width+spacing;
            methodRect.width = dropWidth;
            Rect ratioRet = position;
            ratioRet.x = methodRect.x+methodRect.width+spacing;
            ratioRet.width = ratioWidth;
            GUIContent label = new GUIContent("Final value:");
            EditorGUI.PropertyField(childStatRect, childStat, GUIContent.none);
            EditorGUI.PropertyField(parentStatRect, parentStat, GUIContent.none);
            EditorGUI.LabelField(equalRect, new GUIContent(" = "));
            EditorGUI.LabelField(InitialLabel, label);
            if(ratio != null && method != null)
            {
                EditorGUI.LabelField(ratioRet, "= R:");
                ratio.floatValue = EditorGUI.FloatField(ratioRet, ratio.floatValue);
                string[] opt = new string[]{"*", "+"};
                method.enumValueIndex = EditorGUI.Popup(methodRect, method.enumValueIndex,opt);
            }
        }
        public GenericMenu getVariableNames(SerializedProperty selectedDropDown, RelationshipHolder stat, SerializedProperty contraryStat,bool child)
        {
            SerializedProperty statName = selectedDropDown.FindPropertyRelative("path");
            SerializedProperty contraryStatName = contraryStat.FindPropertyRelative("path");

            string currentSelected = statName.stringValue;
            FieldInfo[] variables = child ? getFields(selectedDropDown, stat.forces) : (typeof(SForces)).GetFields();
            GenericMenu content = new GenericMenu();
            foreach(FieldInfo field in variables)
            {
                if(field.FieldType.Equals(typeof(Stat)))
                {
                    if(field.Name.Equals(contraryStatName.stringValue))
                        continue;
                    content.AddItem(new GUIContent(field.Name),currentSelected.Equals(field.Name),()=>{
                        statName.stringValue = field.Name;
                        selectedDropDown.serializedObject.ApplyModifiedProperties();
                        stat.ReloadStat();
                    });
                }
            }
            content.AddItem(new GUIContent("NONE"),currentSelected.Equals(""),()=>{
                statName.stringValue = "";
                selectedDropDown.serializedObject.ApplyModifiedProperties();
                stat.ReloadStat();
            });
            return content;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight*2+8;
        }
    }

    [CustomPropertyDrawer(typeof(RelationshipHolderList))]
    public class RelationshipHolderListDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty myDataList = property.FindPropertyRelative("statRelationships");
            if (!_reorderableLists.ContainsKey(property.propertyPath) || _reorderableLists[property.propertyPath].index > _reorderableLists[property.propertyPath].count - 1)
            {
                var currentList = _reorderableLists[property.propertyPath] = new ReorderableList(myDataList.serializedObject, myDataList, false, true, true, true);
                currentList.drawHeaderCallback = (Rect rect) => {EditorGUI.LabelField(rect, "Relationships");};
                currentList.elementHeightCallback = (int index) => {
                    return 2.6f*EditorGUIUtility.singleLineHeight;};
                currentList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    EditorGUI.PropertyField(rect, myDataList.GetArrayElementAtIndex(index), true);}
                    ;
                currentList.onAddCallback = (ReorderableList list) => {
                    RelationshipHolderList listField = property.GetSerializedObject() as RelationshipHolderList;
                    listField.CreateNewItem();
                }; 
                currentList.onRemoveCallback = (ReorderableList list) => {
                    RelationshipHolderList listField = property.GetSerializedObject() as RelationshipHolderList;
                    listField.RemoveItem(list.index);
                };
            }
            return _reorderableLists[property.propertyPath].GetHeight();
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);
            EditorGUI.BeginProperty(position, label, property);
            _reorderableLists[property.propertyPath].DoList(position);
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}