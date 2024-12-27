using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _loadingText;
    //private TextMeshProUGUI _tempText;
    private int _dotCounter;

    // Start is called before the first frame update
    void Start()
    {
        //_tempText = _loadingText;
        _dotCounter = 0;
        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        yield return new WaitForSeconds(0.4f);
        //_loadingText.GetComponent<TMPro.TextMeshProUGUI>().text = _tempText.GetComponent<TMPro.TextMeshProUGUI>().text;
        switch (_dotCounter)
        {
            case 0:
                _loadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "Loading";
                break;
            case 1:
                _loadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "Loading .";
                break;
            case 2:
                _loadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "Loading . .";
                break;
            case 3:
                _loadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "Loading . . .";
                _dotCounter = -1;
                break;
        }
        _dotCounter++;
        StartCoroutine(Loading());
    }
}
