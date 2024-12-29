using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItemScript : MonoBehaviour
{

    public int _itemID;
    //public string _itemName { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && Inventory.instance.HasFreeSlot())
        {
            Inventory.instance.PickUpItem(_itemID);
            StartCoroutine(WaitThenDestroy());
        }
    }

    private IEnumerator WaitThenDestroy()
    {
        yield return null;
        Destroy(gameObject);
    }
    
}
