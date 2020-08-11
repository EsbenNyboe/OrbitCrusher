using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ProgressBar), true)]
public class ProgressBarEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Placement and Size"))
		{
			((ProgressBar)target).PlacementAndSize();
		}

        EditorGUI.EndDisabledGroup();
	}
}
