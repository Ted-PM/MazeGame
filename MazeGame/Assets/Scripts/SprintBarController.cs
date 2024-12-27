using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintBarController : MonoBehaviour
{
    [SerializeField]
    private GameObject _sprintBar;

    //[SerializeField]
    //private GameObject _sprintBarBG;

    // basically governs how long player can sprint for
    public float timeToRefillSprint;

    public float barPercentComplete;
    [SerializeField]
    private float _sprintCooldownTime;
    public bool canSprint {  get; private set; }
    private bool _isSprinting;

    private void Start()
    {
        barPercentComplete = 1;
        canSprint = true;
        _isSprinting = false;
    }

    public IEnumerator StartSprinting()
    {
        _isSprinting = true;
        StopCoroutine("StopSprinting");
        StopCoroutine("SprintCooldown");
        yield return null;
        float time = timeToRefillSprint*(1-barPercentComplete);
        float StartCompletness = barPercentComplete;
        float timeToComplete = timeToRefillSprint;
        float t = 1- barPercentComplete;
        _sprintBar.transform.localScale = new Vector3(1 * StartCompletness, 1, 1);
        while (t < 1 && _isSprinting)
        {
            //Debug.Log("ratio: " + t);
            yield return null;
            time += Time.deltaTime;
            t = time / timeToComplete;

            _sprintBar.transform.localScale = Vector3.Lerp(new Vector3(1 * StartCompletness, 1, 1), new Vector3(0, 1, 1), t);
            barPercentComplete = 1- t;
        }

        if (barPercentComplete < 0.1)
        {
            StartCoroutine(SprintCooldown());
        }
    }

    public IEnumerator SprintCooldown()
    {
        canSprint = false;
        yield return new WaitForSeconds(_sprintCooldownTime);
        StartCoroutine(StopSprinting());
    }

    public IEnumerator StopSprinting()
    {
        _isSprinting = false;
        StopCoroutine("StartSprinting");
        StopCoroutine("SprintCooldown");
        yield return null;
        float time = timeToRefillSprint * barPercentComplete;
        float StartCompletness = barPercentComplete;

        float timeToComplete = timeToRefillSprint;
        float t = barPercentComplete;
        _sprintBar.transform.localScale = new Vector3(1 * StartCompletness, 1, 1);
        canSprint = true;

        while (barPercentComplete < 1 && !_isSprinting)
        {
            yield return null;
            time += Time.deltaTime;
            t = time / timeToComplete;

            _sprintBar.transform.localScale = Vector3.Lerp(new Vector3(1 * StartCompletness, 1, 1), new Vector3(1, 1, 1), t);
            barPercentComplete = t;
        }
    }

}
