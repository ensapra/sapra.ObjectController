using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using sapra.ObjectController;
using System.IO;
using System.Reflection;
using System.Linq;


namespace sapra.ObjectController.Editor
{
    [CustomPropertyDrawer(typeof(AbstractModule), true)]
    public class ModuleDrawer : PropertyDrawer
    {
        protected GUIStyle boxButtonStyle;
        protected GUIStyle buttonStyle;
        protected GUIStyle headerStyle;
        protected GUIStyle addItems;
        protected GUIStyle workingListStyle;

        //private bool deleteUnused = false;
        /// <summary>
        /// Used to add extra layout after the basic Module Layout
        /// <summary/>

        protected List<AbstractRoutine> AllRoutines = new List<AbstractRoutine>();
        protected virtual void ExtraModuleData(SerializedProperty property, GUIContent label){}
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indent = EditorGUI.indentLevel;
            property.serializedObject.Update();
            EditorGUI.BeginProperty(position, label, property);
            CreateGUIStyles();
            SerializeModule(property, position);
            EditorGUI.indentLevel++;
            ExtraModuleData(property, label);
            EditorGUI.indentLevel--;
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
        private void SerializeModule(SerializedProperty module, Rect position)
        {
            if(module == null)
                return;

            ModuleHeader(position, module);
            ObjectList(module);
        }
        protected void ObjectList(SerializedProperty module)
        {
            SerializedProperty prop = module.FindPropertyRelative("onlyEnabledRoutines");
            EditorGUI.indentLevel += 1;
            GUILayout.Space(5);
            if(prop.isExpanded)
            {
                for(int i = 0; i < prop.arraySize; i++)
                {
                    SerializedProperty item = prop.GetArrayElementAtIndex(i);
                    LoadAbstractRoutine(item);
                }

                if(prop.arraySize <= 0)     
                {       
                    EditorGUILayout.LabelField("No components enabled on this module");
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                }
                //Load cached routines
                SerializedProperty createdRoutines = module.FindPropertyRelative("cachedRoutines");
                if(createdRoutines.arraySize > 0)     
                {       
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                    EditorGUILayout.LabelField("Saved routines");
                }
                for(int i = 0; i < createdRoutines.arraySize; i++)
                {
                    SerializedProperty item = createdRoutines.GetArrayElementAtIndex(i);
                    LoadAbstractRoutine(item);
                }               
            }
            EditorGUI.indentLevel -= 1;
        }
        private void ModuleHeader(Rect position, SerializedProperty module)
        {
            Rect boxRect = new Rect(position.x-15, position.y, position.width+15, position.height+5);
            Rect buttonRect = new Rect(position);
            buttonRect.xMax -= 42;
            buttonRect.xMin += 150;
            buttonRect.y += EditorGUIUtility.standardVerticalSpacing;
            Rect onlyEnabledRect = new Rect(position);
            onlyEnabledRect.xMin = buttonRect.xMax+2;
            onlyEnabledRect.xMax = buttonRect.xMax+22;
            onlyEnabledRect.y += EditorGUIUtility.standardVerticalSpacing;
            Rect dropDownRect = new Rect(position);
            dropDownRect.xMin = onlyEnabledRect.xMax;
            dropDownRect.y += EditorGUIUtility.standardVerticalSpacing*1.5f;
            Rect toggleRect = new Rect(position);
            toggleRect.xMax = buttonRect.xMin;
            toggleRect.y += EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty removeUnused = module.FindPropertyRelative("RemoveUnused");
            bool deleteUnused = removeUnused.boolValue;
            GUI.Box(boxRect, "");
            string deleteUnusedText = deleteUnused ? "R" : "K";
            Color current = GUI.color;
            if(deleteUnused)
                GUI.color = Color.red;
            if(GUI.Button(onlyEnabledRect, deleteUnusedText)) {
                removeUnused.boolValue = !removeUnused.boolValue;
            }
            GUI.color = current;

            SerializedProperty prop = module.FindPropertyRelative("onlyEnabledRoutines");
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
                item.FindPropertyRelative("_isEnabled").boolValue = false;
            }
        }
        /// <summary>
        /// Generates the default FoldoutMenu of routines to be selected
        /// <summary/>
        protected void GenerateFoldoutMenu(SerializedProperty list, SerializedProperty property)
        {
            AbstractModule module = property.GetSerializedObject() as AbstractModule;
            List<System.Type> types = module.GetAssemblyRoutines();
            AbstractRoutine[] routines = module.EnabledRoutinesObject;
            AbstractRoutine[] cached = module.ChachedRoutinesObject;
            SerializedProperty cachedList = property.FindPropertyRelative("cachedRoutines");

            GenericMenu newMenu = new GenericMenu();
            for(int i = 0; i < types.Count; i++)
            {
                System.Type target = types[i];
                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(target);
                string routeName = ObjectName(target.FullName);
                foreach(System.Attribute attr in attrs)
                {
                    if(attr is RoutineCategoryAttribute)
                    {
                        routeName = ((RoutineCategoryAttribute)attr).Category + routeName;
                    }
                }

                GUIContent content = new GUIContent(routeName);
                bool exists = routines != null && routines.Any(a => a != null && a.GetType().IsEquivalentTo(target));
                if(exists)
                {
                    (AbstractRoutine rot, int index) foundRoutine = routines.Select((obj, index) => (obj,index)).First(a => a.obj != null && a.obj.GetType().IsEquivalentTo(target));
                    if(foundRoutine.rot.isEnabled)
                    {
                        //The object exists and is enabled
                        newMenu.AddDisabledItem(content);
                    }
                    else
                    {
                        //The object exists but is not enabled
                        newMenu.AddItem(content, false, ()=>{
                            SerializedProperty routineFound = list.GetArrayElementAtIndex(foundRoutine.index);
                            routineFound.FindPropertyRelative("_isEnabled").boolValue = true;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                else
                {
                    exists = cached != null && cached.Any(a => a != null && a.GetType().IsEquivalentTo(target));
                    if(exists)
                    {
                        //Exists on cache
                        (AbstractRoutine rot, int index) foundRoutine = cached.Select((obj, index) => (obj,index)).First(a => a.obj != null && a.obj.GetType().IsEquivalentTo(target));
                        newMenu.AddItem(content, false, ()=>{
                            if(list.arraySize <= 0)
                                list.arraySize = 1;
                            else
                                list.InsertArrayElementAtIndex(list.arraySize-1);
                            SerializedProperty newClass = list.GetArrayElementAtIndex(list.arraySize-1);
                            newClass.managedReferenceValue = cachedList.GetArrayElementAtIndex(foundRoutine.index).managedReferenceValue;
                            newClass.FindPropertyRelative("_isEnabled").boolValue = true;
                            cachedList.DeleteArrayElementAtIndex(foundRoutine.index);
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    else
                    {
                        //The object doesn't exist on the main array, so create it again
                        newMenu.AddItem(content, false, ()=>{
                            if(list.arraySize <= 0)
                                list.arraySize = 1;
                            else
                                list.InsertArrayElementAtIndex(list.arraySize-1);
                            SerializedProperty newClass = list.GetArrayElementAtIndex(list.arraySize-1);
                            newClass.managedReferenceValue = System.Activator.CreateInstance(target);
                            newClass.FindPropertyRelative("_isEnabled").boolValue = true;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
            }
            newMenu.ShowAsContext();
        }
        private void LoadAbstractRoutine(SerializedProperty item)
        {
            Rect position = EditorGUILayout.GetControlRect();           
            AbstractRoutineHeader(position, item);
            EditorGUI.indentLevel += 2;
            if(item.isExpanded)                
                EditorGUILayout.PropertyField(item);
            GUILayout.Space(4);
            EditorGUI.indentLevel -= 2;
        }

        /// <summary>
        /// Generates the Header of a Routine
        /// <summary/>
        protected void AbstractRoutineHeader(Rect position, SerializedProperty AbstractRoutineProperty)
        {
            string correctPropertyName = ObjectName(AbstractRoutineProperty.managedReferenceFullTypename);
            SerializedProperty enabledBool = AbstractRoutineProperty.FindPropertyRelative("_isEnabled");

            Rect boxPosition = position;
            boxPosition.height = 22;
            Rect togglePosition = boxPosition;
            togglePosition.xMax = 35;
            togglePosition.x += 5;
            Rect buttonPosition = boxPosition;
            buttonPosition.xMin = togglePosition.xMax;
            
            Event current = Event.current;
            GUI.Box(boxPosition, "", boxButtonStyle);
            enabledBool.boolValue = GUI.Toggle(togglePosition,enabledBool.boolValue, "");
            GUI.Label(buttonPosition, correctPropertyName, buttonStyle);
            if(buttonPosition.Contains(current.mousePosition) && current.type == EventType.MouseDown)
            {
                if(current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Edit Script"),false, () => 
                    {
                        OpenFile(AbstractRoutineProperty.managedReferenceValue.GetType());                 
                    });
                    menu.ShowAsContext();
                    current.Use();
                }
                else if (current.button == 0)
                {
                    AbstractRoutineProperty.isExpanded = !AbstractRoutineProperty.isExpanded;
                    current.Use();
                }
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
        /// <summary>
        /// Returns the name of a routine, without the first letter
        /// <summary/>
        protected string ObjectName(string name)
        {
            string[] propertyName = name.Split('.');
            string lastBit = propertyName[propertyName.Length-1];
            propertyName = lastBit.Split(" ");
            lastBit = propertyName[propertyName.Length-1];
            string noFirst = lastBit.Substring(1);
            return UpperSplit(noFirst);
        }

        public void OpenFile(System.Type type)
        {
            string[] Guid = AssetDatabase.FindAssets( string.Format( "{0} t:script", type.Name));
            if(Guid.Length > 0)
            {
                string dataPath = Application.dataPath;
                dataPath = Directory.GetParent(dataPath).FullName + "/";
                var path = dataPath + AssetDatabase.GUIDToAssetPath(Guid[0]);
                System.Diagnostics.Process.Start(@path);
            }   
        }
    }
}