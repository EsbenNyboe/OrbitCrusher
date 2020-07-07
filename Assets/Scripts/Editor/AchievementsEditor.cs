using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Achievements), true)]
public class AchievementsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Apply Numbers"))
		{
			((Achievements)target).ApplyNumbersToTextboxes();
		}
		if (GUILayout.Button("Preview Achievement Look"))
		{
			((Achievements)target).ManualAchievementUpdate();
		}
		if (GUILayout.Button("Set Panel Placement"))
		{
			((Achievements)target).SetPanelPlacement();
		}

		EditorGUI.EndDisabledGroup();
	}
}
