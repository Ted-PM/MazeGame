using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;

    //public GameObject _leftEye;
    //public GameObject _rightEye;

    //public float walkingSpeed;
    //public float maxHealth;
    //float currentHealth;
    //public float attackDamage;

    //public GameObject gutsPrefab;
    //public GameObject babyGutsPrefab;

    //public Image healthBarBG;
    //public Image healthBarFill;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();       //
        target = FindFirstObjectByType<PlayerController>().transform;
        //currentHealth = maxHealth;
        //healthBarBG.gameObject.SetActive(false);
        //healthBarFill.gameObject.SetActive(false);
        //healthBarBG.GetComponent<Image>()
    }

    void Update()
    {
        ChasePlayer();
        //LookAt();
    }


    void ChasePlayer()
    {
        agent.destination = target.position;
    }

    //void LookAt()
    //{
    //    _leftEye.transform.LookAt(Camera.main.transform);
    //    _rightEye.transform.LookAt(Camera.main.transform);
    //}

    public void TakeDamage(float damageToTake)        // public for projectile script 
    {
        //healthBarBG.gameObject.SetActive(true);
        //healthBarFill.gameObject.SetActive(true);
        //currentHealth -= damageToTake;

        //if (currentHealth <= 0)
        //{
        //    ZombieSpawner.Instance.CountZombies();
        //    Instantiate(gutsPrefab, transform.position, Quaternion.identity, null);
        //    Destroy(gameObject);

        //}
        //else
        //{
        //    Instantiate(babyGutsPrefab, transform.position, Quaternion.identity, null);
        //    healthBarFill.fillAmount = currentHealth / maxHealth;
        //}
    }

    public void GiveDamage()
    {



    }
}