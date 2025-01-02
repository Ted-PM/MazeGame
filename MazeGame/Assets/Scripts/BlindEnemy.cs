using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class BlindEnemy : MonoBehaviour
{
    PlayerController _player;
    Transform target;
    Vector3 _currentTarget;
    NavMeshAgent agent;
    private CapsuleCollider _enemyCollider;
    private Animator _enemyAnimation;

    [SerializeField]
    private AudioSource _whistle;

    [SerializeField]
    private float _enemySpeed;
    private bool _isRoaming;
    public bool playerLoud;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindFirstObjectByType<PlayerController>();
        _enemyCollider = GetComponent<CapsuleCollider>();
        target = FindFirstObjectByType<PlayerController>().transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = _enemySpeed;
        _currentTarget = FindRandomDestination();
        _isRoaming = true;
        playerLoud = false;
        _enemyAnimation = GetComponentInChildren<Animator>();
        _whistle.Play();
        StartCoroutine(IsEnemyMoving());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BigMazeGenerator.Instance._wallsAreUp && _whistle.volume < 1)
        {
            _whistle.volume = 1;
        }
        if (!BigMazeGenerator.Instance._wallsAreUp && agent.speed != 0)
        {
            UpdateEnemySpeed(3);
            //_enemyAnimation.speed = 0;
        }
        else if (_player.sneakItemUsed || _player.isCrouching || !_player._isWalking)
        {
            UpdateEnemySpeed(0);
        }
        else if (_player._isWalking && !_player.isSprinting)
        {
            UpdateEnemySpeed(1);
        }
        else
        {
            UpdateEnemySpeed(2);
        }
        //StartCoroutine(IsEnemyMoving());
        if (playerLoud && PlayerInRange() && !_player.sneakItemUsed)
        {
            _currentTarget = target.position;
            Chase(_currentTarget);
            transform.LookAt(Camera.main.transform);
        }
        else if (!_isRoaming)
        {
            _currentTarget = FindRandomDestination();
            Chase(_currentTarget);
            //StartCoroutine(IsEnemyMoving());
        }
        else
        {
            if (Vector3.Distance(agent.transform.position, _currentTarget) <= 0.5f)
            {
                _isRoaming = false;
            }
            else
            {
                Chase(_currentTarget);
            }
        }
    }

    private IEnumerator IsEnemyMoving()
    {
        var oldPos = transform.position;
        yield return new WaitForSeconds(0.5f);
        var newPos = transform.position;

        float xChange = newPos.x - oldPos.x;
        float zChange = newPos.z - oldPos.z;
        if (xChange < 0.5f && xChange > -0.5f && zChange < 0.5f && zChange > -0.5f)
        {
            //Debug.Log("Enemy not moving, finding new path");
            _isRoaming = false;
        }

        StartCoroutine(IsEnemyMoving());

    }

    private IEnumerator DecreaseWhistle()
    {
        while (_whistle.volume >0)
        {
            yield return null;
            _whistle.volume -= 0.1f;
        }
    }
    //private IEnumerator WaitBeforeResetRoam()
    //{
    //yield return new WaitForSeconds(1);
    //}

    private bool PlayerInRange()
    {
        bool result = false;
        float playerDistanceX = transform.position.x - target.position.x;
        float playerDistanceZ = transform.position.z - target.position.z;

        if ((playerDistanceX <= 25 && playerDistanceX >= -25 && playerDistanceZ <= 25 && playerDistanceZ >= -25) || BigMazeGenerator.Instance._wallsAreUp)
        {
            result = true;
        }

        return result;
    }
    private void UpdateEnemySpeed(int ID = 0)
    {
        _enemyAnimation.speed = 1;
        switch (ID)
        {
            case 0:
                agent.speed = _enemySpeed;
                playerLoud = false;
                break;
            case 1:
                agent.speed = _enemySpeed + 2;
                playerLoud = true;
                break;
            case 2:
                agent.speed = _enemySpeed + 5;
                playerLoud = true;
                break;
            case 3:
                agent.speed = 0f;
                playerLoud = false;
                StartCoroutine(DecreaseWhistle());
                _enemyAnimation.speed = 0;
                break;
            default:
                Debug.LogWarning("Enemy speed not found");
                break;
        }
  
    }

    private void Chase(Vector3 dest)
    {
        agent.destination = dest;
    }
    //private void ChasePlayer()
    //{
    //    agent.destination = target.position;
    //}

    //void LookAt()
    //{
    //    transform.LookAt(Camera.main.transform);
    //    //transform.LookAt(Camera.main.transform);
    //}

    Vector3 FindRandomDestination()
    {
        _isRoaming = true;
        Vector3 destination;
        int width = BigMazeGenerator.Instance.GetMazeWidth();
        int depth = BigMazeGenerator.Instance.GetMazeDepth();

        int destinationX = Random.Range(-2, 3);
        while (destinationX == 0 || (destinationX + (int)(transform.position.x + 5) / 10) >= width || (destinationX + (int)(transform.position.x + 5) / 10) < 0)
        { destinationX = Random.Range(-2, 3); }

        int destinationZ = Random.Range(-2, 3);
        while (destinationZ == 0 || (destinationZ + (int)(transform.position.z + 5) / 10) >= depth || (destinationZ + (int)(transform.position.z + 5) / 10) < 0)
        { destinationZ = Random.Range(-2, 3); }

        destination = new Vector3((destinationX + (int)(transform.position.x + 5) / 10) * 10, 0, (destinationZ + (transform.position.z + 5) / 10) * 10);

        
        return destination;
    }
}
