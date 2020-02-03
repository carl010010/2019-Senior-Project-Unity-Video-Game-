using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Random = UnityEngine.Random;

public class FootStepController : MonoBehaviour
{

    public enum SoundType { NONE, SOFT, MEDIUM, LOUD };

    [Serializable]
    private struct s_Sound
    {
        public string SoundTag;
        public SoundType soundType;
        public AudioClip[] m_FootstepSounds;
        [Range(0, 2)]
        public float SoundModifier;
    }


    private FirstPersonController m_FirstPersonController;
    private AudioSource m_AudioSource;

    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

    [SerializeField] private float m_SoundRaduis;
    [SerializeField] private float m_SoundLifeTime;

    [SerializeField]
    private SoundType m_soundType = SoundType.NONE;
    private s_Sound? currentSound = null;

    [SerializeField] private s_Sound[] m_sounds;

    private void Start()
    {
        m_FirstPersonController = GetComponent<FirstPersonController>();
        m_AudioSource = GetComponent<AudioSource>();

        if (m_sounds == null)
        {
            Debug.LogError("No Sound Types found");
        }
    }

    public void GetWalkingSound(RaycastHit hitInfo)
    {
        m_soundType = SoundType.NONE;
        currentSound = null;

        Collider HitCollider = hitInfo.collider;
        if (HitCollider != null)
        {
            foreach (var s in m_sounds)
            {
                if (HitCollider.CompareTag(s.SoundTag))
                {
                    m_soundType = s.soundType;
                    if (s.m_FootstepSounds.Length > 0)
                        currentSound = s;
                    break;
                }
            }
        }
    }

    public void PlayFootStepAudio(bool isPlayerGrounded, bool isPlayerStanding)
    {
        if (isPlayerGrounded)
        {
            return;
        }

        CreateSoundStim(isPlayerStanding);

        float volume = 1;

        if (m_soundType == SoundType.NONE || m_soundType == SoundType.SOFT)
        {
            volume = 0.4f;
        }
        else if (m_soundType == SoundType.MEDIUM)
        {
            volume = 0.6f;
        }
        else if (m_soundType == SoundType.LOUD)
        {
            volume = 0.8f;
        }

        if (m_FirstPersonController.m_MovingType == FirstPersonController.MovingType.SLOWWALK)
        {
            volume -= 0.2f;
        }
        else if (m_FirstPersonController.m_MovingType == FirstPersonController.MovingType.RUN)
        {
            volume += 0.2f;
        }

        if (!isPlayerStanding)
        {
            volume -= .1f;
        }

        AudioClip[] FootstepSounds = m_FootstepSounds;
        if (currentSound != null)
        {
            FootstepSounds = currentSound.Value.m_FootstepSounds;
            if (currentSound.Value.SoundModifier > 0)
            {
                volume *= currentSound.Value.SoundModifier;
            }
        }

        int n = Random.Range(1, FootstepSounds.Length);
        m_AudioSource.clip = FootstepSounds[n];
        m_AudioSource.volume = volume;
        m_AudioSource.PlayOneShot(m_AudioSource.clip);


        // move picked sound to index 0 so it's not picked next time
        FootstepSounds[n] = FootstepSounds[0];
        FootstepSounds[0] = m_AudioSource.clip;

        //m_AudioSource.volume = 1;
    }

    public void CreateSoundStim(bool isPlayerStanding = true , string soundName = null)
    {
        GameObject soundObject;
        if (soundName != null)
            soundObject = new GameObject(soundName);
        else
            soundObject = new GameObject("Player Sound");

        soundObject.transform.position = transform.position;

        SoundStim sound = soundObject.AddComponent<SoundStim>();

        float soundRaduisTypeModifier = 1;
        float soundRadiusModifier = 1;

        if (m_soundType == SoundType.MEDIUM)
        {
            soundRaduisTypeModifier *= 1.2f;
        }
        else if (m_soundType == SoundType.LOUD)
        {
            soundRaduisTypeModifier *= 2;
        }

 
        if (m_FirstPersonController.m_MovingType == FirstPersonController.MovingType.SLOWWALK)
        {
            soundRadiusModifier = 0.4f;
        }
        else if (m_FirstPersonController.m_MovingType == FirstPersonController.MovingType.RUN)
        {
            soundRadiusModifier = 1.3f;
        }

        if(!isPlayerStanding)
        {
            soundRadiusModifier *= .8f;
        }

        sound.radius = m_SoundRaduis * soundRaduisTypeModifier * soundRadiusModifier;
        sound.lifeTime = m_SoundLifeTime * soundRaduisTypeModifier * soundRadiusModifier;
    }

    public void PlayLandingSound()
    {
        CreateSoundStim(true, "LandingSounding");
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
    }


    public void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }

}
