using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelDataExporter), true)]
public class LevelDataExporterEditor: Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Export Data"))
		{
			((LevelDataExporter)target).ExportAllLevelData();
		}
		EditorGUI.EndDisabledGroup();
	}
}
