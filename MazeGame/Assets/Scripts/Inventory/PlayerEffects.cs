using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _effects;

    [HideInInspector]
    public bool[] effectActive;
    void Start()
    {
        effectActive = new bool[_effects.Count];
        for (int i = 0; i < _effects.Count; i++)
        {
            effectActive[i] = false;
            _effects[i].SetActive(false);
        }
    }

    public IEnumerator StartEffect(float time, int effectID)
    {
        while (effectActive[effectID])
        {
            yield return null;
        }
        StartCoroutine(DisplayEffect(time, effectID));
    }

    public IEnumerator DisplayEffect(float time, int effectID)
    {
        _effects[effectID].SetActive(true);
        effectActive[effectID] = true;
        float t = 0f;
        bool flash1 = false;
        bool flash2 = false;


        while (t < time)
        {
            //_effects[effectID].SetActive(false);
            //yield return (t + 1) / time;

            if (t > (time * 0.75f) && !flash2)
            {
                _effects[effectID].SetActive(false);
                yield return new WaitForSeconds(0.3f);
                _effects[effectID].SetActive(true);
                flash2 = true;
            }
            else if (t > (time * 0.5f) && !flash1)
            {
                _effects[effectID].SetActive(false);
                yield return new WaitForSeconds(0.1f);
                _effects[effectID].SetActive(true);
                flash1  =true;
            }
            t+= Time.deltaTime;
            //_effects[effectID].SetActive(true);
            yield return null;
            //yield return (t + 1) / time;
            //t += Time.deltaTime;
        }
        _effects[effectID].SetActive(false);
        effectActive[effectID] = false;
    }

    //public void OnDisable()
    //{
    //    for (int i = 0; i < effectActive.Count; i++)
    //    {
    //        Destroy(effectActive[i]);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
