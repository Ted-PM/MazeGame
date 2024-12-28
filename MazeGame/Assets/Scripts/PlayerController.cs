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

    [SerializeField]
    private SprintBarController _resetWallsBar;

    //[SerializeField]
    private Camera _playerCamera;

    //float shake = 1.0f;
    float shakeAmount = 0.4f;
    float decreaseFactor = 1.0f;


    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
        rb = GetComponent<Rigidbody>();
        _playerCamera = GetComponentInChildren<Camera>();
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
            transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed); //move forward
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.back * Time.deltaTime * movementSpeed); //move backwards
        }
        if (Input.GetKey("a"))
        {
            transform.Translate(Vector3.left * Time.deltaTime * movementSpeed); //move left
        }
        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * movementSpeed); //move right
        }

        if (Input.GetKey(KeyCode.Space) && canJump)
        {
            canJump = false;
            rb.AddForce(transform.up * jumpForce);
        }

        if (Input.GetKeyDown("q"))
        {
            ShakeCamera(1.0f);

            //_playerCamera.GetComponent<Animator>().SetTrigger("ShakeCameraTrigger"); 
        }


        if (Input.GetKeyDown("left shift") && _sprintBar.canSprint  && !isSprinting)
        {
            //StopCoroutine("PlayerWalkingAnim");
            //StartCoroutine(PlayerWalkingAnim(0.2f, 3f));
            StartSprinting();
        }
        else if ((!Input.GetKey("left shift") && isSprinting) || !_sprintBar.canSprint)
        {
            //StopCoroutine("PlayerWalkingAnim");
            StopSprinting();
        }

        StartCoroutine(PlayerIsMoving());

        if (_isWalking && !_walkingAnimPlaying)
        {
            StartCoroutine(PlayerWalkingAnim());
        }
        
        if (_isWalking && !_stepSoundPlaying)
        {
            //StopCoroutine("PlayerWalkingAnim");
            //StartCoroutine(PlayerWalkingAnim());
            //_playerCamera.GetComponent<Animator>().SetTrigger("ShakeCameraTrigger");
            StartCoroutine(WalkSound());
            
        }
        else if (!_isWalking) 
        {
            //_playerCamera.GetComponent<Animator>().ResetTrigger("ShakeCameraTrigger");
            StopCoroutine("PlayerWalkingAnim");
            StopCoroutine(WalkSound());
        }
    }

    private bool _walkingAnimPlaying = false;

    private IEnumerator PlayerWalkingAnim()
    {
        if (canJump && _isWalking)
        {
            _walkingAnimPlaying = true;
            float time = 0.0f;
            float speed = 0.0f;
            if (isSprinting)
            {
                time = 0.395f;
                speed = 1.5f;
            }
            else
            {
                time = 0.595f;
                speed = 1f;
            }
            _playerCamera.GetComponent<Animator>().speed = speed;
            _playerCamera.GetComponent<Animator>().SetTrigger("ShakeCameraTrigger");
            yield return new WaitForSeconds(time);
            //_playerCamera.GetComponent<Animator>().ResetTrigger("ShakeCameraTrigger");
            _playerCamera.GetComponent<Animator>().SetTrigger("StopCameraShake");
            //yield return null;
            StartCoroutine(PlayerWalkingAnim());
        }
        else
        {
            _walkingAnimPlaying = false;
        }
    }

    public void ShakeCamera(float shake)
    {
        //Debug.Log("Start Shake cam");
        StopCoroutine("PlayerWalkingAnim");
        _playerCamera.GetComponent<Animator>().enabled = false;
        StartCoroutine(_ShakeCamera(shake));
    }

    //private IEnumerator _ShakeCamera2(float time)
    //{

    //}

    private IEnumerator _ShakeCamera(float shake)
    {
        yield return null;
        //Debug.Log("shaking, Shake = " + shake);

        if (shake > 0)
        {
            Vector3 oldPos = _playerCamera.transform.localPosition;
            Vector3 newPos = Random.insideUnitSphere * shakeAmount;

            float time = 0;
            float t = 0;
            while (t < 1)
            {
                time += Time.deltaTime;
                t = time / .2f;
                _playerCamera.transform.localPosition = Vector3.Lerp(oldPos, newPos, t);
                //Debug.Log("Cam Pos = " + _playerCamera.transform.localPosition.x + ", " + _playerCamera.transform.localPosition.y + ", " + _playerCamera.transform.localPosition.z);
            }

            //_playerCamera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
            shake -= Time.deltaTime * decreaseFactor;
            StartCoroutine(_ShakeCamera(shake));
        }
        else
        {
            _playerCamera.transform.localPosition = new Vector3(0, 0, 0);
            shake = 0.0f;
            _playerCamera.GetComponent<Animator>().enabled = true;
        }
    }

    private IEnumerator PlayerIsMoving()
    {
        Vector3 oldPos = transform.position;
        yield return null;
        Vector3 newPos = transform.position;
        if ((int)oldPos.x != (int)newPos.x || (int)oldPos.z != (int)newPos.z)
        {
            _isWalking = true;
        }
        else
        {
            _isWalking =  false;
            StopCoroutine(WalkSound());
        }
    }

    private IEnumerator WalkSound()
    {
        if (canJump && _isWalking && isSprinting)
        {
            var leftStep = _step;
            var rightStep = _step;
            _stepSoundPlaying = true;
            if (!_step.isPlaying)
            {
                yield return new WaitForSeconds((0.4f)/3f);
                leftStep.Play();
                yield return new WaitForSeconds((0.4f)/2f);
                rightStep.Play();
                //_step.Play();
            }
            yield return new WaitForSeconds((0.4f) / 6f);
            //_step.Stop();
            leftStep.Stop();
            rightStep.Stop();
            _stepSoundPlaying = false;
            //if (!_step.isPlaying)
            //{
            StartCoroutine(WalkSound());
            //}
        }
        else if (canJump && _isWalking)
        {
            var leftStep = _step; 
            var rightStep = _step;  
            _stepSoundPlaying = true;
            if (!_step.isPlaying)
            {
                yield return new WaitForSeconds((0.6f)/3f);
                leftStep.Play();
                yield return new WaitForSeconds((0.6f) / 2f);
                rightStep.Play();
                //_step.Play();
            }
            yield return new WaitForSeconds((0.6f) / 6f);
            leftStep.Stop();
            rightStep.Stop();
            //_step.Stop();
            _stepSoundPlaying = false;
            //if (!_step.isPlaying)
            //{
            StartCoroutine(WalkSound());
            //}
        }
        else
        {
            _stepSoundPlaying = false;
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
            if (BigMazeGenerator.Instance._canResetWalls == true && _resetWallsBar.barPercentComplete >= 0.95)
            {
                rb.isKinematic = true;
                //rb.constraints = RigidbodyConstraints.FreezePosition;
                //rb.constraints = RigidbodyConstraints.FreezeRotation;
                //MazeGenerator.Instance.ClearAllWalls(waitBeforeResetWallsTime);
                StartCoroutine(_resetWallsBar.StartSprinting());
                //_canInteractWithWalls = false;
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
            //Debug.Log("start coroutine ");
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
            //Debug.Log("start stop corouting " );
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

    public void FreezePlayer()
    {
        //rb.constraints = RigidbodyConstraints.None;
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = true;

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
