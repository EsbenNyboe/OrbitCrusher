using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelLengthChecker), true)]
public class LevelLengthCheckerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Check Level Length"))
		{
			((LevelLengthChecker)target).CheckLevelLength();
		}
		



		//if (GUILayout.Button("Previous Font"))
		//{
		//	((FontManager)target).ChoosePreviousFont();
		//}
		//if (GUILayout.Button("Choose Random Font"))
		//{
		//	((FontManager)target).ChooseRandomFont();
		//}
		//if (GUILayout.Button("Apply Selected Font"))
		//{
		//	((FontManager)target).ApplySelectedFont();
		//}
		//if (GUILayout.Button("Bigger Font Size"))
		//{
		//	((FontManager)target).MakeFontSizeBigger();
		//}
		//if (GUILayout.Button("Smaller Font Size"))
		//{
		//	((FontManager)target).MakeFontSizeSmaller();
		//}


		EditorGUI.EndDisabledGroup();
	}
}
