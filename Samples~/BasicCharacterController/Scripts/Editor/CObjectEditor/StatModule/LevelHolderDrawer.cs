using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;

namespace sapra.ObjectController.Editor
{
    [CustomPropertyDrawer(typeof(LevelHolder))]
    public class LevelHolderDrawer : HolderDrawer
    {
        protected override void topLine(Rect position, SerializedProperty property)
        {
            LevelHolder levelHolder = property.GetSerializedObject() as LevelHolder;
            SerializedProperty childStatStruct = property.FindPropertyRelative("childStat");
            SerializedProperty levelValue = property.FindPropertyRelative("levelValue");
            SerializedProperty baseValue = levelValue != null ? levelValue.FindPropertyRelative("valueAtLevel1") : null;

            SerializedProperty childName = childStatStruct.FindPropertyRelative("path");
            SerializedProperty childStat = childStatStruct.FindPropertyRelative("stat");
            GUIContent childContent = new GUIContent(childName.stringValue);
            float spacing = 5;
            float smallWidht = 16; 
            float valuesSpace = ((position.width/2)-smallWidht*2-spacing)/2;
            Rect labelC = position;
            labelC.width = smallWidht;
            Rect dropC = position;
            dropC.x = labelC.x+labelC.width;
            dropC.width = ((position.width)/2)-smallWidht-spacing;

            Rect baseValureLabelRect = labelC;
            baseValureLabelRect.x = dropC.x+dropC.width+spacing;
            Rect baseValueRect = position;
            baseValueRect.x = baseValureLabelRect.x+baseValureLabelRect.width;
            baseValueRect.width = valuesSpace;
            Rect finalValureLabelRect = labelC;
            finalValureLabelRect.x = baseValueRect.x+baseValueRect.width+spacing;
            Rect finalValueRect = baseValueRect;
            finalValueRect.x = finalValureLabelRect.x+finalValureLabelRect.width;

            EditorGUI.LabelField(labelC, "C:");
            if(EditorGUI.DropdownButton(dropC, childContent, FocusType.Passive))
            {
                getVariableNames(childStatStruct, levelHolder).DropDown(dropC);
            }
            if(baseValue != null)
            {
                EditorGUI.LabelField(baseValureLabelRect, "B:");
                baseValue.floatValue = EditorGUI.FloatField(baseValueRect, baseValue.floatValue);
            }
            if(childStat.managedReferenceValue != null)
            {
                EditorGUI.LabelField(finalValureLabelRect, "F:");
                EditorGUI.PropertyField(finalValueRect, childStat, GUIContent.none);
            }
        }

        protected override void bottomLine(Rect position, SerializedProperty property)
        {
            LevelHolder levelHolder = property.GetSerializedObject() as LevelHolder;
            SerializedProperty levelValue = property.FindPropertyRelative("levelValue");
            SerializedProperty levelList = levelValue != null ? levelValue.FindPropertyRelative("levelCurvesList") : null;

            EditorGUI.BeginChangeCheck();
            ValidationResult result = levelHolder.validateRelationship();
            switch(result)
            {
                case ValidationResult.MissingChild:
                    EditorGUI.LabelField(position, "No stat selected",warningText);
                break;
                case ValidationResult.RequiresInitializing:
                    EditorGUI.LabelField(position, "Needs to be Initialized", warningText);
                break;
                case ValidationResult.Valid:
                    EditorGUI.PropertyField(position, levelList);
                    EditorGUI.EndFoldoutHeaderGroup();
                break;
                default:
                    EditorGUI.LabelField(position, "Unkown Error", errorText);
                break;
            }
            EditorGUI.EndChangeCheck();
        }
        public GenericMenu getVariableNames(SerializedProperty selectedDropDown, LevelHolder statLevel)
        {
            SerializedProperty statName = selectedDropDown.FindPropertyRelative("path");

            string currentSelected = statName.stringValue;
            FieldInfo[] variables = getFields(selectedDropDown, statLevel.forces);
            GenericMenu content = new GenericMenu();
            foreach(FieldInfo field in variables)
            {
                if(field.FieldType.Equals(typeof(Stat)))
                {
                    content.AddItem(new GUIContent(field.Name),currentSelected.Equals(field.Name),()=>{
                        statName.stringValue = field.Name;
                        selectedDropDown.serializedObject.ApplyModifiedProperties();
                        statLevel.ReloadStat();
                    });
                }
            }
            content.AddItem(new GUIContent("NONE"),currentSelected.Equals(""),()=>{
                statName.stringValue = "";
                selectedDropDown.serializedObject.ApplyModifiedProperties();
                statLevel.ReloadStat();
            });
            return content;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            LevelHolder target = property.GetSerializedObject() as LevelHolder;
            if(target == null)
                return 0;
            ValidationResult result = target.validateRelationship();
            switch(result)
            {
                case ValidationResult.Valid:
                    SerializedProperty levelValue = property.FindPropertyRelative("levelValue");
                    SerializedProperty levelList = levelValue.FindPropertyRelative("levelCurvesList");
                    float extraHeight = EditorGUI.GetPropertyHeight(levelList);
                    return EditorGUIUtility.singleLineHeight*2+8+extraHeight;
                default:
                    return EditorGUIUtility.singleLineHeight*2+8;
            }
        }
    }

