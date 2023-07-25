using UnityEngine;
using UnityEditor;
using sapra.ObjectController;

namespace sapra.ObjectController.Editor
{
    [CustomEditor(typeof(ObjectController), true)]
    public class ObjectControllerEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            ObjectController component = this.target as ObjectController;
            component.InitializeController();
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
            ObjectController component = this.target as ObjectController;
            Undo.RecordObject(component, "Reloaded requirements in " + target.name);
            component.InitializeController();
            serializedObject.ApplyModifiedProperties();      
        }
    }
}