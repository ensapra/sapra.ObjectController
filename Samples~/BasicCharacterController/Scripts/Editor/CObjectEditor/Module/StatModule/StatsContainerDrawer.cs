using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatsContainer), true)]
public class StatsContainerDrawer : Editor {
    public override void OnInspectorGUI() {
        StatsContainer container = this.target as StatsContainer;
        base.OnInspectorGUI();
        var previousGUIState = GUI.enabled;

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("Limiters", EditorStyles.boldLabel);
        GUI.enabled = false;
        GenerateField(nameof(container.DynamicMovingSpeed), container.DynamicMovingSpeed);
        VisualizeCurrentModifiers(container.GetCurrentModifiers());
        GUI.enabled = true;

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("Values", EditorStyles.boldLabel);

        GUI.enabled = false;
        GenerateField(nameof(container.MaximumSpeed), container.MaximumSpeed);
        GenerateField(nameof(container.MinimumSpeed), container.MinimumSpeed);
        GenerateField(nameof(container.MiddleSpeed), container.MiddleSpeed);
        GenerateField(nameof(container.MaximumSlideSpeed), container.MaximumSlideSpeed);
        GenerateField(nameof(container.MinimumSlideSpeed), container.MinimumSlideSpeed);
        GenerateField(nameof(container.MaximumSwimSpeed), container.MaximumSwimSpeed);
        GenerateField(nameof(container.MaximumJumpForce), container.MaximumJumpForce);
        GUI.enabled = previousGUIState;
    }
    private void VisualizeCurrentModifiers(List<DynamicModifier> modifiers)
    {
        EditorGUI.indentLevel++;
        modifiers.Sort((a,b) => a.TagetValue.CompareTo(b.TagetValue));
        foreach(DynamicModifier modifier in modifiers)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(modifier.GetType().ToString());
            EditorGUILayout.LabelField(modifier.TagetValue.ToString());
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
    }
    private void GenerateField(string name, float value){
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(name);
        EditorGUILayout.FloatField(value);
        EditorGUILayout.EndHorizontal();
    }
}