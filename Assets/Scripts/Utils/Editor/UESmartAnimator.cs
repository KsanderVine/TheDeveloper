using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

[CustomEditor(typeof(SmartAnimator))]
[CanEditMultipleObjects]
public class UESmartAnimator : Editor
{
    private SmartAnimator smartAnimator;
    private GUIStyle titleStyle;
    private GUIStyle subTitleStyle;
    private GUIStyle panelSkin;
    private GUIStyle label;
    private GUIStyle background;
    private GUISkin customSkin;

    private static SmartAnimator.SmartAnimation bufferAnimation;

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

            EditorUtility.SetDirty(target as SmartAnimator);
            if (!Application.isPlaying)
            {
                var scene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }

    private void FillStyles()
    {
        if (customSkin == null)
            customSkin = CustomGUI.gui.customSkin;

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
        smartAnimator = target as SmartAnimator;
        FillStyles();

        //DrawDefaultInspector();

        GUILayout.BeginVertical();

        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Main Settings", titleStyle);

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Sprite Renderer: ", EditorStyles.label, GUILayout.Width(141), GUILayout.Height(18));
        smartAnimator.spRenderer = (SpriteRenderer)EditorGUILayout.ObjectField(smartAnimator.spRenderer, typeof(SpriteRenderer), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Image Renderer: ", EditorStyles.label, GUILayout.Width(141), GUILayout.Height(18));
        smartAnimator.imageRenderer = (Image)EditorGUILayout.ObjectField(smartAnimator.imageRenderer, typeof(Image), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Play Speed: ", EditorStyles.miniLabel, GUILayout.Width(141f), GUILayout.Height(18));
        smartAnimator.speed = Mathf.FloorToInt(GUILayout.HorizontalSlider(smartAnimator.speed, 0f, 10f, GUILayout.Height(18)) * 100f) / 100f;
        GUILayout.Label(smartAnimator.speed.ToString(), EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(25));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(smartAnimator.isExitAnimationBlocked == false ? background : EditorStyles.helpBox);
        smartAnimator.isExitAnimationBlocked = GUILayout.Toggle(smartAnimator.isExitAnimationBlocked, "", GUILayout.Height(18), GUILayout.Width(15));
        GUILayout.Label("Exit animations block", smartAnimator.isExitAnimationBlocked == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(smartAnimator.unscaledTimeUpdate == false ? background : EditorStyles.helpBox);
        smartAnimator.unscaledTimeUpdate = GUILayout.Toggle(smartAnimator.unscaledTimeUpdate, "", GUILayout.Height(18), GUILayout.Width(15));
        GUILayout.Label("Unscaled Time Update", smartAnimator.unscaledTimeUpdate == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(smartAnimator.destroyAfterPlay == false ? background : EditorStyles.helpBox);
        smartAnimator.destroyAfterPlay = GUILayout.Toggle(smartAnimator.destroyAfterPlay, "", GUILayout.Height(18), GUILayout.Width(15));
        GUILayout.Label("Destroy After Play", smartAnimator.destroyAfterPlay == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(smartAnimator.destroyParent == false ? background : EditorStyles.helpBox);
        smartAnimator.destroyParent = GUILayout.Toggle(smartAnimator.destroyParent, "", GUILayout.Height(18), GUILayout.Width(15));
        GUILayout.Label("Destroy Parent", smartAnimator.destroyParent == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18));
        GUILayout.EndHorizontal();

        GUILayout.Label("Animations", titleStyle);

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add new animation", GUILayout.Height(25)))
        {
            List<SmartAnimator.SmartAnimation> animations = new List<SmartAnimator.SmartAnimation>();
            animations.AddRange(smartAnimator.animations);

            SmartAnimator.SmartAnimation animation = new SmartAnimator.SmartAnimation();
            animation.editorTitle = "New Animation";
            animation.sprites = new List<Sprite>();

            animations.Add(animation);
            smartAnimator.animations = animations.ToArray();
            RedrawAll(true);
            return;
        }
        if (bufferAnimation != null)
        {
            if (GUILayout.Button("Paste ''"+bufferAnimation.editorTitle+"''", GUILayout.Height(25)))
            {
                List<SmartAnimator.SmartAnimation> animations = new List<SmartAnimator.SmartAnimation>();
                animations.AddRange(smartAnimator.animations);

                SmartAnimator.SmartAnimation animation = new SmartAnimator.SmartAnimation();

                animation.editorTitle = bufferAnimation.editorTitle;
                animation.frameTime = bufferAnimation.frameTime;
                animation.circled = bufferAnimation.circled;
                animation.exitAnimation = bufferAnimation.exitAnimation;
                animation.exitAnimationID = bufferAnimation.exitAnimationID;

                List<Sprite> sprites = new List<Sprite>();
                sprites.AddRange(bufferAnimation.sprites);
                animation.sprites = sprites;

                animations.Add(animation);
                smartAnimator.animations = animations.ToArray();
                RedrawAll(true);
                return;
            }
        }
        if (GUILayout.Button("Hide all", GUILayout.Height(25), GUILayout.Width(90)))
        {
            for (int i = 0; i < smartAnimator.animations.Length; i++) smartAnimator.animations[i].isVisible = false;
        }
        if (GUILayout.Button("Show all", GUILayout.Height(25), GUILayout.Width(90)))
        {
            for (int i = 0; i < smartAnimator.animations.Length; i++) smartAnimator.animations[i].isVisible = true;
        }
        GUILayout.EndHorizontal();

        if (smartAnimator.animations == null || smartAnimator.animations.Length == 0)
        {
            EditorGUILayout.HelpBox("List of animations is empty", MessageType.Info);
        }

        if (smartAnimator.animations == null)
        {
            smartAnimator.animations = new SmartAnimator.SmartAnimation[] { };
            RedrawAll(true);
            return;
        }

        if(smartAnimator.animations.Length == 0)
        {
            EditorGUILayout.HelpBox("There is no animations yet!", MessageType.Info);
        }

        for(int i = 0; i < smartAnimator.animations.Length; i++)
        {
            if(i > 0) GUILayout.Space(5);

            GUILayout.BeginVertical(smartAnimator.animations[i].isVisible ? EditorStyles.helpBox : GUI.skin.box);
            GUILayout.BeginHorizontal();
            smartAnimator.animations[i].isVisible = GUILayout.Toggle(smartAnimator.animations[i].isVisible, (smartAnimator.animations[i].isVisible ? "▼" : "►"), label, GUILayout.Height(18), GUILayout.Width(15));
            GUILayout.Label("[" + i + "] " + smartAnimator.animations[i].editorTitle, subTitleStyle);
            GUILayout.Label("[" + smartAnimator.animations[i].sprites.Count * smartAnimator.animations[i].frameTime + " s.]", subTitleStyle, GUILayout.Width(65));

            if (i > 0)
            {
                if (GUILayout.Button("▲", GUILayout.Width(24), GUILayout.Height(18)))
                {
                    SmartAnimator.SmartAnimation tempAnimation = smartAnimator.animations[i - 1];
                    smartAnimator.animations[i - 1] = smartAnimator.animations[i];
                    smartAnimator.animations[i] = tempAnimation;
                    RedrawAll(true);
                    return;
                }
            }
            else GUILayout.Space(28);

            if (i < smartAnimator.animations.Length - 1)
            {
                if (GUILayout.Button("▼", GUILayout.Width(24), GUILayout.Height(18)))
                {
                    SmartAnimator.SmartAnimation tempAnimation = smartAnimator.animations[i + 1];
                    smartAnimator.animations[i + 1] = smartAnimator.animations[i];
                    smartAnimator.animations[i] = tempAnimation;
                    RedrawAll(true);
                    return;
                }
            }
            else GUILayout.Space(28);

            if (bufferAnimation != null)
            {
                if (GUILayout.Button("Paste", GUILayout.Width(50), GUILayout.Height(18)))
                {
                    if (EditorUtility.DisplayDialog("Smart Animation", "Override ''" + smartAnimator.animations[i].editorTitle + "'' animation with buffered animation ''" + bufferAnimation.editorTitle + "''?", "Yes", "No"))
                    {
                        smartAnimator.animations[i].editorTitle = bufferAnimation.editorTitle;
                        smartAnimator.animations[i].frameTime = bufferAnimation.frameTime;
                        smartAnimator.animations[i].circled = bufferAnimation.circled;
                        smartAnimator.animations[i].exitAnimation = bufferAnimation.exitAnimation;
                        smartAnimator.animations[i].exitAnimationID = bufferAnimation.exitAnimationID;

                        List<Sprite> sprites = new List<Sprite>();
                        sprites.AddRange(bufferAnimation.sprites);
                        smartAnimator.animations[i].sprites = sprites;
                        RedrawAll(true);
                        return;
                    }
                }
            }

            if (GUILayout.Button("Copy", GUILayout.Width(50), GUILayout.Height(18)))
            {
                bufferAnimation = smartAnimator.animations[i];
                return;
            }

            if (GUILayout.Button("Remove", GUILayout.Width(65), GUILayout.Height(18)))
            {
                if (EditorUtility.DisplayDialog("Smart Animation", "Remove ''" + smartAnimator.animations[i].editorTitle + "'' animation?", "Yes", "No"))
                {
                    List<SmartAnimator.SmartAnimation> animations = new List<SmartAnimator.SmartAnimation>();
                    animations.AddRange(smartAnimator.animations);
                    animations.RemoveAt(i);
                    smartAnimator.animations = animations.ToArray();
                    RedrawAll(true);
                    return;
                }
            }
            GUILayout.EndHorizontal();

            if (smartAnimator.animations[i].isVisible)
            {
                DrawAnimationBlank(smartAnimator.animations[i]);
                GUILayout.Space(2);
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Space(2);
                GUILayout.EndVertical();
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndVertical();
        GUILayout.EndVertical();

        if (GUI.changed)
        {
            RedrawAll();
        }
    }

    private void DrawAnimationBlank(SmartAnimator.SmartAnimation animation)
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Editor Title", EditorStyles.label, GUILayout.Width(141), GUILayout.Height(18));
        animation.editorTitle = GUILayout.TextField(animation.editorTitle);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("Frame Time", EditorStyles.label, GUILayout.Width(141), GUILayout.Height(18));
        animation.frameTime = EditorGUILayout.FloatField(animation.frameTime);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(animation.circled == false ? background : EditorStyles.helpBox);
        animation.circled = GUILayout.Toggle(animation.circled, "", GUILayout.Height(18), GUILayout.Width(15));
        GUILayout.Label("Is Circles", animation.circled == false ? EditorStyles.whiteMiniLabel : EditorStyles.miniLabel, GUILayout.Height(18));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(animation.exitAnimation == false ? background : EditorStyles.helpBox);
        animation.exitAnimation = GUILayout.Toggle(animation.exitAnimation, "", GUILayout.Height(18), GUILayout.Width(15));

        if (animation.exitAnimation == false)
            GUILayout.Label("Enable Exit Animation", EditorStyles.whiteMiniLabel, GUILayout.Height(18), GUILayout.Width(141));
        else
        {
            GUILayout.Label("Exit Animation ID:", EditorStyles.miniLabel, GUILayout.Height(18), GUILayout.Width(141));
            List<string> names = new List<string>();
            for (int i = 0; i < smartAnimator.animations.Length; i++) names.Add(smartAnimator.animations[i].editorTitle);
            animation.exitAnimationID = EditorGUILayout.Popup(animation.exitAnimationID, names.ToArray());
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        if(animation.sprites == null)
        {
            animation.sprites = new List<Sprite>();
            RedrawAll(true);
            return;
        }
        GUILayout.Label("Animation Sprites", EditorStyles.label, GUILayout.Height(18));
        if (animation.sprites.Count == 0)
        {
            EditorGUILayout.HelpBox("There is no animation sprites yet!", MessageType.Info);
        }
        for (int i = 0; i < animation.sprites.Count; i++)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-", GUILayout.Width(24), GUILayout.Height(18)))
            {
                List<Sprite> sprites = new List<Sprite>();
                sprites.AddRange(animation.sprites);
                sprites.RemoveAt(i);
                animation.sprites = sprites;
                RedrawAll(true);
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.Space(4);
            animation.sprites[i] = (Sprite)EditorGUILayout.ObjectField(animation.sprites[i], typeof(Sprite), false);
            GUILayout.EndVertical();

            if (i > 0)
            {
                if (GUILayout.Button("▲", GUILayout.Width(24), GUILayout.Height(18)))
                {
                    Sprite sprite = animation.sprites[i - 1];
                    animation.sprites[i - 1] = animation.sprites[i];
                    animation.sprites[i] = sprite;
                    RedrawAll(true);
                    return;
                }
            }
            else GUILayout.Space(28);

            if (i < animation.sprites.Count - 1)
            {
                if (GUILayout.Button("▼", GUILayout.Width(24), GUILayout.Height(18)))
                {
                    Sprite sprite = animation.sprites[i + 1];
                    animation.sprites[i + 1] = animation.sprites[i];
                    animation.sprites[i] = sprite;
                    RedrawAll(true);
                    return;
                }
            }
            else GUILayout.Space(28);

            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Add new sprite", GUILayout.Height(18)))
        {
            List<Sprite> sprites = new List<Sprite>();
            sprites.AddRange(animation.sprites);
            sprites.Add(null);
            animation.sprites = sprites;
            return;
        }
        GUILayout.EndVertical();
    }
}
