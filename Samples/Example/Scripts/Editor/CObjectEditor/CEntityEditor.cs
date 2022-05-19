using UnityEngine;
using UnityEditor;
using sapra.ObjectController;
using sapra.ObjectController.Editor;

namespace sapra.ObjectController.Samples.Editor
{
    [CustomEditor(typeof(CObject))]
    public class CObjectEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            CObject component = this.target as CObject;
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
            CObject component = this.target as CObject;
            Undo.RecordObject(component, "Reloaded requirements in " + target.name);
            component.InitializeObject(true);
            component.SwitchTo(showEnabled);
            serializedObject.ApplyModifiedProperties();      
        }
    }
}