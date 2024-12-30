using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
//using static UnityEditor.Progress;
//using UnityEngine.ProBuilder.Shapes;

public class InventorySlot : MonoBehaviour
{
    private Image _slotBG;
    private SpriteRenderer _renderer;

    public int slotID;

    // true if has item in slot
    public bool hasItem;
    public int currentItemID;
    public bool isSelected;
    private Vector3 _defaultScale;
    private Vector3 _largerScale;
    //private IEnumerator _itemNameCoroutine;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _slotBG = GetComponentInChildren<Image>();
        hasItem = false;
        isSelected = false;
        currentItemID = -1;
        _defaultScale = this.transform.localScale;
        _largerScale = _defaultScale + new Vector3(0.1f, 0.1f, 0);
    }

    public void GetItem(Sprite _sprite, int itemID)
    {
        if (_renderer != null && _sprite != null)
        {
            currentItemID = itemID;
            _renderer.sprite = _sprite;
            hasItem = true;
        }
    }

    public int UseItem()
    {
        int itemID = currentItemID;
        if (_renderer != null && _renderer.sprite != null && currentItemID != -1 && isSelected)
        {
            _renderer.sprite = null;
            hasItem = false;
            DeSelectSlot();
            //Inventory.instance.UseItem(currentItemID);
            currentItemID = -1;
        }

        return itemID;
    }

    public void SelectSlot()
    {
        if (!isSelected)
        {
            isSelected = true;
            _slotBG.transform.localScale = _largerScale;
            _renderer.transform.localScale += new Vector3(2.5f, 2.5f, 0);
            Inventory.instance.DisplayItemName(currentItemID);
            //_itemNameCoroutine = Inventory.instance.DisplayItemName(currentItemID);
            //StartCoroutine(_itemNameCoroutine);
        }
    }
    public void DeSelectSlot()
    {
        if (isSelected)
        {
            isSelected = false;
            _slotBG.transform.localScale = _defaultScale;
            _renderer.transform.localScale -= new Vector3(2.5f, 2.5f, 0);
            //StopCoroutine(Inventory.instance.DisplayItemName(currentItemID));
            //StopCoroutine( _itemNameCoroutine );
        }
    }
}
