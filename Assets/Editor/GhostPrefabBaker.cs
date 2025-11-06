// Assets/Editor/GhostPrefabBaker.cs
using UnityEngine;
using UnityEditor;
using System.IO;

public class GhostPrefabBaker : MonoBehaviour
{
    [MenuItem("Tools/Ghost/Create Ghost Prefab from Selection")]
    static void CreateGhostFromSelection()
    {
        var go = Selection.activeGameObject;
        if (!go)
        {
            EditorUtility.DisplayDialog("Ghost Baker", "Select a GameObject first.", "OK");
            return;
        }

        var skinned = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (!skinned)
        {
            EditorUtility.DisplayDialog("Ghost Baker",
                "No SkinnedMeshRenderer found under selection.\nIf your model is not skinned, just duplicate and assign transparent material.",
                "OK");
            return;
        }

        // Bake mesh
        var bakedMesh = new Mesh();
        skinned.BakeMesh(bakedMesh, true);

        // Save mesh asset
        var origPath = "Assets/GhostMeshes";
        if (!AssetDatabase.IsValidFolder(origPath))
            AssetDatabase.CreateFolder("Assets", "GhostMeshes");

        string meshName = go.name + "_GhostMesh.asset";
        string meshPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(origPath, meshName));
        AssetDatabase.CreateAsset(bakedMesh, meshPath);
        AssetDatabase.SaveAssets();

        // Build ghost GO (static mesh)
        var ghostGO = new GameObject(go.name + "_Ghost");
        ghostGO.transform.position = skinned.transform.position;
        ghostGO.transform.rotation = skinned.transform.rotation;
        ghostGO.transform.localScale = skinned.transform.lossyScale; // keep world scale

        var mf = ghostGO.AddComponent<MeshFilter>();
        mf.sharedMesh = bakedMesh;

        var mr = ghostGO.AddComponent<MeshRenderer>();
        // user will assign material manually (safer), but if you want you can auto-assign:
        // mr.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MAT_GhostTransparent.mat");

        // Remove any collider/RB by default (ghost visual only)
        // (none added here)

        // Save as prefab
        var prefabPathFolder = "Assets/GhostPrefabs";
        if (!AssetDatabase.IsValidFolder(prefabPathFolder))
            AssetDatabase.CreateFolder("Assets", "GhostPrefabs");

        string prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(prefabPathFolder, ghostGO.name + ".prefab"));
        var prefab = PrefabUtility.SaveAsPrefabAsset(ghostGO, prefabPath);

        Object.DestroyImmediate(ghostGO);

        EditorUtility.DisplayDialog("Ghost Baker",
            $"Ghost prefab created:\n{prefabPath}\n\nAssign transparent material on its MeshRenderer.",
            "Nice!");
        Selection.activeObject = prefab;
    }
}
