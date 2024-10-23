using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoneVisualizer))]
public class BoneSelectorEditor : Editor
{
    private Transform selectedBone;

    void OnSceneGUI()
    {
        BoneVisualizer boneVisualizer = (BoneVisualizer)target;
        SkinnedMeshRenderer skinnedMeshRenderer = boneVisualizer.GetComponent<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer == null || skinnedMeshRenderer.bones == null)
        {
            return;
        }

        Handles.color = boneVisualizer.boneColor;

        // Iterate through each bone and make it selectable
        foreach (Transform bone in skinnedMeshRenderer.bones)
        {
            if (bone == null) continue;

            // Draw a selectable handle on each bone
            if (Handles.Button(bone.position, bone.rotation, boneVisualizer.boneSize * 2f, boneVisualizer.boneSize * 2.5f, Handles.SphereHandleCap))
            {
                selectedBone = bone;
                Selection.activeGameObject = bone.gameObject;  // Select the bone in the hierarchy
            }

            // Highlight the selected bone in a different color (yellow)
            if (selectedBone == bone)
            {
                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, bone.position, bone.rotation, boneVisualizer.boneSize * 2.5f, EventType.Repaint);
            }
        }
    }
}
