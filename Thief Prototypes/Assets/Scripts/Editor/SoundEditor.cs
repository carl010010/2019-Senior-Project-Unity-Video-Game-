using UnityEngine;
using System.Collections;
using UnityEditor;
using Util;
using System;
using System.Reflection;

[CustomEditor(typeof(DoorSounds))]
public class SoundEditor : Editor
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

        DoorSounds sounds = (DoorSounds)target;

        foreach (FieldInfo sound in sounds.GetType().GetFields())
        {
            if (sound.FieldType == typeof(Sound))
            {
                Sound tempSound = sound.GetValue(sounds) as Sound;

                if (tempSound.clip != null && GUILayout.Button(tempSound.clip.name))
                {
                    _previewer.clip = tempSound.clip;
                    _previewer.volume = tempSound.volume;
                    _previewer.Play();
                }
            }
        }
    }
}
