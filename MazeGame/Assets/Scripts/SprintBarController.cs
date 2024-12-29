using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SprintBarController : MonoBehaviour
{
    [SerializeField]
    private GameObject _sprintBar;

    // basically governs how long player can sprint for
    public float timeToRefillSprint;
    public float timeToDrainSprint;

    public float barPercentComplete;
    [SerializeField]
    private float _sprintCooldownTime;
    public bool canSprint {  get; private set; }
    public bool _isSprinting;

    private Vector3 _baseScale;
    private Vector3 _minScale;
    //public IEnumerator _startSprinting;
    //public IEnumerator _sprintCooldown;
    //public IEnumerator _stopSprinting;

    private void Start()
    {
        barPercentComplete = 1;
        canSprint = true;
        _isSprinting = false;
        _baseScale  = _sprintBar.transform.localScale;
        _minScale = new Vector3(0, 1, 1);
        //_startSprinting = StartSprinting();
        //_sprintCooldown = SprintCooldown();
        //_stopSprinting = StopSprinting();
        StartCoroutine(SprintControler());
    }
    //private void Update()
    //{
    //    Debug.Log("enerfy: " + barPercentComplete);
    //}

    public IEnumerator SprintControler()
    {
        if (!canSprint)
        {
            _isSprinting = false;
        }
        //Vector3 InitialScale = _sprintBar.transform.localScale;
        //float StartCompletness = barPercentComplete;
        //_sprintBar.transform.localScale = new Vector3(StartCompletness, 1, 1);
        //yield return new WaitForEndOfFrame();
        if (_isSprinting && canSprint && barPercentComplete > 0.01)
        {
            float time = timeToDrainSprint * (1- barPercentComplete);
            //float StartCompletness = barPercentComplete;
            float timeToComplete = timeToDrainSprint;
            float t = 1 - barPercentComplete;
            //float t = 0;
            //Vector3 InitialScale = _sprintBar.transform.localScale;
            //_sprintBar.transform.localScale = new Vector3(StartCompletness, 1, 1);

            while (t < 1 && _isSprinting)
            {
                //Debug.Log("ratio: " + t);
                t = time / timeToComplete;
                time += Time.deltaTime;

                //_sprintBar.transform.localScale = Vector3.Lerp(new Vector3(StartCompletness, 1, 1), new Vector3(0, 1, 1), t);
                //_sprintBar.transform.localScale = Vector3.Lerp(InitialScale, new Vector3(0, 1, 1), t);
                _sprintBar.transform.localScale = Vector3.Lerp(_baseScale, _minScale, t);
                barPercentComplete = (1 - t);
                yield return new WaitForFixedUpdate();
            }
            if (barPercentComplete < 0.01)
            {
                _isSprinting = false;
                canSprint = false;
                barPercentComplete = 0.0f;
                yield return new WaitForSeconds(_sprintCooldownTime);
                //StartCoroutine(_sprintCooldown);
            }
        }
        else if (!_isSprinting && barPercentComplete < 1)
        {
            yield return new WaitForSeconds(0.1f);
            //yield return new WaitForEndOfFrame();
            float time = timeToRefillSprint * barPercentComplete;
            //float StartCompletness = barPercentComplete;
            //Vector3 InitialScale = _sprintBar.transform.localScale;

            float timeToComplete = timeToRefillSprint;
            float t = barPercentComplete;
            //float t = 0;
            //_sprintBar.transform.localScale = new Vector3(StartCompletness, 1, 1);
            canSprint = true;
            //_sprintBar.transform.localScale = new Vector3(barPercentComplete, 1, 1);
            while (barPercentComplete < 1 && !_isSprinting)
            {
                t = time / timeToComplete;
                time += Time.deltaTime;

                //_sprintBar.transform.localScale = Vector3.Lerp(new Vector3(StartCompletness, 1, 1), new Vector3(1, 1, 1), t);
                //_sprintBar.transform.localScale = Vector3.Lerp(InitialScale, new Vector3(1, 1, 1), t);
                _sprintBar.transform.localScale = Vector3.Lerp(_minScale, _baseScale, t);
                barPercentComplete = t;
                yield return new WaitForFixedUpdate();
            }

            if (barPercentComplete >= 1)
            {
                barPercentComplete = 1.0f;
            }
        }
         yield return new WaitForEndOfFrame();
        StartCoroutine(SprintControler());

    }

    //public IEnumerator StartSprinting()
    //{
    //    _isSprinting = true;
    //    StopCoroutine("StopSprinting");
    //    StopCoroutine("SprintCooldown");
    //    //StopCoroutine(_sprintCooldown);
    //    //StopCoroutine(_stopSprinting);
    //    //yield return null;
    //    yield return new WaitForEndOfFrame();
    //    float time = timeToDrainSprint * (1-barPercentComplete);
    //    float StartCompletness = barPercentComplete;
    //    float timeToComplete = timeToDrainSprint;
    //    float t = 1 - barPercentComplete;
    //    _sprintBar.transform.localScale = new Vector3(1 * StartCompletness, 1, 1);
    //    while (t < 1 && _isSprinting)
    //    {
    //        //Debug.Log("ratio: " + t);
    //        yield return null;
    //        time += Time.deltaTime;
    //        t = time / timeToComplete;

    //        _sprintBar.transform.localScale = Vector3.Lerp(_baseScale, _minScale, t);
    //        barPercentComplete = (1 - t);
    //    }

    //    if (barPercentComplete < 0.01)
    //    {
    //        barPercentComplete = 0.0f;
    //        StartCoroutine(SprintCooldown());
    //        //StartCoroutine(_sprintCooldown);
    //    }
    //}

    //public IEnumerator SprintCooldown()
    //{
    //    canSprint = false;
    //    yield return new WaitForSeconds(_sprintCooldownTime);
    //    StartCoroutine(StopSprinting());
    //    //StartCoroutine(_stopSprinting);
    //}

    //public IEnumerator StopSprinting()
    //{
    //    _isSprinting = false;
    //    StopCoroutine("StartSprinting");
    //    StopCoroutine("SprintCooldown");
    //    //StopCoroutine(_startSprinting);
    //    //StopCoroutine(_sprintCooldown);
    //    yield return new WaitForEndOfFrame();
    //    float time = timeToRefillSprint * barPercentComplete;
    //    float StartCompletness = barPercentComplete;

    //    float timeToComplete = timeToRefillSprint;
    //    float t = barPercentComplete;
    //    _sprintBar.transform.localScale = new Vector3(1 * StartCompletness, 1, 1);
    //    canSprint = true;

    //    while (barPercentComplete < 1 && !_isSprinting)
    //    {
    //        yield return null;
    //        time += Time.deltaTime;
    //        t = time / timeToComplete;

    //        _sprintBar.transform.localScale = Vector3.Lerp(_minScale,_baseScale, t);
    //        barPercentComplete = t;
    //    }

    //    if (barPercentComplete >=1)
    //    {
    //        barPercentComplete = 1.0f;
    //    }
    //}
    public void RefillBar()
    {
        StopCoroutine(SprintControler());
        barPercentComplete = 1;
        _sprintBar.transform.localScale = _baseScale;
        canSprint = true;
        StartCoroutine(SprintControler());
        //StopAllCoroutines();
        //StartCoroutine(RefillBarCoroutine());
    }
    //private IEnumerator RefillBarCoroutine()
    //{
    //    bool wasSprinting = _isSprinting;
        
    //    //StopCoroutine(_startSprinting);
    //    //StopCoroutine(_stopSprinting);
    //    //StopCoroutine(_sprintCooldown);

    //    ////StopAllCoroutines();
    //    //Debug.Log("Stopping Coroutines");
    //    //if (wasSprinting)
    //    //{
    //    //    StopCoroutine("StartSprinting");
    //    //}
    //    //else 
    //    //{
    //    //    StopCoroutine("SprintCooldown");
    //    //    StopCoroutine("StopSprinting");
    //    //}
    //    yield return new WaitForEndOfFrame();
    //    Debug.Log("Coroutines stopped");
    //    //if (_isSprinting)
    //    //{
    //    //    //StopCoroutine("StopSprinting");
    //    //    StopCoroutine("StartSprinting");
    //    //}
    //    //else
    //    //{
    //    //    StopCoroutine("SprintCooldown");
    //    //    StopCoroutine("StopSprinting");
    //    //}

    //    //yield return null;

    //    //float trueTimeToComplete = timeToRefillSprint;

    //    //timeToRefillSprint = 0.1f;

    //    //StartCoroutine(StopSprinting());
    //    //yield return null;
    //    //timeToRefillSprint = trueTimeToComplete;

    //    //if (wasSprinting)
    //    //{
    //    //    StartCoroutine(StartSprinting());
    //    //}
    //    barPercentComplete = 1;
    //    _isSprinting = false;
    //    canSprint = true;
    //    _sprintBar.transform.localScale = new Vector3(1, 1, 1);
    //    //yield return null;
    //    Debug.Log("Energy refilled: " + barPercentComplete);
    //    yield return new WaitForEndOfFrame();

    //    //if (wasSprinting)
    //    //{
    //    //    //StartCoroutine(_startSprinting);
    //    //}
    //    if (wasSprinting)
    //    {
    //        Debug.Log("Restarting sprinting");
    //        StartCoroutine(StartSprinting());

    //    }
    //}

}
