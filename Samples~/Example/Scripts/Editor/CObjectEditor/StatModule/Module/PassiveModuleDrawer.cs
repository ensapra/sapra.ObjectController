using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using sapra.ObjectController;
using sapra.ObjectController.Editor;

namespace sapra.ObjectController.Samples.Editor
{
    [CustomPropertyDrawer(typeof(PassiveModule))]
    public class PassiveModuleDrawer : ModuleDrawer
    {
        protected override void GenerateFoldoutMenu(SerializedProperty list, SerializedProperty property)
        {
            GenericMenu newMenu = new GenericMenu();
            for(int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty item = list.GetArrayElementAtIndex(i);
                string[] propertyName = item.managedReferenceFullTypename.Split('.');
                string finalName = ObjectName(propertyName[propertyName.Length-1]);
                if(finalName.Contains("Detection"))
                {
                    finalName = "Detections/" + finalName;
                }
                else if(finalName.Contains("S M"))
                    finalName = "Modifiers/" + finalName;
                else
                    finalName = "Others/" +finalName;
                GUIContent content = new GUIContent(finalName);
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
    }
}
