using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _sensText;
    [SerializeField]
    private Slider _sensSlider;
    private int _minimum = 1;
    private int _maximum = 10;

    private float sensitivity;

    public void UpdateSensitivity()
    {
        _sensSlider.value = sensitivity;
        _sensText.text = "Sensitivity = " + sensitivity.ToString();
        PlayerController.lookSpeed = sensitivity;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (sensitivity == 0) { sensitivity = 2.5f; }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSensitivity();
        if (Input.GetKeyDown(KeyCode.RightArrow) && sensitivity <_maximum)
        {
            sensitivity += 0.5f;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && sensitivity > _minimum)
        {
            sensitivity -= 0.5f;
        }
    }

    
    void OnEnable()
    {
        sensitivity = PlayerPrefs.GetFloat("sensitivity");
        sensitivity = Mathf.Round(sensitivity * 10f) / 10f;
        if (sensitivity <1 ) { sensitivity = 1; }
        if (sensitivity > 10 ) { sensitivity = 10; }
        _sensSlider.value = sensitivity;
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        PlayerController.lookSpeed = PlayerPrefs.GetFloat("sensitivity");
    }
}
