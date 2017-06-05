using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    static Inventory _instance;
    static Inventory Instance {
        get {
            if (_instance == null) _instance = (Instantiate(Resources.Load("GameUI")) as GameObject).GetComponent<Inventory>();
            return _instance;
        }
    }

    struct InventoryEntry{
        public InventoryEntry(InventoryItem item, GameObject interfaceObject) {
            this.item = item;
            this.interfaceObject = interfaceObject;
        }

        public InventoryItem item;
        public GameObject interfaceObject;
    }

    public GameObject itemContainer;

    GameObject inventoryImage;
    List<InventoryEntry> items = new List<InventoryEntry>();

    void Awake() {
        if (_instance == null) _instance = this;
        inventoryImage = Resources.Load<GameObject>("InventoryImage");
    }

    GameObject instantiateInterfaceObject(Sprite sprite) {
        var interfaceObjectInstance = Instantiate(inventoryImage) as GameObject;
        interfaceObjectInstance.transform.SetParent(itemContainer.transform);
        interfaceObjectInstance.transform.localScale = Vector3.one;
        var image = interfaceObjectInstance.GetComponent<Image>();
        image.sprite = sprite;
        return interfaceObjectInstance;
    }

    public static bool AddItem(string name) {
        var item = Resources.Load<InventoryItem>("InventoryItems/" + name);
        if (item == null) {
            Debug.LogError("InventoryItem " + name + " not found.");
            return false;
        }
        Instance.items.Add(new InventoryEntry(item, Instance.instantiateInterfaceObject(item.sprite)));
        return true;
    }

    public static bool RemoveItem(string name) {
        var rmIndex = Instance.items.Select((value, index) => new { value, index = index + 1 })
                .Where(pair => pair.value.item.name == name)
                .Select(pair => pair.index)
                .FirstOrDefault() - 1;
        if (rmIndex == -1) return false;
        Destroy(Instance.items[rmIndex].interfaceObject);
        Instance.items.RemoveAt(rmIndex);
        return true;
    }

    public static bool HasItem(string name) {
        return Instance.items.Where(itemEntry => itemEntry.item.name == name).Count() > 0;
    }
}
