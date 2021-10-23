using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelMusicChecker), true)]
public class LevelMusicCheckerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

        //if (GUILayout.Button("Check Level Music Content"))
        //{
        //    ((LevelMusicChecker)target).CheckLevelMusic();
        //}
        if (GUILayout.Button("Print Level Music Data"))
        {
            ((LevelMusicChecker)target).PrintAllLevelMusicData();
        }

        EditorGUI.EndDisabledGroup();
	}
}
