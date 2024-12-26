using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float jumpForce;

    public Transform playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    float rotationX = 0;
    Rigidbody rb;

    [SerializeField]
    private float waitBeforeResetWallsTime;

    [SerializeField]
    private AudioSource _step;
    private bool _stepSoundPlaying;
    private bool _isWalking;

    [SerializeField]
    private AudioSource _heartBeat;

    Collider _collider;

    bool canJump;


    [SerializeField]
    private float _sprintMultiplier;
    public bool isSprinting = false;
    [SerializeField]
    private SprintBarController _sprintBar;
    //[SerializeField]
    //[SerializeField]
    //private AudioSource _playerKilled;
    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _heartBeat.Play();
        canJump = true;
        isSprinting = false;
        _stepSoundPlaying = false;
        _isWalking = false;
        //StartCoroutine(WalkSound());
        //rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        lookAtMouse();

        if (!rb.isKinematic)
        {
            Mover();
        }
        else
        {
            StopSprinting();
        }
        if (FindObjectOfType<BigMazeGenerator>() != null)
        {
            WallController();
        }
        //Debug.Log(sprintRatio);
    }

    void Mover()
    {
        if (Input.GetKey("w"))
        {
            //StartCoroutine(WalkingSound());
            if (!_stepSoundPlaying)
            {
                _isWalking = true;
                StopCoroutine(WalkSound());
                StartCoroutine(WalkSound());
            }
            transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed); //move forward
        }
        if (Input.GetKey("s"))
        {
            if (!_stepSoundPlaying)
            {
                _isWalking = true;
                StopCoroutine(WalkSound());
                StartCoroutine(WalkSound());
            }
            transform.Translate(Vector3.back * Time.deltaTime * movementSpeed); //move backwards
        }
        if (Input.GetKey("a"))
        {
            if (!_stepSoundPlaying)
            {
                _isWalking = true;
                StopCoroutine(WalkSound());
                StartCoroutine(WalkSound());
            }
            transform.Translate(Vector3.left * Time.deltaTime * movementSpeed); //move left
        }
        if (Input.GetKey("d"))
        {
            if (!_stepSoundPlaying)
            {
                _isWalking = true;
                StopCoroutine(WalkSound());
                StartCoroutine(WalkSound());
            }
            transform.Translate(Vector3.right * Time.deltaTime * movementSpeed); //move right
        }
        if (!Input.GetKey("w") && !Input.GetKey("s") && !Input.GetKey("a") && !Input.GetKey("d"))
        {
            if (_stepSoundPlaying)
            {
                StopCoroutine(WalkSound());
                _stepSoundPlaying = false;
                _isWalking = false;
            }
        }

        if (Input.GetKey(KeyCode.Space) && canJump)
        {
            canJump = false;
            rb.AddForce(transform.up * jumpForce);
        }


        if (Input.GetKeyDown("left shift") && _sprintBar.canSprint && !isSprinting)
        {
            StartSprinting();
        }
        else if ((!Input.GetKey("left shift") && isSprinting) || !_sprintBar.canSprint)
        {
            StopSprinting();
        }
        
        //else
        //{
        //    StartCoroutine(SprintCooldown());
        //}
    }

    //private void WalkingSound()
    //{
    //    //AudioSource tempStep = _step;
    //    if (!_step.isPlaying && !isSprinting && canJump)
    //    {
    //        _step.Play();
    //    }
    //    //yield return new WaitForSeconds(.5f);
    //    //if (tempStep.isPlaying)
    //    //{
    //    //    tempStep.Stop();
    //    //}
    //    //else if (isSprinting)
    //    //{

    //    //}
    //}

    private IEnumerator WalkSound()
    {
        if (canJump && _isWalking && isSprinting)
        {
            _stepSoundPlaying = true;
            if (!_step.isPlaying)
            {
                _step.Play();
            }
            yield return new WaitForSeconds(.3f);
            _step.Stop();
            if (!_step.isPlaying)
            {
                StartCoroutine(WalkSound());
            }
        }
        else if (canJump && _isWalking)
        {
            _stepSoundPlaying = true;
            _step.Play();
            yield return new WaitForSeconds(.7f);
            _step.Stop();
            StartCoroutine(WalkSound());
        }
        //else
        //{
        //    _stepSoundPlaying = false;
        //    yield return null;
        //}
    }

    private void WallController()
    {
        if (Input.GetKeyDown("v"))
        {
            //if (MazeGenerator.Instance._canResetWalls == true)
            if (BigMazeGenerator.Instance._canResetWalls == true)
            {
                rb.isKinematic = true;
                //rb.constraints = RigidbodyConstraints.FreezePosition;
                //rb.constraints = RigidbodyConstraints.FreezeRotation;
                //MazeGenerator.Instance.ClearAllWalls(waitBeforeResetWallsTime);
                BigMazeGenerator.Instance.ClearAllWalls(waitBeforeResetWallsTime);
            }
            //else if (MazeGenerator.Instance._canResetWalls == false)
            else if (BigMazeGenerator.Instance._canResetWalls == false)
            {
                //MazeGenerator.Instance.PlayerResetAll();
                BigMazeGenerator.Instance.PlayerResetAll();
            }
        }
    }

    private void StartSprinting()
    {
        //Debug.Log("current speed: " + movementSpeed);
        if (!isSprinting)
        {
            movementSpeed = movementSpeed * _sprintMultiplier;
            isSprinting = true;
            if (_sprintBar.canSprint)
            {
                StartCoroutine(_sprintBar.StartSprinting());
            }
            Debug.Log("start coroutine ");
        }
        //else if (canSprint)
        //{
        //    _sprintTime += Time.deltaTime;
        //    sprintRatio = _sprintTime / _maxSprintTime;
        //    if (sprintRatio >= 1)
        //    {
        //        canSprint = false;
        //    }
        //}
    }

    private void StopSprinting()
    {
        //Debug.Log("current stop speed: " + movementSpeed);
        if (isSprinting)
        {
            movementSpeed = movementSpeed / _sprintMultiplier;
            isSprinting = false;
            if (_sprintBar.canSprint) 
            { 
                StartCoroutine(_sprintBar.StopSprinting());
            }
            Debug.Log("start stop corouting " );
        }   
    }

    //private IEnumerator SprintCooldown()
    //{
    //    yield return new WaitForSeconds(_sprintCooldownTime);

    //    canSprint = true;
    //    StopSprinting();
    //}

    public void UnfreezePlayer()
    {
        //rb.constraints = RigidbodyConstraints.None;
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            canJump = true;
            StartCoroutine(WalkSound());
        }
        if (collision.gameObject.tag == "Enemy")
        {

            GetComponentInChildren<CapsuleCollider>().enabled = false;
            GameManager.Instance.PlayerDead();
        }
        if (collision.gameObject.tag == "End")
        {
            GameManager.Instance.PlayerWin();
        }
    }

    void lookAtMouse()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}
