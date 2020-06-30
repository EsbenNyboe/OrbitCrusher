using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FontManager), true)]
public class FontManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if (GUILayout.Button("Apply Size Procedure (Destructive)"))
		{
			((FontManager)target).SizeProcedure();
		}

		//if (GUILayout.Button("Text Box New Width"))
		//{
		//	((FontManager)target).TextBoxNewWidth();
		//}
		//if (GUILayout.Button("Text Box New Height"))
		//{
		//	((FontManager)target).TextBoxNewHeight();
		//}
		//if (GUILayout.Button("Text Box Text New Width"))
		//{
		//	((FontManager)target).TextBoxTextNewWidth();
		//}
		//if (GUILayout.Button("Text Box Text New Pos Y"))
		//{
		//	((FontManager)target).TextBoxTextNewPosY();
		//}
		//if (GUILayout.Button("Text Box Text Font Size"))
		//{
		//	((FontManager)target).TextBoxTextNewFontSize();
		//}
		//if (GUILayout.Button("Text Box Ok Font Size"))
		//{
		//	((FontManager)target).TextBoxOkNewFontSize();
		//}
		//if (GUILayout.Button("Text Interactive UI Font Size"))
		//{
		//	((FontManager)target).TextOtherInteractiveUIFontSize();
		//}



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
