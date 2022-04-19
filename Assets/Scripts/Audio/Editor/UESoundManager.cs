using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

[CustomEditor(typeof(SoundManager))]
public class UESoundManager : Editor
{
    public SoundManager editor;

    private GUIStyle titleStyle;
    private GUIStyle subTitleStyle;
    private GUIStyle panelSkin;
    private GUIStyle label;
    private GUIStyle background;

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

            EditorUtility.SetDirty(target as SoundManager);
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
        editor = target as SoundManager;
        FillStyles();

        editor.SoundSettings = Resources.LoadAll<SoundSettings>("Audio");

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Sound Manager", titleStyle);
        DrawDefaultInspector();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Sound Settings", titleStyle);

        if (editor.SoundMixer == null)
        {
            string[] search = AssetDatabase.FindAssets("TotalMixer");
            if (search.Length > 0)
            {
                string guid = AssetDatabase.GUIDToAssetPath(search[0]);
                editor.SoundMixer = AssetDatabase.LoadAssetAtPath(guid, typeof(AudioMixer)) as AudioMixer;
            }
        }
        editor.MixerGroups = editor.SoundMixer.FindMatchingGroups(string.Empty);

        if (editor.MixerGroups.Length == 0)
        {
            EditorGUILayout.HelpBox("Setup AudioMixerGroup for AudioMixer!", MessageType.Error);
            GUILayout.EndVertical();
            return;
        }

        editor.MixerGroups = editor.SoundMixer.FindMatchingGroups(string.Empty);

        if (editor.SoundMixer != null)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Visible Mixer Group: ", EditorStyles.miniLabel, GUILayout.Width(141), GUILayout.Height(18));

            AudioMixerGroup[] groups = editor.SoundMixer.FindMatchingGroups(string.Empty);
            string[] optionStrings = new string[groups.Length];
            int optionID = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == editor.visibleMixerGroup) optionID = i;
                optionStrings[i] = groups[i].name;
            }
            optionID = EditorGUILayout.Popup(optionID, optionStrings);
            editor.visibleMixerGroup = groups[optionID];
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (editor.SoundSettings != null && editor.SoundSettings.Length > 0)
        {
            for (int i = 0; i < editor.SoundSettings.Length; i++)
            {
                if (editor.SoundSettings[i] == null) continue;
                if (editor.visibleMixerGroup.name == "Master" || editor.visibleMixerGroup == editor.SoundSettings[i].MixerGroup)
                {
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    GUILayout.Label(editor.SoundSettings[i].SoundID, EditorStyles.miniLabel, GUILayout.Width(141), GUILayout.Height(18));

                    EditorGUILayout.ObjectField(editor.SoundSettings[i], typeof(SoundSettings), false);

                    if (GUILayout.Button("Select", GUILayout.Height(18), GUILayout.Width(90)))
                    {
                        Selection.activeObject = editor.SoundSettings[i];
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        GUILayout.EndVertical();

        if (editor.SoundMixer != null)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Visible Mixer Group: ", EditorStyles.miniLabel, GUILayout.Width(141), GUILayout.Height(18));

            AudioMixerGroup[] groups = editor.SoundMixer.FindMatchingGroups(string.Empty);
            string[] optionStrings = new string[groups.Length];
            int optionID = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == editor.visibleMixerGroup) optionID = i;
                optionStrings[i] = groups[i].name;
            }
            optionID = EditorGUILayout.Popup(optionID, optionStrings);
            editor.visibleMixerGroup = groups[optionID];
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        if (GUI.changed)
        {
            RedrawAll();
        }
    }
}