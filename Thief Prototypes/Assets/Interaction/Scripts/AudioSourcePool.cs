using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Util;

public static class AudioSourcePool// : ScriptableObject
{

    static AudioSourcePool()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        AudioMixer audioMixer = Resources.Load("AudioMixer") as AudioMixer;
        if(audioMixer == null)
        {
            Debug.LogError("Could not find AudioMixer in Resources Folder");
        }
        else
        {
            audioMixerGroup = audioMixer.FindMatchingGroups("Master")[0];
            if (audioMixerGroup == null)
            {
                Debug.LogError("Could not find AudioMixerGroup 'Master' in " + audioMixer.name);
            }
        }
    }

    private static readonly AudioMixerGroup audioMixerGroup;

    private static readonly Queue<AudioSource> audioSources = new Queue<AudioSource>();
    private static readonly LinkedList<AudioSource> inuse = new LinkedList<AudioSource>();
    private static readonly Queue<LinkedListNode<AudioSource>> nodePool = new Queue<LinkedListNode<AudioSource>>();

    private static int lastCheckFrame = -1;

    public static void PlayAtPoint(Sound sound, Vector3 point, float delayInSeconds = 0)
    {
        if (sound.clip == null)
        {
            Debug.LogError("Sound clip is null");
        }

        AudioSource source;

        if (lastCheckFrame != Time.frameCount)
        {
            lastCheckFrame = Time.frameCount;
            if (CheckInUse(sound.clip) == true)
                return;
        }

        if (audioSources.Count == 0)
        {
            GameObject temp = new GameObject("AudioPoolObject");
            source = temp.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = audioMixerGroup;
        }
        else
            source = audioSources.Dequeue();

        if (nodePool.Count == 0)
            inuse.AddLast(source);
        else
        {
            LinkedListNode<AudioSource> node = nodePool.Dequeue();
            node.Value = source;
            inuse.AddLast(node);
        }

        source.transform.position = point;
        source.clip = sound.clip;
        source.volume = sound.volume;

        source.PlayDelayed(delayInSeconds);

    }

    private static bool CheckInUse(AudioClip clip)
    {
        bool ret = false;

        LinkedListNode<AudioSource> node = inuse.First;
        while (node != null)
        {
            LinkedListNode<AudioSource> current = node;
            node = node.Next;

            if (!current.Value.isPlaying)
            {
                audioSources.Enqueue(current.Value);
                inuse.Remove(current);
                nodePool.Enqueue(current);
            }
            else if (current.Value.clip == clip)
            {
                ret = true;
            }
        }
        return ret;
    }

    static void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        audioSources.Clear();
        inuse.Clear();
        nodePool.Clear();
    }
}