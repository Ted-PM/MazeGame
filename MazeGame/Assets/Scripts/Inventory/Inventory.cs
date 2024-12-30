using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [SerializeField]
    private List<InteractableItemScript> _itemList;

    [SerializeField]
    private InventorySlot[] _inventorySlots;

    [SerializeField]
    private PlayerController _playerController;

    private int _selectedSlot;
    private void Awake()
    {
        instance = this;
        _selectedSlot = 0;
    }

    public void SelectNextSlot()
    {
        _inventorySlots[_selectedSlot].DeSelectSlot();
        _selectedSlot++;
        _selectedSlot = _selectedSlot % 4;
        _inventorySlots[_selectedSlot].SelectSlot();
    }

    public void SelectPrevSlot()
    {
        _inventorySlots[_selectedSlot].DeSelectSlot();
        _selectedSlot--;
        if (_selectedSlot <= -1) { _selectedSlot = 3; }
        //_selectedSlot = _selectedSlot % 4;
        _inventorySlots[_selectedSlot].SelectSlot();
    }

    public void SelectSlot(int _slotNumber)
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (i == (_slotNumber) && !_inventorySlots[i].isSelected)
            {
                 _inventorySlots[i].SelectSlot();
            }
            else
            {
                _inventorySlots[i].DeSelectSlot();
            }
        }
    }

    public void PickUpItem(int _itemID)
    {
        int freeSlot = FindFreeSlot();
        if (_itemList[_itemID].GetComponent<SpriteRenderer>().sprite == null)
        {
            Debug.LogWarning("Warning: Item doesn't have a sprite.");
        }
        else if (freeSlot != -1)
        {
            _inventorySlots[freeSlot].GetItem(_itemList[_itemID].GetComponent<SpriteRenderer>().sprite, _itemID);
        }
    }

    public void UseItem()
    {
        int selectedSlot = FindSelectedSlot();
        int itemID = -1;

        if (selectedSlot != -1)
        {
            itemID = _inventorySlots[selectedSlot].UseItem();

            _playerController.PlayerUseItem(itemID);
        }
        else
        {
            Debug.Log("No Slot selected");
        }
    }

    private int FindSelectedSlot()
    {
        int result = -1;
        for (int i = 0; i < _inventorySlots.Length;i++)
        {
            if (_inventorySlots[i].hasItem && _inventorySlots[i].isSelected)
            {
                result = i;
                break;
            }
        }

        return result;
    }

    private int FindFreeSlot()
    {
        int result = -1;
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (!_inventorySlots[i].hasItem)
            {
                result = i;
                break;
            }
        }

        return result;
    }

    public bool HasFreeSlot()
    {
        bool result = false;

        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (!_inventorySlots[i].hasItem)
            {
                result = true;
                break;
            }
        }

        return result;
    }
}
