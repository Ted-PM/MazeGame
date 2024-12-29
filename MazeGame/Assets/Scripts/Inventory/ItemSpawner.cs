using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    [SerializeField]
    private List<GameObject> _items;

    [SerializeField]
    private List<GameObject> _wallItems;

    [SerializeField]
    private List<InteractableItemScript> _interactables;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnRandomInteractable(int x, int z)
    {
        InteractableItemScript item = _interactables[Random.Range(0, _interactables.Count)];

        float xPos = (float)(Random.Range(-40, 40) / 10);
        float zPos = (float)(Random.Range(-40, 40) / 10);
        float randomRotation = (float)(Random.Range(0, 360));

        var newItem = Instantiate(item, /*item.transform.position + */new Vector3(10 * x + xPos, 0.5f, 10 * z + zPos), Quaternion.identity);
        //newItem.tag = "Floor";
        newItem.transform.eulerAngles = new Vector3(0f, randomRotation, 0f);
    }

    public void SpawnRandomItem(int x, int z)
    {
        GameObject item = _items[Random.Range(0, _items.Count)];

        float xPos = (float)(Random.Range(-40, 40)/10);
        float zPos = (float)(Random.Range(-40, 40)/10);
        float randomRotation = (float)(Random.Range(0, 360));

        var newItem = Instantiate(item, /*item.transform.position + */new Vector3(10*x + xPos, 0, 10*z + zPos), Quaternion.identity);
        newItem.tag = "Floor";
        newItem.transform.eulerAngles = new Vector3(0f, randomRotation, 0f);
    }

    public void SpawnWallItemLeft(int x, int z) //int x, int z, 
    {
        GameObject wallItem = _wallItems[Random.Range(0, _wallItems.Count)];

        var newItem = Instantiate(wallItem, new Vector3(-4f + x*10, 1.5f, z*10), Quaternion.identity);
        newItem.transform.eulerAngles = new Vector3(0f, -90f, 0f);
    }

    public void SpawnWallItemRight(int x, int z) //int x, int z, 
    {
        GameObject wallItem = _wallItems[Random.Range(0, _wallItems.Count)];

        var newItem = Instantiate(wallItem, new Vector3(4f + x * 10, 1.5f, z * 10), Quaternion.identity);
        newItem.transform.eulerAngles = new Vector3(0f, 90f, 0f);
    }

    public void SpawnWallItemFront(int x, int z) //int x, int z, 
    {
        GameObject wallItem = _wallItems[Random.Range(0, _wallItems.Count)];

        var newItem = Instantiate(wallItem, new Vector3(x * 10, 1.5f, 4f + z * 10), Quaternion.identity);
        //newItem.transform.eulerAngles = new Vector3(0f, 90f, 0f);
    }

    public void SpawnWallItemBack(int x, int z) //int x, int z, 
    {
        GameObject wallItem = _wallItems[Random.Range(0, _wallItems.Count)];

        var newItem = Instantiate(wallItem, new Vector3(x * 10, 1.5f, -4f + z * 10), Quaternion.identity);
        newItem.transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }
}
