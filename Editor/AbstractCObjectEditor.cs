using UnityEngine;
using UnityEditor;
using sapra.ObjectController;

namespace sapra.ObjectController.Editor
{
    [CustomEditor(typeof(AbstractCObject), true)]
    public class AbstractCObjectEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            AbstractCObject component = this.target as AbstractCObject;
            component.LoadModuleRoutines();
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
            Rect rect = EditorGUILayout.GetControlRect();
            rect = new Rect(rect.x-15, rect.y, rect.width+15, rect.height+5);
            if(GUI.Button(rect, "Reload requirements"))
            {
                loadRequirements();
            }        
            EditorGUILayout.EndHorizontal();
        }
        void loadRequirements()
        {
            AbstractCObject component = this.target as AbstractCObject;
            Undo.RecordObject(component, "Reloaded requirements in " + target.name);
            component.LoadModuleRoutines();
            serializedObject.ApplyModifiedProperties();      
        }
    }
}