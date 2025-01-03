using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;
    private float _timerTime;
    //void Start()
    //{
    //    _timerTime = 0f;
    //}

    private void OnEnable()
    {
        _timerTime = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _timerTime += Time.fixedDeltaTime;
        UpdateTimer();
        //_timerText.text = _timerTime.ToString();
    }

    private void UpdateTimer()
    {
        float minutes = Mathf.FloorToInt(_timerTime / 60);
        float seconds = Mathf.FloorToInt(_timerTime % 60);

        _timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("gameTime", _timerTime);
    }
}
