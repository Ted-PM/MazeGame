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

    bool landed;

    bool beenSeen;

    private CapsuleCollider _enemyCollider;

    //Renderer m_Renderer;

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
        DontDestroyOnLoad(this.gameObject);

        _enemyCollider = GetComponent<CapsuleCollider>();
        //m_Renderer = GetComponent<Renderer>();
        beenSeen = false;

        landed = false;
        GetComponent<NavMeshAgent>().enabled = false;
        //GetComponent<Renderer>().enabled = false;
        //agent = GetComponent<NavMeshAgent>();       //
        target = FindFirstObjectByType<PlayerController>().transform;
        //currentHealth = maxHealth;
        //healthBarBG.gameObject.SetActive(false);
        //healthBarFill.gameObject.SetActive(false);
        //healthBarBG.GetComponent<Image>()
    }

    void Update()
    {
        if (landed)
        {
            ChasePlayer();

            bool isAnyoneLookingAtMe = IsAnyoneLookingAtMe();
            if (isAnyoneLookingAtMe && !beenSeen && BigMazeGenerator.Instance._wallsAreUp)
            {
                Debug.Log("Seen");
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                GetComponent<NavMeshAgent>().speed = 0;
                GetComponent<NavMeshAgent>().angularSpeed = 0;
                GetComponentInChildren<Animator>().enabled = false;
                beenSeen = true;
            }
            else if (beenSeen)
            {
                LookAt();
            }
            //else if (beenSeen)
            //{
            //    Debug.Log("no longer seen");
            //    BigMazeGenerator.Instance.Spawn2Enemies();
            //    Destroy(gameObject);
            //}

            //Debug.Log("Enemy Speed: " + GetComponent<NavMeshAgent>().speed);
        }
        //LookAt();
    }

    public void IncreaseEnemySpeed()
    {
        GetComponent<NavMeshAgent>().speed += 2;
    }

    public void DecreaseEnemySpeed()
    {
        GetComponent<NavMeshAgent>().speed -=2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            StartCoroutine(WaitForNavMesh());
        }
    }

    public void RemoveEnemyRenderer()
    {
        GetComponent<Renderer>().enabled = false;
    }

    public void AddEnemyRenderer()
    {
        GetComponent<Renderer>().enabled = true;
    }

    private IEnumerator WaitForNavMesh()
    {
        yield return null;
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Renderer>().enabled = true;
        agent = GetComponent<NavMeshAgent>();
        landed = true;
    }

    private void EnemyDie()
    {
        Debug.Log("Enemy Die");
        BigMazeGenerator.Instance.Spawn2Enemies();
        Destroy(gameObject);
        //StartCoroutine(KillEnemy());
    }

    private IEnumerator KillEnemy()
    {
        yield return null;
        Destroy(gameObject);
    }

    void ChasePlayer()
    {
        agent.destination = target.position;
    }

    //void OnBecameVisible()
    //{
    //    Debug.Log("Object is seen");
    //    beenSeen = true;
    //}

    private void OnBecameInvisible()
    {
        if (beenSeen)
        {
            Debug.Log("No longer seen");
            EnemyDie();
            //Destroy(gameObject);
            //m_Renderer.enabled = false;
            //GetComponent<Collider>().enabled = false;
        }
    }

    void LookAt()
    {
        transform.LookAt(Camera.main.transform);
        transform.LookAt(Camera.main.transform);
    }

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

    // Returns true if any players in the list are looking at this gameObject
    // otherwise, returns false
    private bool IsAnyoneLookingAtMe()//List<PlayerController> players)
    {
        //if (FindObjectOfType<PlayerController>().TryGetComponent(out PlayerController possibleTarget))
        //{
            if (IsGameObjectInView())//possibleTarget.playerCamera))
            {
                return true;
            }
        //}

        //for (int i = 0; i < players.Count; i++)
        //{
        //    if (players[i].TryGetComponent(out PlayerController possibleTarget))
        //    {
        //        if (IsGameObjectInView())//possibleTarget.playerCamera))
        //        {
        //            return true;
        //        }
        //    }
        //}
        return false;
    }

    // Returns whether the current gameObject is within view of the given Camera
    private bool IsGameObjectInView()//Camera cam)
    {
        Camera cam = Camera.main;
        // Check if the object is within camera bounds
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Bounds bounds = _enemyCollider.bounds;
        if (!GeometryUtility.TestPlanesAABB(planes, bounds))
        {
            return false;
        }

        // Check if the object is visible within the camera (not occluded)
        Vector3[] corners = new Vector3[8];
        bounds.GetCorners(corners);

        foreach (Vector3 corner in corners)
        {
            Vector3 direction = corner - cam.transform.position;
            if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnApplicationQuit()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
