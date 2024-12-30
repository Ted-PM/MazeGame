using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [SerializeField]
    private List<InteractableItemScript> _itemList;

    [SerializeField]
    private InventorySlot[] _inventorySlots;

    [SerializeField]
    private PlayerController _playerController;

    [SerializeField]
    private TextMeshProUGUI _slotContents;

    private int _selectedSlot;
    private void Awake()
    {
        instance = this;
        _selectedSlot = 0;
        _slotContents.enabled = false;
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

    public void DisplayItemName(int itemID)
    {
        StopCoroutine("_DisplayItemName");
        _slotContents.enabled = false;
        StartCoroutine(_DisplayItemName(itemID));
    }
    private IEnumerator _DisplayItemName(int itemID)
    {
        //yield return new WaitForSeconds(0.1f);
        yield return new WaitForFixedUpdate();
        if (itemID != -1)
        {
            _slotContents.GetComponent<TMPro.TextMeshProUGUI>().text = _itemList[itemID].itemName.ToString();
            _slotContents.enabled = true;
            yield return new WaitForSeconds(1f);
        }
        _slotContents.enabled = false;
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

    public int GetNumberOfItems() { return _itemList.Count; }
}
