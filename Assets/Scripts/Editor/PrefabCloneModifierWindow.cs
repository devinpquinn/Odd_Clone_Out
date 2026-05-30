using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class PrefabCloneModifierWindow : EditorWindow
{
    private const string BaseLayerStateName = "Idle_A";
    private const string ShapeKeyLayerStateName = "Eyes_Blink";

    private GameObject sourcePrefab;
    private DefaultAsset destinationFolder;

    [MenuItem("Tools/Prefab Clone Modifier")]
    public static void ShowWindow()
    {
        GetWindow<PrefabCloneModifierWindow>("Prefab Clone Modifier");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Clone Prefab And Apply Modifications", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sourcePrefab = (GameObject)EditorGUILayout.ObjectField("Source Prefab", sourcePrefab, typeof(GameObject), false);
        destinationFolder = (DefaultAsset)EditorGUILayout.ObjectField("Destination Folder", destinationFolder, typeof(DefaultAsset), false);

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(!CanRun()))
        {
            if (GUILayout.Button("Clone And Modify"))
            {
                CloneAndModify();
            }
        }
    }

    private bool CanRun()
    {
        if (sourcePrefab == null || destinationFolder == null)
        {
            return false;
        }

        string prefabPath = AssetDatabase.GetAssetPath(sourcePrefab);
        if (string.IsNullOrEmpty(prefabPath) || PrefabUtility.GetPrefabAssetType(sourcePrefab) == PrefabAssetType.NotAPrefab)
        {
            return false;
        }

        string folderPath = AssetDatabase.GetAssetPath(destinationFolder);
        return AssetDatabase.IsValidFolder(folderPath);
    }

    private void CloneAndModify()
    {
        string sourcePath = AssetDatabase.GetAssetPath(sourcePrefab);
        string folderPath = AssetDatabase.GetAssetPath(destinationFolder);

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("Invalid Folder", "Please select a valid destination folder inside Assets.", "OK");
            return;
        }

        string newPrefabPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folderPath, sourcePrefab.name + ".prefab").Replace("\\", "/"));

        if (!AssetDatabase.CopyAsset(sourcePath, newPrefabPath))
        {
            EditorUtility.DisplayDialog("Copy Failed", "Could not copy source prefab.", "OK");
            return;
        }

        GameObject prefabRoot = null;

        try
        {
            prefabRoot = PrefabUtility.LoadPrefabContents(newPrefabPath);

            RemoveRootComponents(prefabRoot);
            Animator animator = ConfigureAnimator(prefabRoot, folderPath);
            AddStaggerComponents(prefabRoot);
            DeleteLodMeshes(prefabRoot);
            ConfigureMeshLod0(prefabRoot);

            if (animator != null)
            {
                EditorUtility.SetDirty(animator);
            }

            EditorUtility.SetDirty(prefabRoot);
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, newPrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", "Created modified prefab at:\n" + newPrefabPath, "OK");
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Prefab modification failed: " + ex);
            EditorUtility.DisplayDialog("Error", "Failed to modify prefab. See console for details.", "OK");
        }
        finally
        {
            if (prefabRoot != null)
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }
    }

    private static void RemoveRootComponents(GameObject root)
    {
        LODGroup lodGroup = root.GetComponent<LODGroup>();
        if (lodGroup != null)
        {
            DestroyImmediate(lodGroup);
        }

        CapsuleCollider capsuleCollider = root.GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            DestroyImmediate(capsuleCollider);
        }
    }

    private static Animator ConfigureAnimator(GameObject root, string outputFolder)
    {
        Animator animator = root.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No Animator found on prefab root.");
            return null;
        }

        AnimatorController sourceController = animator.runtimeAnimatorController as AnimatorController;
        if (sourceController == null)
        {
            Debug.LogWarning("Animator does not use an AnimatorController.");
            return animator;
        }

        string sourceControllerPath = AssetDatabase.GetAssetPath(sourceController);
        string copiedControllerPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(outputFolder, sourceController.name + "_Modified.controller").Replace("\\", "/"));

        if (AssetDatabase.CopyAsset(sourceControllerPath, copiedControllerPath))
        {
            AnimatorController copiedController = AssetDatabase.LoadAssetAtPath<AnimatorController>(copiedControllerPath);
            animator.runtimeAnimatorController = copiedController;

            SetLayerDefaultState(copiedController, 0, "Base Layer", BaseLayerStateName);
            SetLayerDefaultState(copiedController, 1, "Shapekey", ShapeKeyLayerStateName);

            EditorUtility.SetDirty(copiedController);
        }
        else
        {
            Debug.LogWarning("Could not copy AnimatorController. Modifying original reference instead.");
            SetLayerDefaultState(sourceController, 0, "Base Layer", BaseLayerStateName);
            SetLayerDefaultState(sourceController, 1, "Shapekey", ShapeKeyLayerStateName);
            EditorUtility.SetDirty(sourceController);
        }

        return animator;
    }

    private static void SetLayerDefaultState(AnimatorController controller, int layerIndex, string expectedLayerName, string clipName)
    {
        if (controller == null)
        {
            return;
        }

        if (layerIndex < 0 || layerIndex >= controller.layers.Length)
        {
            Debug.LogWarning($"AnimatorController '{controller.name}' is missing layer index {layerIndex}.");
            return;
        }

        AnimatorControllerLayer layer = controller.layers[layerIndex];
        if (!string.Equals(layer.name, expectedLayerName))
        {
            Debug.LogWarning($"Layer {layerIndex} is named '{layer.name}', expected '{expectedLayerName}'. Continuing by index.");
        }

        AnimatorState state = FindStateByClipOrName(layer.stateMachine, clipName);
        if (state == null)
        {
            Debug.LogWarning($"Could not find state or clip named '{clipName}' in layer '{layer.name}'.");
            return;
        }

        layer.stateMachine.defaultState = state;
    }

    private static AnimatorState FindStateByClipOrName(AnimatorStateMachine stateMachine, string clipName)
    {
        foreach (ChildAnimatorState childState in stateMachine.states)
        {
            AnimatorState state = childState.state;
            if (state == null)
            {
                continue;
            }

            if (state.name == clipName)
            {
                return state;
            }

            if (state.motion != null && state.motion.name == clipName)
            {
                return state;
            }
        }

        foreach (ChildAnimatorStateMachine childStateMachine in stateMachine.stateMachines)
        {
            AnimatorState found = FindStateByClipOrName(childStateMachine.stateMachine, clipName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static void AddStaggerComponents(GameObject root)
    {
        StaggerAnimation baseLayerStagger = root.AddComponent<StaggerAnimation>();
        baseLayerStagger.clipName = BaseLayerStateName;
        baseLayerStagger.layer = 0;

        StaggerAnimation shapeKeyLayerStagger = root.AddComponent<StaggerAnimation>();
        shapeKeyLayerStagger.clipName = ShapeKeyLayerStateName;
        shapeKeyLayerStagger.layer = 1;
    }

    private static void DeleteLodMeshes(GameObject root)
    {
        string[] namesToDelete = { "Mesh_LOD1", "Mesh_LOD2", "Mesh_LOD3" };

        foreach (string objectName in namesToDelete)
        {
            Transform target = FindChildByName(root.transform, objectName);
            if (target != null)
            {
                DestroyImmediate(target.gameObject);
            }
        }
    }

    private static void ConfigureMeshLod0(GameObject root)
    {
        Transform meshLod0 = FindChildByName(root.transform, "Mesh_LOD0");
        if (meshLod0 == null)
        {
            Debug.LogWarning("Could not find Mesh_LOD0 in prefab hierarchy.");
            return;
        }

        BoxCollider boxCollider = meshLod0.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = meshLod0.gameObject.AddComponent<BoxCollider>();
        }

        boxCollider.isTrigger = true;

        int creatureLayer = LayerMask.NameToLayer("Creature");
        if (creatureLayer == -1)
        {
            Debug.LogWarning("Layer 'Creature' does not exist. Mesh_LOD0 layer was not changed.");
            return;
        }

        SetLayerRecursively(meshLod0.gameObject, creatureLayer);
    }

    private static void SetLayerRecursively(GameObject root, int layer)
    {
        root.layer = layer;

        foreach (Transform child in root.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private static Transform FindChildByName(Transform root, string targetName)
    {
        var stack = new Stack<Transform>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            Transform current = stack.Pop();
            if (current.name == targetName)
            {
                return current;
            }

            for (int i = 0; i < current.childCount; i++)
            {
                stack.Push(current.GetChild(i));
            }
        }

        return null;
    }
}
