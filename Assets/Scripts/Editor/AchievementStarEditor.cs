using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AchievementStar), true)]
public class AchievementStarEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Apply Number"))
		{
			((AchievementStar)target).ApplyNumberToTextbox(((AchievementStar)target).levelNumber);
		}
		if (GUILayout.Button("Achievement"))
		{
			((AchievementStar)target).NewAchievement();
		}


		EditorGUI.EndDisabledGroup();
	}
}
