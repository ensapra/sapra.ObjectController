using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

namespace sapra.ObjectController.Editor
{
public static class EditorHelper
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    public static object GetSerializedObject(this SerializedProperty property)
    {
        return property.serializedObject.GetChildObject(property.propertyPath);
    }

    private static readonly Regex matchArrayElement = new Regex(@"^data\[(\d+)\]$");
    public static object GetChildObject(this SerializedObject serializedObject, string path)
    {
        object propertyObject = serializedObject == null || serializedObject.targetObject == null ? null : serializedObject.targetObject;
		if (!string.IsNullOrEmpty(path) && propertyObject != null)
		{
			string[] splitPath = path.Split('.');
			Type fieldType = null;

			//work through the given property path, node by node
			for (int i = 0; i < splitPath.Length; i++)
			{
				string pathNode = splitPath[i];
				//both arrays and lists implement the IList interface
				if (fieldType != null && typeof(IList).IsAssignableFrom(fieldType))
				{
					//IList items are serialized like this: `Array.data[0]`
					Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal), serializedObject.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

					//just skip the `Array` part of the path
					pathNode = splitPath[++i];

					//match the `data[0]` part of the path and extract the IList item index
					Match elementMatch = matchArrayElement.Match(pathNode);
					int index;
					if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out index))
					{
						IList objectArray = (IList)propertyObject;
						bool validArrayEntry = objectArray != null && index < objectArray.Count;
						propertyObject = validArrayEntry ? objectArray[index] : null;
					}
					else
					{
						Debug.LogErrorFormat(serializedObject.targetObject, "Unexpected path format for array item: '{0}'", pathNode);
					}
					//reset fieldType, so we don't end up in the IList branch again next iteration
					fieldType = null;
				}
				else
				{
					Type instanceType = propertyObject.GetType();
					BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
					FieldInfo field;
                    do
					{
						field = instanceType.GetField(pathNode, fieldBindingFlags);

						//b/c a private, serialized field of a subclass isn't directly retrievable,
						fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
						//if neccessary, work up the inheritance chain until we find it.
						instanceType = instanceType.BaseType;
					}
                    while (field == null && instanceType != typeof(object));

					//store object info for next iteration or to return
					propertyObject = field == null || propertyObject == null ? null : field.GetValue(propertyObject);
					fieldType = field == null ? null : field.FieldType;
				}
			}
		}
        return propertyObject;
    }    
}
}

