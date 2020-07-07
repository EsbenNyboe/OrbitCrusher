using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BuildPipeline), true)]
public class BuildPipelineEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Prepare for Build"))
		{
			((BuildPipeline)target).PrepareForBuild();
		}

		EditorGUI.EndDisabledGroup();
	}
}
