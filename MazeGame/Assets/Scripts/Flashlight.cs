using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private GameObject _spotLight;
    public static bool lightOn;
    private void Start()
    {
        lightOn = true;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _spotLight.SetActive(!_spotLight.activeSelf);
            lightOn = _spotLight.activeSelf;
        }
    }
    public IEnumerator Flicker(float time)
    {
        for (int i = 1; i < 4; i++)
        {
            _spotLight.SetActive(false);
            yield return new WaitForSeconds(time*0.1f*i);
            _spotLight.SetActive(true);
            yield return new WaitForSeconds(time * 0.1f * i);
        }
    }
}
