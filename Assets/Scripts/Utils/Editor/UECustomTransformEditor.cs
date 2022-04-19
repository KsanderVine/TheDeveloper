using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class UICustomTransformEditor : Editor
{
    BakedTransform bakedTransform;
    private void RedrawAll()
    {
        if (GUI.changed)
        {
            if (bakedTransform != null) EditorUtility.SetDirty(bakedTransform);
            EditorUtility.SetDirty(target as Transform);
            if (!Application.isPlaying)
            {
                var scene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        Transform transform = target as Transform;
        bakedTransform = transform.GetComponent<BakedTransform>();
        DrawABetterInspector(transform, bakedTransform);

        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (bakedTransform == null)
        {
            if (GUILayout.Button("Bake transform"))
            {
                bakedTransform = transform.gameObject.AddComponent<BakedTransform>();

                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(bakedTransform);

                Vector2 offset = new Vector2(0,0);

                offset.x = Mathf.Clamp(transform.position.x - Mathf.FloorToInt(transform.position.x), 0, 1);
                offset.y = Mathf.Clamp(transform.position.y - Mathf.FloorToInt(transform.position.y), 0, 1);

                bakedTransform.offset = offset;
                bakedTransform.autoFit = true;
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(bakedTransform.autoFit == true ? "Disable auto-fit" : "Enable auto-fit"))
            {
                bakedTransform.autoFit = !bakedTransform.autoFit;
            }
            if (GUILayout.Button("Rebake position"))
            {
                Vector2 offset = new Vector2(0, 0);

                offset.x = Mathf.Clamp(transform.position.x - Mathf.FloorToInt(transform.position.x), 0, 1);
                offset.y = Mathf.Clamp(transform.position.y - Mathf.FloorToInt(transform.position.y), 0, 1);

                bakedTransform.offset = offset;
            }
            if (GUILayout.Button("Remove bake"))
            {
                EditorUtility.SetDirty(bakedTransform);
                DestroyImmediate(bakedTransform);
                EditorUtility.SetDirty(target);
                EditorGUIUtility.ExitGUI();
                return;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        //if (Selection.activeGameObject.scene != EditorSceneManager.GetActiveScene()) return;
        RedrawAll();
    }

    public void OnSceneGUI()
    {
        Transform transform = target as Transform;
        BakedTransform bakedTransform = transform.GetComponent<BakedTransform>();
        if (bakedTransform == null || bakedTransform.autoFit == false || Application.isPlaying) return;

        Vector2 position = transform.position;
        Vector2 offset = bakedTransform.offset;

        position.x = Mathf.FloorToInt(position.x);
        position.y = Mathf.FloorToInt(position.y);

        transform.position = (position + offset);
    }

    public void DrawABetterInspector(Transform t, BakedTransform bakedTransform)
    {
        GUILayout.BeginVertical();
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Local Space", EditorStyles.boldLabel, GUILayout.Height(18));
        if (GUILayout.Button("Center", GUILayout.Width(55), GUILayout.Height(18)))
        {
            Camera camera = SceneView.lastActiveSceneView.camera;
            Vector3 cameraPosition = camera.transform.position;
            cameraPosition.z = t.position.z;
            t.position = FixIfNaN(cameraPosition);
        }
        if (GUILayout.Button("Reset", GUILayout.Width(55), GUILayout.Height(18))) t.localPosition = FixIfNaN(Vector3.zero);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Position", EditorStyles.label, GUILayout.Width(160), GUILayout.Height(18));
        Vector3 localPosition = EditorGUILayout.Vector3Field("", t.localPosition);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotation", EditorStyles.label, GUILayout.Width(160), GUILayout.Height(18));
        Vector3 localEulerAngles = EditorGUILayout.Vector3Field("", t.localEulerAngles);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Local scale", EditorStyles.label, GUILayout.Width(160), GUILayout.Height(18));
        Vector3 localScale = EditorGUILayout.Vector3Field("", t.localScale);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        GUILayout.Label("World Space", EditorStyles.boldLabel, GUILayout.Height(18));
        if (GUILayout.Button("Reset", GUILayout.Width(55), GUILayout.Height(18))) t.position = FixIfNaN(Vector3.zero);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Position", EditorStyles.label, GUILayout.Width(160), GUILayout.Height(18));
        Vector3 position = EditorGUILayout.Vector3Field("", t.position);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotation", EditorStyles.label, GUILayout.Width(160), GUILayout.Height(18));
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("", t.eulerAngles);
        GUILayout.EndHorizontal();

        if(bakedTransform != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Baked offset", EditorStyles.label, GUILayout.Width(160), GUILayout.Height(18));
            EditorGUILayout.Vector3Field("", bakedTransform.offset);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndVertical();

        if (GUI.changed)
        {
            Undo.RecordObject(t, "Transform Change");

            if (t.localPosition != localPosition)
            {
                t.localPosition = FixIfNaN(localPosition);
                return;
            }
            if (t.localEulerAngles != localEulerAngles)
            {
                t.localEulerAngles = FixIfNaN(localEulerAngles);
                return;
            }
            if (t.localScale != localScale)
            {
                t.localScale = FixIfNaN(localScale);
                return;
            }

            if (t.position != position)
            {
                t.position = FixIfNaN(position);
                return;
            }
            if (t.eulerAngles != eulerAngles)
            {
                t.eulerAngles = FixIfNaN(eulerAngles);
                return;
            }
        }
    }

    private Vector3 FixIfNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
        {
            v.x = 0.0f;
        }
        if (float.IsNaN(v.y))
        {
            v.y = 0.0f;
        }
        if (float.IsNaN(v.z))
        {
            v.z = 0.0f;
        }
        return v;
    }
}