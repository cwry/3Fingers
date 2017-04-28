using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CreateCustomAssetsMenu{
    [MenuItem("Assets/Create/Inventory Item")]
    public static void CreateInventoryItem() {
        CustomAssetUtility.CreateAsset(ScriptableObject.CreateInstance<InventoryItem>());
    }
}
