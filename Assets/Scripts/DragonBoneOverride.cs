using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class DragonBoneOverride : MonoBehaviour
{
    public Rigidbody dragonRigidbody;        // Dragon's Rigidbody
    public Animator dragonAnimator;          // Dragon's Animator
    public Avatar customAvatar;              // Custom avatar to get bones from
    public float turnInfluence = 0.5f;       // Influence of turning on bone rotation
    public float leanInfluence = 0.5f;       // Influence of leaning based on forward velocity

    // A list of bones to override, dynamically filled from the avatar
    public List<Transform> bonesToAffect = new List<Transform>();
    public List<bool> boneAffectedFlags = new List<bool>();

    // Store original bone rotations to preserve natural animation
    private Dictionary<Transform, Quaternion> originalBoneRotations = new Dictionary<Transform, Quaternion>();

    private void Start()
    {
        if (dragonRigidbody == null)
            dragonRigidbody = GetComponent<Rigidbody>();
        if (dragonAnimator == null)
            dragonAnimator = GetComponent<Animator>();

        // Load bones from the avatar and initialize the affected flags
        LoadBonesFromAvatar();

        // Store the original rotations of the bones
        foreach (Transform bone in bonesToAffect)
        {
            if (bone != null)
            {
                originalBoneRotations[bone] = bone.localRotation;
            }
        }
    }

    private void LoadBonesFromAvatar()
    {
        if (customAvatar == null || dragonAnimator == null)
        {
            Debug.LogError("No custom avatar or animator assigned!");
            return;
        }

        bonesToAffect.Clear();
        boneAffectedFlags.Clear();

        // Get all the bone transforms from the avatar
        for (int i = 0; i < (int)HumanBodyBones.LastBone; i++)
        {
            Transform boneTransform = dragonAnimator.GetBoneTransform((HumanBodyBones)i);
            if (boneTransform != null)
            {
                bonesToAffect.Add(boneTransform);
                boneAffectedFlags.Add(false); // Default all bones to unaffected
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 velocity = dragonRigidbody.linearVelocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);  // Velocity in local space

        // Determine how much influence the velocity has on bone adjustments
        float turnAdjustment = localVelocity.x * turnInfluence;
        float leanAdjustment = localVelocity.z * leanInfluence;

        // Apply adjustments to each bone based on velocity and selection
        for (int i = 0; i < bonesToAffect.Count; i++)
        {
            if (boneAffectedFlags[i])  // Check if this bone is selected to be affected
            {
                Transform bone = bonesToAffect[i];

                if (bone != null)
                {
                    // Restore original rotation to avoid cumulative adjustments
                    bone.localRotation = originalBoneRotations[bone];

                    // Apply new rotations based on velocity
                    Quaternion adjustedRotation = Quaternion.Euler(leanAdjustment, turnAdjustment, 0f);
                    bone.localRotation *= adjustedRotation;
                }
            }
        }
    }

    // Optional: Draw Gizmos in the scene view to visualize the bones affected
    private void OnDrawGizmos()
    {
        if (bonesToAffect != null)
        {
            foreach (Transform bone in bonesToAffect)
            {
                if (bone != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(bone.position, 0.05f);
                }
            }
        }
    }

    // Custom inspector button to load bones from the avatar manually
    [CustomEditor(typeof(DragonBoneOverride))]
    public class DragonBoneOverrideEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DragonBoneOverride script = (DragonBoneOverride)target;

            if (GUILayout.Button("Load Bones From Avatar"))
            {
                script.LoadBonesFromAvatar();
            }

            if (script.bonesToAffect != null && script.bonesToAffect.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Bone Affected Flags:", EditorStyles.boldLabel);

                // Display each bone and its corresponding toggle flag
                for (int i = 0; i < script.bonesToAffect.Count; i++)
                {
                    if (script.bonesToAffect[i] != null)
                    {
                        script.boneAffectedFlags[i] = EditorGUILayout.Toggle(script.bonesToAffect[i].name, script.boneAffectedFlags[i]);
                    }
                }
            }
        }
    }
}