    [CustomPropertyDrawer(typeof(LevelHolderList))]
    public class LevelHolderListDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(!_reorderableLists.ContainsKey(property.propertyPath))
                GenerateList(property);
            return _reorderableLists[property.propertyPath].GetHeight();
        }
        public void GenerateList(SerializedProperty property)
        {
            SerializedProperty myDataList = property.FindPropertyRelative("statLevels");
            if (!_reorderableLists.ContainsKey(property.propertyPath) || _reorderableLists[property.propertyPath].index > _reorderableLists[property.propertyPath].count - 1)
            {
                var currentList = _reorderableLists[property.propertyPath] = new ReorderableList(myDataList.serializedObject, myDataList, false, true, true, true);
                currentList.drawHeaderCallback = (Rect rect) => {EditorGUI.LabelField(rect, "Levels");};
                currentList.elementHeightCallback = (int index) => {
                    return EditorGUI.GetPropertyHeight(myDataList.GetArrayElementAtIndex(index));
                };
                currentList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    rect.height = EditorGUI.GetPropertyHeight(myDataList.GetArrayElementAtIndex(index));
                    EditorGUI.PropertyField(rect, myDataList.GetArrayElementAtIndex(index), true);}
                    ;
                currentList.onAddCallback = (ReorderableList list) => {
                    LevelHolderList listField = property.GetSerializedObject() as LevelHolderList;
                    listField.CreateNewItem();
                    property.serializedObject.ApplyModifiedProperties();
                }; 
                currentList.onRemoveCallback = (ReorderableList list) => {
                    LevelHolderList listField = property.GetSerializedObject() as LevelHolderList;
                    listField.RemoveItem(list.index);
                    property.serializedObject.ApplyModifiedProperties();
                };
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            position = EditorGUI.IndentedRect(position);
            EditorGUI.BeginProperty(position, label, property);     
            if(!_reorderableLists.ContainsKey(property.propertyPath))
                GenerateList(property);
            var currentList = _reorderableLists[property.propertyPath];
            currentList.DoList(position);
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

    }

    [CustomPropertyDrawer(typeof(LevelCurvesList))]
    public class LevelCurvesListDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(!_reorderableLists.ContainsKey(property.propertyPath))
                GenerateList(property);
            return _reorderableLists[property.propertyPath].GetHeight()-10;
        }
        public void GenerateList(SerializedProperty property)
        {
            SerializedProperty myDataList = property.FindPropertyRelative("levelCurves");
            if (!_reorderableLists.ContainsKey(property.propertyPath) || _reorderableLists[property.propertyPath].index > _reorderableLists[property.propertyPath].count - 1)
            {
                var currentList = _reorderableLists[property.propertyPath] = new ReorderableList(myDataList.serializedObject, myDataList, true, true, true, true);
                currentList.drawHeaderCallback = (Rect rect) => {EditorGUI.LabelField(rect, "Levelers");};
                currentList.elementHeightCallback = (int index) => {
                    return 1.1f*EditorGUIUtility.singleLineHeight;};
                currentList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    EditorGUI.PropertyField(rect, myDataList.GetArrayElementAtIndex(index), true);}
                    ;
                currentList.drawNoneElementCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "There are no levelers selected");
                };
                currentList.onAddCallback = (ReorderableList list) =>{
                    LevelCurvesList listField = property.GetSerializedObject() as LevelCurvesList;
                    listField.GenerateLevel();
                    property.serializedObject.ApplyModifiedProperties();
                };
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            position = EditorGUI.IndentedRect(position);
            EditorGUI.BeginProperty(position, label, property);
            if(!_reorderableLists.ContainsKey(property.propertyPath))
                GenerateList(property);
            _reorderableLists[property.propertyPath].DoList(position);
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }
    }


    [CustomPropertyDrawer(typeof(LevelCurves))]
    public class LevelCurvesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            position = EditorGUI.IndentedRect(position);
            EditorGUI.BeginProperty(position, label, property);
            DrawSingleLine(property, position);
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }
        public void DrawSingleLine(SerializedProperty property, Rect position)
        {
            SerializedProperty statType = property.FindPropertyRelative("statType");
            SerializedProperty currentLevel = property.FindPropertyRelative("currentLevel");
            SerializedProperty animationCurve = property.FindPropertyRelative("levelUpCurve");
            position.y += 2;
            position.height = EditorGUIUtility.singleLineHeight;
            Rect typeRect = position;
            float spacing = 5;
            typeRect.width = ((position.width/2)/2);
            Rect levelRect = typeRect;
            levelRect.x = typeRect.x+typeRect.width+spacing;
            levelRect.width -= spacing;
            Rect animationCurveRect = position;
            animationCurveRect.x = levelRect.x + levelRect.width+spacing;
            animationCurveRect.width = position.width/2-spacing; 

            statType.enumValueIndex = EditorGUI.Popup(typeRect, statType.enumValueIndex, statType.enumDisplayNames);
            currentLevel.floatValue = EditorGUI.FloatField(levelRect, currentLevel.floatValue);
            EditorGUI.PropertyField(animationCurveRect, animationCurve, GUIContent.none);
        }
    }
}