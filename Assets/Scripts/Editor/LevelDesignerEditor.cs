using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelDesigner), true)]
public class LevelDesignerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Toggle SpawnZone Meshes"))
		{
			((LevelDesigner)target).ToggleSpawnZoneMeshrenderers();
		}

        if (GUILayout.Button("Copy/Paste Sound Triggers"))
        {
            ((LevelDesigner)target).CopyPasteSoundTriggers();
        }
        if (GUILayout.Button("Update Inspector Names"))
        {
            ((LevelDesigner)target).NameSoundTriggers();
        }

        EditorGUI.EndDisabledGroup();
	}
}
