using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelMusic), true)]
public class LevelMusicEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Prepare Level Music"))
		{
			((LevelMusic)target).PrepareLevelMusic();
		}

		EditorGUI.EndDisabledGroup();
	}
}
