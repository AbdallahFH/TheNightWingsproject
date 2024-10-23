using UnityEngine;

[ExecuteInEditMode]
public class BoneVisualizer : MonoBehaviour
{
    public Color boneColor = Color.green;
    public float boneSize = 0.1f;
    
    private SkinnedMeshRenderer skinnedMeshRenderer;

    void OnDrawGizmos()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null || skinnedMeshRenderer.bones == null)
        {
            return;
        }

        Gizmos.color = boneColor;

        foreach (Transform bone in skinnedMeshRenderer.bones)
        {
            if (bone != null)
            {
                // Draw a small sphere at each bone's position
                Gizmos.DrawSphere(bone.position, boneSize);

                // Draw a line to the bone's parent (if it has one)
                if (bone.parent != null)
                {
                    Gizmos.DrawLine(bone.position, bone.parent.position);
                }
            }
        }
    }
}
