using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    static Inventory instance;
    public static Inventory Instance {
        get {
            if (instance == null) instance = (Instantiate(Resources.Load("InventoryUI")) as GameObject).GetComponent<Inventory>();
            return instance;
        }
    }

    private struct InventoryEntry{
        public InventoryEntry(InventoryItem item, GameObject interfaceObject) {
            this.item = item;
            this.interfaceObject = interfaceObject;
        }

        public InventoryItem item;
        public GameObject interfaceObject;
    }

    public int preferredItemWidth = 100;
    public GameObject itemContainer;

    GameObject inventoryImage;
    List<InventoryEntry> items = new List<InventoryEntry>();

    void Awake() {
        inventoryImage = Resources.Load<GameObject>("InventoryImage");
    }

    public GameObject instantiateInterfaceObject(Sprite sprite) {
        var interfaceObjectInstance = Instantiate(inventoryImage) as GameObject;
        interfaceObjectInstance.transform.SetParent(itemContainer.transform);
        interfaceObjectInstance.transform.localScale = Vector3.one;
        var image = interfaceObjectInstance.GetComponent<Image>();
        image.sprite = sprite;
        return interfaceObjectInstance;
    }

    public bool AddItem(string name) {
        var item = Resources.Load<InventoryItem>("InventoryItems/" + name);
        if (item == null) {
            Debug.LogError("InventoryItem " + name + " not found.");
            return false;
        }
        items.Add(new InventoryEntry(item, instantiateInterfaceObject(item.sprite)));
        return true;
    }

    public bool RemoveItem(string name) {
        var rmIndex = items.Select((value, index) => new { value, index = index + 1 })
                .Where(pair => pair.value.item.name == name)
                .Select(pair => pair.index)
                .FirstOrDefault() - 1;
        if (rmIndex == -1) return false;
        Destroy(items[rmIndex].interfaceObject);
        items.RemoveAt(rmIndex);
        return true;
    }

    public bool HasItem(string name) {
        return items.Where(itemEntry => itemEntry.item.name == name).Count() > 0;
    }
}
