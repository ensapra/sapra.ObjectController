using UnityEngine;
using UnityEditor;
using sapra.ObjectController;

namespace sapra.ObjectController.Editor
{
    [CustomEditor(typeof(AbstractCObject), true)]
    public class CObjectEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            AbstractCObject component = this.target as AbstractCObject;
            component.GetAllComponents();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            LoadReloadButton(); 
            serializedObject.ApplyModifiedProperties();      
        }
        void LoadReloadButton()
        {
            EditorGUILayout.BeginHorizontal();
            bool currentEnabled = serializedObject.FindProperty("onlyEnabled").boolValue;
            if(GUILayout.Button("Reload requirements"))
            {
                loadRequirements(currentEnabled);
            }        
            string text;
            if(currentEnabled)
                text = "Show all";
            else
                text = "Show only enabled";
            if(GUILayout.Button(text))
            {
                currentEnabled = !currentEnabled;
                loadRequirements(currentEnabled);
            }
            EditorGUILayout.EndHorizontal();
        }
        void loadRequirements(bool showEnabled)
        {
            AbstractCObject component = this.target as AbstractCObject;
            Undo.RecordObject(component, "Reloaded requirements in " + target.name);
            component.InitializeObject(true);
            component.SwitchTo(showEnabled);
            serializedObject.ApplyModifiedProperties();      
        }
    }
}