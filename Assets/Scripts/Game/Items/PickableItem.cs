using Scripts.Events.Game;
using Scripts.Game;
using Scripts.Game.Items;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PickableItem : Interactable
{
    [SerializeField, ValueDropdown("GetItems")] private int _id;
    [SerializeField] private int _amount;
    [SerializeField] private string _interactDescription;

    public int Id => _id;
    public int Amount => _amount;

    public event Action PickedUp;

    public override string interactDescription => _interactDescription;

    public override void Interact()
    {
        EventManager.Instance.Invoke(new GameEvent.AddItem() { Id = _id, Amount = _amount });
        
        PickedUp?.Invoke();

        Destroy(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

#if UNITY_EDITOR
    private IEnumerable GetItems()
    {
        var itemsLibrary = AssetDatabase.LoadAssetAtPath<ItemsLibrary>("Assets/Resources/Libraries/ItemsLibrary.asset");
        var items = itemsLibrary.Items;
        var dropdownList = new ValueDropdownList<int>();

        foreach (var item in items)
            dropdownList.Add($"{item.Id} - {item.Name}", item.Id);

        return dropdownList;
    }
#endif
}
