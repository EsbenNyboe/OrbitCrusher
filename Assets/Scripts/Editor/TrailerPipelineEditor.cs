using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TrailerPipeline), true)]
public class TrailerPipelineEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Apply Settings"))
		{
			((TrailerPipeline)target).ApplySettings();

		}

		EditorGUI.EndDisabledGroup();
	}
}
