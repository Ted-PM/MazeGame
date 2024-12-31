using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InteractableItemScript : MonoBehaviour
{

    public int _itemID;
    public string itemName;
    public int _globalID;
    //public string _itemName { get; private set; }

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player Nearby");
            Inventory.instance.DisplayNearbyItems(_itemID, _globalID);
        }
    }

    public void PickUpItem()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        Inventory.instance.StopDisplayNearbyItem(_globalID);
        Debug.Log("Player Left");
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player" && Inventory.instance.HasFreeSlot())
    //    {
    //        //Inventory.instance.PickUpItem(_itemID);
    //        StartCoroutine(WaitThenDestroy());
    //    }
    //}

    public IEnumerator WaitThenDestroy()
    {
        ItemSpawner._iteractablePrefabs[_globalID] = null;
        Inventory.instance.StopDisplayNearbyItem(_globalID);
        yield return null;
        Destroy(gameObject);
    }
    
}
