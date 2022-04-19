using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public void Awake()
    {
        Instance = this;
        SoundSettings = Resources.LoadAll<SoundSettings>("Audio");
        MixerGroups = SoundMixer.FindMatchingGroups(string.Empty);
    }

    public AudioMixer SoundMixer;
    public AnimationCurve DefaultRolloff;

    [HideInInspector] public AudioMixerGroup[] MixerGroups;
    [HideInInspector] public SoundSettings[] SoundSettings;

#if UNITY_EDITOR
    [HideInInspector] public AudioMixerGroup visibleMixerGroup;
#endif

    public static AudioSource Play(string soundID, bool destroyByTime = true)
    {
        return Instance._Play(GetSoundSettings(soundID), destroyByTime);
    }

    public static AudioSource Play(SoundSettings soundSettings, bool destroyByTime = true)
    {
        return Instance._Play(soundSettings, destroyByTime);
    }

    private AudioSource _Play(SoundSettings soundSettings, bool destroyByTime = true)
    {
        if (soundSettings == null) return null;

        AudioSource audioSource = CreateAudioSource();
        AudioClip audioClip = soundSettings.GetRandomAudioClip();

        audioSource.transform.SetParent(this.transform);
        audioSource.transform.localPosition = Vector3.zero;

        audioSource.clip = audioClip;
        float pitchOffset = Random.Range(-soundSettings.RandomPitchOffset, soundSettings.RandomPitchOffset);
        audioSource.outputAudioMixerGroup = soundSettings.MixerGroup;
        audioSource.spatialBlend = soundSettings.IsAbosoluteSound == true ? 0 : 1;
        audioSource.reverbZoneMix = soundSettings.IsAbosoluteSound == true ? 0 : 1;
        audioSource.dopplerLevel = 1f;

        audioSource.minDistance = soundSettings.MinDistance;
        audioSource.maxDistance = soundSettings.MaxDistance;
        audioSource.loop = soundSettings.IsLooped;
        audioSource.volume = soundSettings.Volume;
        audioSource.Play();

        if (destroyByTime && soundSettings.IsLooped == false)
            Destroy(audioSource.gameObject, audioClip.length * (1f + pitchOffset) + 1f);
        return audioSource;
    }

    public static AudioSource PlaySource(string soundID, AudioSource audioSource)
    {
        return Instance._PlaySource(GetSoundSettings(soundID), audioSource);
    }

    public static AudioSource PlaySource(SoundSettings soundSettings, AudioSource audioSource)
    {
        return Instance._PlaySource(soundSettings, audioSource);
    }

    private AudioSource _PlaySource(SoundSettings soundSettings, AudioSource audioSource)
    {
        if (soundSettings == null) return null;

        AudioClip audioClip = soundSettings.GetRandomAudioClip();

        audioSource.clip = audioClip;
        float pitchOffset = Random.Range(-soundSettings.RandomPitchOffset, soundSettings.RandomPitchOffset);
        audioSource.outputAudioMixerGroup = soundSettings.MixerGroup;
        audioSource.spatialBlend = soundSettings.IsAbosoluteSound == true ? 0 : 1;
        audioSource.reverbZoneMix = soundSettings.IsAbosoluteSound == true ? 0 : 1;
        audioSource.dopplerLevel = 1f;

        audioSource.minDistance = soundSettings.MinDistance;
        audioSource.maxDistance = soundSettings.MaxDistance;
        audioSource.loop = soundSettings.IsLooped;
        audioSource.volume = soundSettings.Volume;
        audioSource.Play();

        return audioSource;
    }

    public static AudioSource PlayWorldSpace(string soundID, Vector3 position, bool destroyByTime = true)
    {
        return Instance._PlayWorldSpace(GetSoundSettings(soundID), position, destroyByTime);
    }

    public static AudioSource PlayWorldSpace(SoundSettings soundSettings, Vector3 position, bool destroyByTime = true)
    {
        return Instance._PlayWorldSpace(soundSettings, position, destroyByTime);
    }

    private AudioSource _PlayWorldSpace(SoundSettings soundSettings, Vector3 position, bool destroyByTime = true)
    {
        if (soundSettings == null) return null;

        AudioSource audioSource = CreateAudioSource();
        AudioClip audioClip = soundSettings.GetRandomAudioClip();

        audioSource.transform.SetParent(this.transform);
        audioSource.transform.localPosition = Vector3.zero;

        audioSource.clip = audioClip;
        float pitchOffset = Random.Range(-soundSettings.RandomPitchOffset, soundSettings.RandomPitchOffset);
        audioSource.outputAudioMixerGroup = soundSettings.MixerGroup;
        audioSource.spatialBlend = soundSettings.IsAbosoluteSound == true ? 0 : 1;
        audioSource.reverbZoneMix = soundSettings.IsAbosoluteSound == true ? 0 : 1;
        audioSource.dopplerLevel = 1f;

        audioSource.minDistance = soundSettings.MinDistance;
        audioSource.maxDistance = soundSettings.MaxDistance;
        audioSource.loop = soundSettings.IsLooped;
        audioSource.transform.position = position;
        audioSource.volume = soundSettings.Volume;
        audioSource.Play();

        if (destroyByTime)
            Destroy(audioSource.gameObject, audioClip.length * (1f + pitchOffset) + 1f);
        return audioSource;
    }

    public static AudioSource CreateAudioSource ()
    {
        GameObject soundHandler = new GameObject("AudioSource", typeof(AudioSource));
        AudioSource audioSource = soundHandler.GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = Instance._GetMixerGroup("Master");

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        audioSource.reverbZoneMix = 0;

        audioSource.pitch = 1f;
        audioSource.volume = 0f;

        audioSource.spatialBlend = 0;
        audioSource.reverbZoneMix = 0;

        audioSource.minDistance = 3f;
        audioSource.maxDistance = 35f;

        audioSource.clip = null;
        audioSource.loop = false;

        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, Instance.DefaultRolloff);
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0;
        return audioSource;
    }

    public static void SetDefaultAudioSourceSettings (AudioSource audioSource)
    {
        audioSource.outputAudioMixerGroup = Instance._GetMixerGroup("Master");
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        audioSource.reverbZoneMix = 0;

        audioSource.pitch = 1f;
        audioSource.volume = 0f;

        audioSource.spatialBlend = 0;
        audioSource.reverbZoneMix = 0;

        audioSource.minDistance = 3f;
        audioSource.maxDistance = 35f;

        audioSource.clip = null;
        audioSource.loop = false;

        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, Instance.DefaultRolloff);
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0;
    }

    public static SoundSettings GetSoundSettings (string soundID)
    {
        return Instance._GetSoundSettings(soundID);
    }
    private SoundSettings _GetSoundSettings(string soundID)
    {
        for(int i = 0; i < SoundSettings.Length; i++)
        {
            if(SoundSettings[i].SoundID == soundID)
            {
                return SoundSettings[i];
            }
        }
        return null;
    }

    public static void SetEffectsSnapshot (string snapshotName, float time = 1f)
    {
        Instance._SetEffectsSnapshot(snapshotName, time);
    }
    private void _SetEffectsSnapshot(string snapshotName, float time = 1f)
    {
        AudioMixerSnapshot audioMixer = GetEffectsSnapshot(snapshotName);
        if(audioMixer != null) audioMixer.TransitionTo(1f);
    }
    private AudioMixerSnapshot GetEffectsSnapshot (string snapshotName)
    {
        return SoundMixer.FindSnapshot(snapshotName);
    }

    public static void SetSoundGroupVolume(string groupName, float volume)
    {
        Instance._SetSoundGroupVolume(groupName, volume);
    }
    private void _SetSoundGroupVolume(string groupName, float volume)
    {
        SoundMixer.SetFloat(groupName + "Volume", Mathf.Log(volume) * 20);
    }

    public static void SetSoundGroupPitch(string groupName, float pitch)
    {
        Instance._SetSoundGroupPitch(groupName, pitch);
    }
    private void _SetSoundGroupPitch(string groupName, float pitch)
    {
        SoundMixer.SetFloat(groupName + "Pitch", pitch);
    }

    public static float GetSoundGroupVolume(string groupName)
    {
        return Instance._GetSoundGroupVolume(groupName);
    }
    private float _GetSoundGroupVolume(string groupName)
    {
        SoundMixer.GetFloat(groupName + "Volume", out float volume);
        return volume;
    }

    public static float GetSoundGroupPitch(string groupName)
    {
        return Instance._GetSoundGroupPitch(groupName);
    }
    private float _GetSoundGroupPitch(string groupName)
    {
        SoundMixer.GetFloat(groupName + "Pitch", out float pitch);
        return pitch;
    }

    public static AudioMixerGroup GetMixerGroup (string mixerGroupName)
    {
        return Instance._GetMixerGroup(mixerGroupName);
    }
    private AudioMixerGroup _GetMixerGroup(string mixerGroupName)
    {
        for(int i = 0; i < MixerGroups.Length; i++)
        {
            if(MixerGroups[i].name == mixerGroupName)
            {
                return MixerGroups[i];
            }
        }
        return _GetMixerGroup("Master");
    }

    private void Log(string msg) { Debug.Log("[SoundManager] " + msg); }

    public enum SoundPosition
    {
        Child,
        Point,
        Line,
        BoxCollider,
        Existance,
        Camera,
        Listener,
    }
}
