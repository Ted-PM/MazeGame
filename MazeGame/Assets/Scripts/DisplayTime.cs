using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTime : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;
    [SerializeField]
    private string _displayText;
    private float _time;
    void Start()
    {
        _time = PlayerPrefs.GetFloat("gameTime");
        float minutes = Mathf.FloorToInt(_time / 60);
        float seconds = Mathf.FloorToInt(_time % 60);

        _timerText.text = _displayText + string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
