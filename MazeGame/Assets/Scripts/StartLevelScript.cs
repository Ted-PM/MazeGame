using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevelScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _instructions;
    [SerializeField]
    private GameObject _startText;
    [SerializeField]
    private AudioSource _clickedSound;
    private void Start()
    {
        _instructions.SetActive(true);
        _startText.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !_instructions.activeSelf)
        {
            _clickedSound.Play();
            GameManager.Instance.StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && _instructions.activeSelf)
        {
            _clickedSound.Play();
            _instructions.SetActive(false);
            _startText.SetActive(true);
        }
    }
}
