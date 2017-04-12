using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraFollowSettings))]
class CameraFollowSettingsEditor : Editor {

    public override void OnInspectorGUI() {
        CameraFollowSettings tar = (CameraFollowSettings)target;
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("coll"), new GUIContent("Effect Collider"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("dampen"), new GUIContent("Dampen Camera Movement"));
        if (tar.dampen) EditorGUILayout.PropertyField(serializedObject.FindProperty("dampModifier"), new GUIContent("Camera Dampening Modifier"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraSizeMethod"), new GUIContent("Camera Size"));
        if (tar.cameraSizeMethod != CameraFollowSettings.CameraSizeMethod.VALUE) EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraSizeModifier"), new GUIContent("Camera Size Modifier"));
        if (tar.cameraSizeMethod == CameraFollowSettings.CameraSizeMethod.VALUE) EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraSizeValue"), new GUIContent("Camera Size Value"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraXPositionMethod"), new GUIContent("Camera X Position"));
        if (tar.cameraXPositionMethod == CameraFollowSettings.CameraPositionMethod.VALUE) EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraXPositionValue"), new GUIContent("Camera X Value"));
        if (tar.cameraXPositionMethod != CameraFollowSettings.CameraPositionMethod.VALUE) EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraXPositionOffset"), new GUIContent("Camera X Offset"));
        if (
            tar.cameraXPositionMethod == CameraFollowSettings.CameraPositionMethod.COLLIDER_MIN ||
            tar.cameraXPositionMethod == CameraFollowSettings.CameraPositionMethod.COLLIDER_MAX ||
            tar.cameraXPositionMethod == CameraFollowSettings.CameraPositionMethod.COLLIDER
        ) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraXPositionOverrideOffset"), new GUIContent("Override Camera X Offset"));
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraYPositionMethod"), new GUIContent("Camera Y Position"));
        if (tar.cameraYPositionMethod == CameraFollowSettings.CameraPositionMethod.VALUE) EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraYPositionValue"), new GUIContent("Camera Y Value"));
        if (tar.cameraYPositionMethod != CameraFollowSettings.CameraPositionMethod.VALUE) EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraYPositionOffset"), new GUIContent("Camera Y Offset"));
        if (
            tar.cameraYPositionMethod == CameraFollowSettings.CameraPositionMethod.COLLIDER_MIN ||
            tar.cameraYPositionMethod == CameraFollowSettings.CameraPositionMethod.COLLIDER_MAX ||
            tar.cameraYPositionMethod == CameraFollowSettings.CameraPositionMethod.COLLIDER
        ) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraYPositionOverrideOffset"), new GUIContent("Override Camera Y Offset"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
