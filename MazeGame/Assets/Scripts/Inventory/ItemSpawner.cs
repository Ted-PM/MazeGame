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

    public static List<InteractableItemScript> _iteractablePrefabs;

    [SerializeField]
    private GameObject _staticItemsParent;

    [SerializeField]
    private List<GameObject> _interactableParent;

    public int _minNumPerInteractable;// { get; private set; }
    private int[] _numInteractables;

    private void Awake()
    {
        Instance = this;
        _numInteractables = new int[_interactables.Count];
        //_numInteractables = new int[_interactables.Count]();
    }
    //private void Start()
    //{
    //    _iteractablePrefabs = new List<InteractableItemScript>();
    //}

    public void SpawnRandomInteractable(int x, int z)
    {
        int newItemIndex = 0;

        if (_iteractablePrefabs == null)
        {
            _iteractablePrefabs = new List<InteractableItemScript>();        }
        else
        {
            newItemIndex = _iteractablePrefabs.Count;
        }

        //int newItemParent = Random.Range(0, _interactables.Count);
        int newItemParent = ChooseInteractable();
        InteractableItemScript item = _interactables[newItemParent];
        //_iteractablePrefabs.Add(_interactables[Random.Range(0, _interactables.Count)]);

        float xPos = (float)(Random.Range(-40, 40) / 10);
        float zPos = (float)(Random.Range(-40, 40) / 10);
        float randomRotation = (float)(Random.Range(0, 360));
        

        var newItem = Instantiate(item, new Vector3(10 * x + xPos, 0.2f, 10 * z + zPos), Quaternion.identity, _interactableParent[newItemParent].transform);
        //newItem.tag = "Floor";
        newItem.transform.eulerAngles = new Vector3(0f, randomRotation, 0f);
        newItem._globalID = newItemIndex;

        _iteractablePrefabs.Add(newItem);
        _numInteractables[newItemParent]++;

        //// -----
        //var secondItem = Instantiate(item, /*item.transform.position + */new Vector3(10 * x + xPos, 0.2f, 10 * z + zPos), Quaternion.identity);
        ////newItem.tag = "Floor";
        //secondItem.transform.eulerAngles = new Vector3(0f, randomRotation, 0f);
        //secondItem._globalID = ++newItemIndex;

        //_iteractablePrefabs.Add(secondItem);
        ////newItem._globalID = 
        //// ----
    }

    private int ChooseInteractable()
    {
        int result = -1;
        List<int> unusedInteractables = new List<int>();
        for (int i = 0; i < _interactables.Count; i++)
        {
            if (_numInteractables[i] < _minNumPerInteractable)
            {
                unusedInteractables.Add(i);
            }
        }

        if (unusedInteractables.Count > 0)
        {
            result = Random.Range(0, unusedInteractables.Count);
            result = unusedInteractables[result];
        }
        else
        {
            result = Random.Range(0, _interactables.Count);
        }

        //int newItemParent = Random.Range(0, _interactables.Count);
        //InteractableItemScript item = _interactables[newItemParent];

        return result;
    }

    public void RemoveItem(int globalID)
    {
        StartCoroutine(_iteractablePrefabs[globalID].WaitThenDestroy());
    }

    public void SpawnRandomItem(int x, int z)
    {
        GameObject item = _items[Random.Range(0, _items.Count)];

        float xPos = (float)(Random.Range(-40, 40)/10);
        float zPos = (float)(Random.Range(-40, 40)/10);
        float randomRotation = (float)(Random.Range(0, 360));

        var newItem = Instantiate(item, new Vector3(10*x + xPos, 0, 10*z + zPos), Quaternion.identity, _staticItemsParent.transform);
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

    public int GetUniqueItemsCount()
    {
        return _interactables.Count;
    }
}
