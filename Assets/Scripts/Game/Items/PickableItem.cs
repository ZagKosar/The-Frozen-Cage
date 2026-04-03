using Scripts.Events.Game;
using Scripts.Game;
using UnityEngine;

public class PickableItem : Interactable
{
    [SerializeField] private int _id;
    [SerializeField] private int _amount;
    [SerializeField] private string _interactDescription;

    public int Id => _id;
    public int Amount => _amount;

    public override string interactDescription => _interactDescription;

    public override void Interact()
    {
        EventManager.Instance.Invoke(new GameEvent.AddItem() { Id = _id, Amount = _amount });
        
        Destroy(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
