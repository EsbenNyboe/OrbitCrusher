using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AudioObject), true)]
public class AudioObjectEditor : Editor
{

	[SerializeField] private AudioSource _previewer;

	public void OnEnable()
	{
		_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	public void OnDisable()
	{
		DestroyImmediate(_previewer.gameObject);
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Preview (editor)"))
		{
			((AudioObject)target).PreviewAudioEvent(_previewer);
		}
		if (GUILayout.Button("Trigger (in-game)"))
		{
			((AudioObject)target).TriggerAudioObject();
		}
		if (GUILayout.Button("Stop Audio Event"))
		{
			((AudioObject)target).StopAudioAllVoices();
		}
		if (GUILayout.Button("Fade In"))
		{
			((AudioObject)target).VolumeChangeInParent(((AudioObject)target).initialVolume, ((AudioObject)target).fadeTimeTestValue, false);
		}
		if (GUILayout.Button("Fade Out"))
		{
			((AudioObject)target).VolumeChangeInParent(0, ((AudioObject)target).fadeTimeTestValue, false);
		}
		EditorGUI.EndDisabledGroup();
	}

}
