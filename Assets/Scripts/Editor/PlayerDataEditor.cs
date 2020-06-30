using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Player), true)]
public class PlayerDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

        if (GUILayout.Button("Set Save Point (manually)"))
        {
            ((Player)target).SavePlayer();
        }

        EditorGUI.EndDisabledGroup();
	}
}
