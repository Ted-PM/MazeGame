using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevelScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _instructions;
    [SerializeField]
    private GameObject _startText;
    private void Start()
    {
        _instructions.SetActive(true);
        _startText.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !_instructions.activeSelf)
        {
            GameManager.Instance.StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && _instructions.activeSelf)
        {
            _instructions.SetActive(false);
            _startText.SetActive(true);
        }
    }
}
