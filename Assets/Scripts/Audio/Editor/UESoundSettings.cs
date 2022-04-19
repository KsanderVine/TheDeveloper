using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

[CustomEditor(typeof(SoundSettings))]
public class UESoundSettings : Editor
{
    public SoundSettings soundSettings;

    private GUIStyle titleStyle;
    private GUIStyle subTitleStyle;
    private GUIStyle panelSkin;
    private GUIStyle label;
    private GUIStyle background;
    private AudioMixer soundMixer;

    public GUISkin customSkin;

    private void RedrawAll(bool forced = false)
    {
        if (forced || GUI.changed)
        {
            EditorUtility.SetDirty(target);

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }

            EditorUtility.SetDirty(target as SoundSettings);
            if (!Application.isPlaying)
            {
                var scene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }
    private void FillStyles()
    {
        if (customSkin == null) customSkin = CustomGUI.gui.customSkin;

        panelSkin = new GUIStyle("textarea");
        panelSkin.fontStyle = FontStyle.Bold;
        panelSkin.normal.textColor = Color.black + Color.white * 0.15f;
        panelSkin.fontSize = 12;
        panelSkin.alignment = TextAnchor.MiddleCenter;
        panelSkin.normal.background = EditorStyles.toolbar.normal.background;
        panelSkin.margin = new RectOffset(0, 0, 4, 0);

        background = new GUIStyle(customSkin.box);

        label = new GUIStyle("label");
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleLeft;

        titleStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        titleStyle.normal.textColor = Color.black + Color.white * 0.25f;
        titleStyle.fontSize = 11;
        titleStyle.fontStyle = FontStyle.Bold;

        subTitleStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        subTitleStyle.alignment = TextAnchor.MiddleLeft;
        subTitleStyle.normal.textColor = Color.black + Color.white * 0.25f;
        subTitleStyle.fontSize = 11;
        subTitleStyle.fontStyle = FontStyle.Bold;
    }

    public override void OnInspectorGUI()
    {
        soundSettings = target as SoundSettings;
        FillStyles();

        if (soundMixer == null)
        {
            string[] search = AssetDatabase.FindAssets("TotalMixer");
            if (search.Length > 0)
            {
                string guid = AssetDatabase.GUIDToAssetPath(search[0]);
                soundMixer = AssetDatabase.LoadAssetAtPath(guid, typeof(AudioMixer)) as AudioMixer;
            }
        }

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Sound ID: ", EditorStyles.miniLabel, GUILayout.Width(141), GUILayout.Height(18));
        soundSettings.SoundID = GUILayout.TextField(soundSettings.SoundID);
        if (soundSettings.AudioClips != null && soundSettings.AudioClips.Length > 0 && soundSettings.AudioClips[0] != null)
        {
            if (GUILayout.Button("As AudioClip", GUILayout.Height(18), GUILayout.Width(90)))
            {
                soundSettings.SoundID = soundSettings.AudioClips[0].name;
            }
        }
        GUILayout.EndHorizontal();

        if (soundMixer != null)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Mixer: ", EditorStyles.miniLabel, GUILayout.Width(141), GUILayout.Height(18));

            AudioMixerGroup[] groups = soundMixer.FindMatchingGroups(string.Empty);
            string[] optionStrings = new string[groups.Length];
            int optionID = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == soundSettings.MixerGroup) optionID = i;
                optionStrings[i] = groups[i].name;
            }
            optionID = EditorGUILayout.Popup(optionID, optionStrings);
            soundSettings.MixerGroup = groups[optionID];
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal(soundSettings.IsLooped == false ? background : EditorStyles.helpBox);
        soundSettings.IsLooped = GUILayout.Toggle(soundSettings.IsLooped, "", GUILayout.Height(18), GUILayout.Width(15));
        GUILayout.Label("Is Looped", soundSettings.IsLooped == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(soundSettings.IsAbosoluteSound == false ? background : EditorStyles.helpBox);
        soundSettings.IsAbosoluteSound = GUILayout.Toggle(soundSettings.IsAbosoluteSound, "", GUILayout.Height(18), GUILayout.Width(15));
        if (soundSettings.IsAbosoluteSound == false)
            GUILayout.Label("Spatial Blend: World (1)", EditorStyles.whiteMiniLabel, GUILayout.Height(18));
        else
            GUILayout.Label("Spatial Blend: Absolute (0)", EditorStyles.miniLabel, GUILayout.Height(18));
        GUILayout.EndHorizontal();

        if (soundSettings.AudioClips == null)
        {
            soundSettings.AudioClips = new AudioClip[] { };
        }
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Audio clips: ", EditorStyles.miniLabel);
        for (int i = 0; i < soundSettings.AudioClips.Length; i++)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(18)))
            {
                List<AudioClip> audioClips = new List<AudioClip>();
                audioClips.AddRange(soundSettings.AudioClips);
                audioClips.RemoveAt(i);
                soundSettings.AudioClips = audioClips.ToArray();
                RedrawAll();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.Space(4);
            soundSettings.AudioClips[i] = (AudioClip)EditorGUILayout.ObjectField(soundSettings.AudioClips[i], typeof(AudioClip), false);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Add new audio clip", GUILayout.Height(18)))
        {
            List<AudioClip> audioClips = new List<AudioClip>();
            audioClips.AddRange(soundSettings.AudioClips);
            audioClips.Add(null);
            soundSettings.AudioClips = audioClips.ToArray();
            return;
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Volume: ", EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(160));
        soundSettings.Volume = Mathf.FloorToInt(GUILayout.HorizontalSlider(soundSettings.Volume, 0f, 1f, GUILayout.Height(18)) * 10f) / 10f;
        GUILayout.Label(soundSettings.Volume.ToString(), EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(25));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(soundSettings.UseRandomVolumeOffset == false ? background : EditorStyles.helpBox);
        soundSettings.UseRandomVolumeOffset = GUILayout.Toggle(soundSettings.UseRandomVolumeOffset, "", GUILayout.Height(18), GUILayout.Width(15));
        if (soundSettings.UseRandomVolumeOffset == false)
            GUILayout.Label("Enable random volume", EditorStyles.whiteMiniLabel, GUILayout.Height(18), GUILayout.Width(141));
        else
            GUILayout.Label("Random volume offset:", EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(141));

        if (soundSettings.UseRandomVolumeOffset)
        {
            soundSettings.RandomVolumeOffset = Mathf.FloorToInt(GUILayout.HorizontalSlider(soundSettings.RandomVolumeOffset, 0f, 1f, GUILayout.Height(18)) * 10f) / 10f;
            GUILayout.Label(soundSettings.RandomVolumeOffset.ToString(), soundSettings.UseRandomVolumeOffset == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(25));
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Pitch: ", EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(160));
        soundSettings.Pitch = Mathf.FloorToInt(GUILayout.HorizontalSlider(soundSettings.Pitch, 0f, 2f, GUILayout.Height(18)) * 10f) / 10f;
        GUILayout.Label(soundSettings.Pitch.ToString(), EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(25));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(soundSettings.UseRandomPitchOffset == false ? background : EditorStyles.helpBox);
        soundSettings.UseRandomPitchOffset = GUILayout.Toggle(soundSettings.UseRandomPitchOffset, "", GUILayout.Height(18), GUILayout.Width(15));

        if (soundSettings.UseRandomPitchOffset == false)
            GUILayout.Label("Enable random pitch", EditorStyles.whiteMiniLabel, GUILayout.Height(18), GUILayout.Width(141));
        else
            GUILayout.Label("Random pitch offset:", EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(141));

        if (soundSettings.UseRandomPitchOffset)
        {
            soundSettings.RandomPitchOffset = Mathf.FloorToInt(GUILayout.HorizontalSlider(soundSettings.RandomPitchOffset, 0f, 1f, GUILayout.Height(18)) * 10f) / 10f;
            GUILayout.Label(soundSettings.RandomPitchOffset.ToString(), soundSettings.UseRandomPitchOffset == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(25));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Minimal distance: ", EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(160));
        soundSettings.MinDistance = Mathf.FloorToInt(GUILayout.HorizontalSlider(soundSettings.MinDistance, 0f, 100f, GUILayout.Height(18)));
        GUILayout.Label(soundSettings.MinDistance.ToString(), EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(25));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Maximum distance: ", EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(160));
        soundSettings.MaxDistance = Mathf.FloorToInt(GUILayout.HorizontalSlider(soundSettings.MaxDistance, 0f, 100f, GUILayout.Height(18)));
        GUILayout.Label(soundSettings.MaxDistance.ToString(), EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(25));
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            RedrawAll();
        }
    }
}
