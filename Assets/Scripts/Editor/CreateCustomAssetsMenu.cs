using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CreateCustomAssetsMenu{
    [MenuItem("Assets/Create/Inventory Item")]
    public static void CreateInventoryItem() {
        CustomAssetUtility.CreateAsset(ScriptableObject.CreateInstance<InventoryItem>());
    }

    [MenuItem("Assets/Create/Spine Animation Description")]
    public static void CreateSpineAnimationDescription() {
        CustomAssetUtility.CreateAsset(ScriptableObject.CreateInstance<SpineAnimationDescription>());
    }
}
