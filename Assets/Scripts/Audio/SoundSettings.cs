using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SoundSettings", menuName = "ScriptableObjects/SoundSettings", order = 1)]
public class SoundSettings : ScriptableObject
{
    public string SoundID;
    public AudioMixerGroup MixerGroup;
    public bool IsAbosoluteSound;
    public bool IsLooped;
    public AudioClip[] AudioClips;

    public bool UseCustomCurve;
    public AnimationCurve CustomCurve;

    public bool UseRandomPitchOffset;
    public float RandomPitchOffset;
    public bool UseRandomVolumeOffset;
    public float RandomVolumeOffset;

    public float Pitch
    {
        get { return _pitch; }
        set { _pitch = Mathf.Clamp(value, 0f, 2f); }
    }
    [SerializeField]
    private float _pitch = 1f;

    public float Volume
    {
        get { return _volume; }
        set { _volume = Mathf.Clamp(value, 0f, 1f); }
    }
    [SerializeField]
    private float _volume = 1f;

    public float MinDistance
    {
        get { return _minDistance; }
        set { _minDistance = Mathf.Clamp(value, 0f, _maxDistance); }
    }
    [SerializeField]
    private float _minDistance = 3f;

    public float MaxDistance
    {
        get { return _maxDistance; }
        set { _maxDistance = Mathf.Clamp(value, _minDistance, float.MaxValue); }
    }
    [SerializeField]
    private float _maxDistance = 45f;

#if UNITY_EDITOR
    public bool IsVisibleInEditor;
#endif

    public AudioClip GetRandomAudioClip()
    {
        if (AudioClips == null || AudioClips.Length == 0) return null;
        return AudioClips[Random.Range(0, AudioClips.Length)];
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Audio/Create Sound Settings", false, 1)]
    public static void CreateSettings()
    {
        AudioClip clip = Selection.activeObject as AudioClip;
        if (clip == null) return;
        SoundSettings soundSettings = ScriptableObject.CreateInstance<SoundSettings>();

        soundSettings.SoundID = clip.name;
        soundSettings.AudioClips = new AudioClip[1] { clip };

        System.IO.DirectoryInfo dir = System.IO.Directory.GetParent(AssetDatabase.GetAssetPath(clip));
        string path = dir.ToString() + "/" + clip.name + ".asset";
        string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
        Debug.Log("Path: " + name);

        AssetDatabase.CreateAsset(soundSettings, name);
        AssetDatabase.SaveAssets();
        Selection.activeObject = soundSettings;
    }
#endif
}
