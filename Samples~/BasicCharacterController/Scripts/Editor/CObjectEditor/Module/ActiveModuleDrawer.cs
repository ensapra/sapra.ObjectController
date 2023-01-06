using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using sapra.ObjectController.Editor;

[CustomPropertyDrawer(typeof(ActiveModule))]
public class ActiveModuleDrawer : ModuleDrawer
{
    ReorderableList enabledActives;
    SerializedProperty selected;

    protected override void ExtraModuleData(SerializedProperty property, GUIContent label)
    {
        if(enabledActives == null)
            MakeList(property);   
        SerializedProperty listProperty = property.FindPropertyRelative("onlyEnabledRoutines");
        if(listProperty.isExpanded)
        {           
            SerializedProperty enabledList = property.FindPropertyRelative("sortedShorterList");
            Rect rect = EditorGUILayout.GetControlRect();
            Rect labelRect = new Rect(rect.x, rect.y, 100, rect.height);
            Rect buttonRect = new Rect(rect.x+ labelRect.width, rect.y, rect.width-labelRect.width, rect.height);
            enabledList.isExpanded = EditorGUI.Foldout(labelRect, enabledList.isExpanded, "Priority Order", true);
            if(GUI.Button(buttonRect, "Sort"))
            {
                SortTheList(enabledList);
            }
            if(enabledList.isExpanded)
                enabledActives.DoLayoutList();
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("whenNullAction"));
        } 
    }
    void SortTheList(SerializedProperty property)
    {
        if(property.isArray)
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Sorted the list of " +property.serializedObject.targetObject.name);
            int i, j;
            int N = property.arraySize;
            for (j=1; j<N; j++) {
                for (i=j; i>0 && (property.GetArrayElementAtIndex(i).managedReferenceValue as AbstractActive).priorityID 
                    < (property.GetArrayElementAtIndex(i-1).managedReferenceValue as AbstractActive).priorityID; i--) {
                        SerializedProperty second = property.GetArrayElementAtIndex(i-1);
                        property.MoveArrayElement(i, i - 1);
                }
            }
        }
    }

    void MakeList(SerializedProperty property)
    {
        selected = property.FindPropertyRelative("currentAction");
        SerializedProperty activeList = property.FindPropertyRelative("sortedShorterList");
        enabledActives = new ReorderableList(property.serializedObject, activeList, 
    true, false, false, false);
    enabledActives.drawElementCallback = 
                (Rect rect, int index, bool isActive, bool isFocused) => 
                {
                    var element = enabledActives.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    if(element.type.Equals(selected.type))
                        EditorGUI.LabelField(new Rect(rect.x,rect.y, rect.width, EditorGUIUtility.singleLineHeight),ObjectName(element.managedReferenceFullTypename) + " <- Current Action", workingListStyle);
                    else
                        EditorGUI.LabelField(new Rect(rect.x,rect.y, rect.width, EditorGUIUtility.singleLineHeight),ObjectName(element.managedReferenceFullTypename));
            };
    }

}

