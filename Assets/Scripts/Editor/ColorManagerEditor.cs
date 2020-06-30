using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ColorManager), true)]
public class ColorManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Apply Colors"))
		{
			((ColorManager)target).ApplyColors();
		}

		EditorGUI.EndDisabledGroup();
	}
}
