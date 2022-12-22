using UnityEngine;
using UnityEditor;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class RoutineCategoryAttribute : PropertyAttribute
{ 
    readonly string category;
    public string Category => category+"/";
    public RoutineCategoryAttribute(string Category)
    {
        this.category = Category;
    }
}