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
    [SerializeField]
    private GameObject _sensitivityManager;
    private void Start()
    {
        _instructions.SetActive(true);
        _startText.SetActive(false);
        _sensitivityManager.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !_instructions.activeSelf && !_sensitivityManager.activeSelf)
        {
            _clickedSound.Play();
            GameManager.Instance.StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && !_instructions.activeSelf && _sensitivityManager.activeSelf)
        {
            _clickedSound.Play();
            _sensitivityManager.SetActive(false);
            _startText.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Return) && _instructions.activeSelf && !_sensitivityManager.activeSelf)
        {
            _clickedSound.Play();
            _instructions.SetActive(false);
            _sensitivityManager.SetActive(true);
            //_startText.SetActive(true);
        }

        if (Input.GetKeyDown("i") && !_instructions.activeSelf && !_sensitivityManager.activeSelf)
        {
            _clickedSound.Play();
            _startText.SetActive(false);
            _sensitivityManager.SetActive(true);
        }
        else if (Input.GetKeyDown("i") && !_instructions.activeSelf)
        {
            _clickedSound.Play();
            _startText.SetActive(true);
            _sensitivityManager.SetActive(false);
        }
    }
}
