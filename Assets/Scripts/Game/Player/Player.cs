using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;

    public Inventory Inventory => _inventory;
}
