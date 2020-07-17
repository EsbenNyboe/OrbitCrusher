using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AchievementParticleEffects), true)]
public class AchievementParticleEffectsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Apply Spacing"))
		{
			((AchievementParticleEffects)target).ApplySpacing();
		}

		EditorGUI.EndDisabledGroup();
	}
}
