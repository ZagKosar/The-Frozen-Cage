using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    [JsonProperty] private List<InventoryItem> _items = new()
    {
        new InventoryItem(){Id = 2, Amount = 1}
    };

    [JsonIgnore] public IReadOnlyList<InventoryItem> Items => _items;

    public void AddItem(int id, int amount = 1)
    {
        InventoryItem item;

        var index = _items.FindIndex(i => i.Id == id);

        if (index != -1)
        {
            item = _items[index];
            item.Amount += amount;
        }
        else
        {
            item = new InventoryItem();
            item.Id = id;
            item.Amount = amount;

            _items.Add(item);
        }
    }

    public void RemoveItem(int id, int amount = 1)
    {
        InventoryItem item;

        var index = _items.FindIndex(i => i.Id == id);

        if (index == -1)
            return;

        item = _items[index];
        item.Amount -= amount;

        if (item.Amount <= 0)
            _items.RemoveAt(index);
    }
}

[Serializable]
public class InventoryItem
{
    public int Id;
    public int Amount;
}