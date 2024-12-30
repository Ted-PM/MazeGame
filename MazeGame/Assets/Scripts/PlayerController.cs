using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    private float _baseSpeed;
    public float maxSpeed;
    public float jumpForce;
    private float _baseJump;

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
    [HideInInspector]
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

    [HideInInspector]
    public static bool playerInvisible;
    [HideInInspector]
    public bool speedItemUsed;
    [HideInInspector]
    public bool sneakItemUsed;

    [HideInInspector]
    public bool isCrouching;
    public float crouchAmount;
    public float timeToCrouch;
    [HideInInspector]
    public bool canCrouch;

    //private IEnumerator _sprintCoroutine;
    //private IEnumerator _stopSprintCoroutine;

    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
        rb = GetComponent<Rigidbody>();
        _playerCamera = GetComponentInChildren<Camera>();
    }
    private void Start()
    {
        //_sprintCoroutine = _sprintBar.StartSprinting();
        //_stopSprintCoroutine = _sprintBar.StopSprinting();
        playerInvisible = false;
        speedItemUsed = false;
        _heartBeat.Play();
        canJump = true;
        isSprinting = false;
        _stepSoundPlaying = false;
        _isWalking = false;
        _baseSpeed = movementSpeed;
        _baseJump = jumpForce;
        isCrouching = false;
        canCrouch = true;
        sneakItemUsed = false;
        //StartCoroutine(WalkSound());
        //rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        lookAtMouse();
        PlayerItemSelector();

        if (FindObjectOfType<BigMazeGenerator>() != null)
        {
            WallController();
        }
        //Debug.Log(sprintRatio);
    }

    private void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            Mover();
        }
        else
        {
            StopSprinting();
        }
        //Debug.Log("Velocity: " + rb.velocity.magnitude);
    }

    void PlayerItemSelector()
    {
        float wheele = Input.mouseScrollDelta.y;

        if (wheele > 0)
        {
            for (int i = 0; i < (int)wheele; i++)
            {
                Inventory.instance.SelectNextSlot();
            }
        }
        else if (wheele < 0)
        {
            for (int i = (int)wheele; i < 0; i++)
            {
                Inventory.instance.SelectPrevSlot();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.instance.SelectSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Inventory.instance.SelectSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Inventory.instance.SelectSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Inventory.instance.SelectSlot(3);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Inventory.instance.UseItem();
        }

    }

    private IEnumerator Crouch()
    {
        canCrouch = false;
        isCrouching = true;
        maxSpeed -= 2;
        float time = 0f;
        float t = 0f;

        while ( t < 1)
        {
            yield return null;
            time += Time.deltaTime;
            t = time / timeToCrouch;
            transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(1, 0.5f, 1), t);
        }

        yield return null;
        canCrouch = true;
        //timeToCrouch
        
        //transform.localScale = transform.localScale - new Vector3(0, crouchAmount, 0);
    }

    private IEnumerator UnCrouch()
    {
        canCrouch = false;
        float time = 0f;
        float t = 0f;

        while (t < 1)
        {
            yield return null;
            time += Time.deltaTime;
            t = time / timeToCrouch;
            transform.localScale = Vector3.Lerp(new Vector3(1, 0.5f, 1), new Vector3(1, 1, 1), t);
        }
        yield return null;
        maxSpeed += 2;
        canCrouch = true;
        //transform.localScale = transform.localScale + new Vector3(0, crouchAmount, 0);
        isCrouching = false ;
    }
    void Mover()
    {
        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.6f);
        //foreach (var hitCollider in hitColliders)
        //{
        //    var directionFromPlayer = transform.localEulerAngles;
        //    //hitCollider.SendMessage("AddDamage");
        //}
        /*
         * _rb.AddForce(
         * 
         */

        //if (Input.GetKey("w"))
        //{
        //    transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed); //move forward
        //}
        //if (Input.GetKey("s"))
        //{
        //    transform.Translate(Vector3.back * Time.deltaTime * movementSpeed); //move backwards
        //}
        //if (Input.GetKey("a"))
        //{
        //    transform.Translate(Vector3.left * Time.deltaTime * movementSpeed); //move left
        //}
        //if (Input.GetKey("d"))
        //{
        //    transform.Translate(Vector3.right * Time.deltaTime * movementSpeed); //move right
        //}
        if (Input.GetKey("w") && canJump)
        {
            rb.AddRelativeForce(Vector3.forward * movementSpeed, ForceMode.Force);
        }
        else if (rb.velocity.magnitude > 0f)
        {
            rb.AddRelativeForce(Vector3.forward * (-movementSpeed*10), ForceMode.Force);
        }
        if (Input.GetKey("s") && canJump)
        {
            rb.AddRelativeForce(Vector3.back * movementSpeed, ForceMode.Force);
        }
        else if (rb.velocity.magnitude > 0f)
        {
            rb.AddRelativeForce(Vector3.back * (-movementSpeed * 10), ForceMode.Force);
        }
        if (Input.GetKey("a") && canJump)
        {
            rb.AddRelativeForce(Vector3.left * movementSpeed, ForceMode.Force);
        }
        else if (rb.velocity.magnitude > 0f)
        {
            rb.AddRelativeForce(Vector3.left * (-movementSpeed * 10), ForceMode.Force);
        }
        if (Input.GetKey("d") && canJump)
        {
            rb.AddRelativeForce(Vector3.right * movementSpeed, ForceMode.Force);
        }
        else if (rb.velocity.magnitude > 0f)
        {
            rb.AddRelativeForce(Vector3.right * (-movementSpeed * 10), ForceMode.Force);
        }

        if (rb.velocity.magnitude > maxSpeed && canJump)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else if (rb.velocity.magnitude < 0f)
        {
            rb.velocity = rb.velocity.normalized * 0f;
        }

        if (Input.GetKey(KeyCode.Space) && canJump)
        {
            canJump = false;
            //rb.AddForce(transform.up * jumpForce);
            rb.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown("q"))
        {
            ShakeCamera(1.0f);

            //_playerCamera.GetComponent<Animator>().SetTrigger("ShakeCameraTrigger"); 
        }



        StartCoroutine(PlayerIsMoving());


        if (Input.GetKey("left shift") && _sprintBar.canSprint && !isSprinting && rb.velocity.magnitude > 0.1f && !isCrouching)
        {
            //StopCoroutine("PlayerWalkingAnim");
            //StartCoroutine(PlayerWalkingAnim(0.2f, 3f));
            StartSprinting();
        }
        else if ((!Input.GetKey("left shift")  || !_sprintBar.canSprint || rb.velocity.magnitude <= 0.1f || isCrouching) && isSprinting)
        {
            Debug.Log("Stop sprinting");
            //StopCoroutine("PlayerWalkingAnim");
            StopSprinting();
        }

        if (_isWalking && !_walkingAnimPlaying)
        {
            StartCoroutine(PlayerWalkingAnim());
        }
        
        if (_isWalking && !_stepSoundPlaying && !sneakItemUsed)
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

        if (Input.GetKey("c"))
        {
            if (!isCrouching && canCrouch)
            {
                StartCoroutine(Crouch());
            }
            else if (canCrouch)
            {
                StartCoroutine(UnCrouch());
            }

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
            else if (isCrouching)
            {
                time = 0.795f;
                speed = .75f;
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
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
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
        if (!sneakItemUsed)
        {
            if (canJump && _isWalking && isSprinting && !isCrouching)
            {
                var leftStep = _step;
                var rightStep = _step;
                leftStep.volume = 2f;
                rightStep.volume = 2f;
                _stepSoundPlaying = true;
                if (!_step.isPlaying)
                {
                    yield return new WaitForSeconds((0.4f) / 3f);
                    leftStep.Play();
                    yield return new WaitForSeconds((0.4f) / 2f);
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
            //else if (canJump && _isWalking && isCrouching)
            //{
            //    var leftStep = _step;
            //    var rightStep = _step;
            //    leftStep.volume = 0.5f;
            //    rightStep.volume = 0.5f;
            //    _stepSoundPlaying = true;
            //    if (!_step.isPlaying)
            //    {
            //        yield return new WaitForSeconds((0.8f) / 3f);
            //        leftStep.Play();
            //        yield return new WaitForSeconds((0.8f) / 2f);
            //        rightStep.Play();
            //        //_step.Play();
            //    }
            //    yield return new WaitForSeconds((0.8f) / 6f);
            //    //_step.Stop();
            //    leftStep.Stop();
            //    rightStep.Stop();
            //    _stepSoundPlaying = false;
            //    //if (!_step.isPlaying)
            //    //{
            //    StartCoroutine(WalkSound());
            //    //}
            //}
            else if (canJump && _isWalking)
            {
                var leftStep = _step;
                var rightStep = _step;
                leftStep.volume = 1f;
                rightStep.volume = 1f;
                if (isCrouching)
                {
                    leftStep.volume = 0.5f;
                    rightStep.volume = 0.5f;
                }
                _stepSoundPlaying = true;
                if (!_step.isPlaying)
                {
                    yield return new WaitForSeconds((0.6f) / 3f);
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
                //StartCoroutine(_resetWallsBar.StartSprinting());
                _resetWallsBar._isSprinting = true;
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
            //movementSpeed = movementSpeed * _sprintMultiplier;
            //movementSpeed += _baseSpeed;
            maxSpeed += 3;
            isSprinting = true;
            if (_sprintBar.canSprint)
            {
                _sprintBar._isSprinting = true;
                //StartCoroutine(_sprintBar.StartSprinting());
                //StartCoroutine(_sprintBar._startSprinting);
                //StartCoroutine(_sprintCoroutine);
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
            //movementSpeed = movementSpeed / _sprintMultiplier;
            //movementSpeed -= _baseSpeed;
            maxSpeed -= 3;
            isSprinting = false;
            _sprintBar._isSprinting = false;
            //if (_sprintBar.canSprint) 
            //{
            //    _sprintBar._isSprinting = false;
            //    //StartCoroutine(_sprintBar.StopSprinting());
            //    //StartCoroutine(_sprintBar._stopSprinting);
            //    //StartCoroutine(_stopSprintCoroutine);
            //}
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

    public void PlayerUseItem(int itemID)
    {
        switch (itemID)
        {
            case 0:
                Debug.Log("item 0 used, INCREASE SPEED");
                StartCoroutine(WaitForPlayerBaseSpeed());
                //StartCoroutine(IncreaseBaseSpeed());
                break;
            case 1:
                Debug.Log("item 1 used, PLAYER INVISIBLE");
                StartCoroutine(WaitForPlayerVisible());
                break;
            case 2:
                Debug.Log("item 2 used, REFILL SPRINT");
                RefillSprint();
                break;
            case 3:
                Debug.Log("item 3 used, REFILL WALLS BAR");
                StartCoroutine(RefillWalls());
                break;
            case 4:
                Debug.Log("item 4 used, SNEAK");
                StartCoroutine(WaitForNotSneaking());
                break;
            case 5:
                Debug.Log("item 5 used, RANDOM EFFECT");
                GetRandomItem();
                break;
            default:
                Debug.LogWarning("That item doesn't have a characteristic attached to it");
                break;
        }
    }

    private void GetRandomItem()
    {
        int itemID = Random.Range(0, Inventory.instance.GetNumberOfItems());
        Inventory.instance.PickUpItem(itemID);
    }
    private IEnumerator WaitForNotSneaking()
    {
        while (sneakItemUsed)
        {
            yield return null;
        }
        StartCoroutine(StartSneaking());
    }

    private IEnumerator StartSneaking()
    {
        sneakItemUsed = true;
        yield return new WaitForSeconds(5);
        sneakItemUsed = false;
    }

    private IEnumerator WaitForPlayerBaseSpeed()
    {
        while (speedItemUsed)
        {
            yield return null;
        }
        StartCoroutine(IncreaseBaseSpeed());
    }
    private IEnumerator IncreaseBaseSpeed()
    {
        speedItemUsed = true;
        maxSpeed += 2;
        //movementSpeed += 2;
        float currentSpeed = movementSpeed;
        Debug.Log("item 0 used, new speed = " + movementSpeed);
        yield return new WaitForSeconds(5);
        //movementSpeed -= 2;
        maxSpeed -= 2;
        speedItemUsed = false;
        Debug.Log("item 0 used, final speed = " + movementSpeed);
    }

    private IEnumerator WaitForPlayerVisible()
    {
        while (playerInvisible)
        {
            yield return null;
        }
        StartCoroutine(GoInvisible());
    }

    private IEnumerator GoInvisible()
    {
        Debug.Log("Player Invisible");
        playerInvisible = true;
        yield return new WaitForSeconds(5);
        playerInvisible = false;
        Debug.Log("Player Visible");
    }
    private IEnumerator RefillWalls()
    {
        while (!BigMazeGenerator.Instance._canResetWalls)
        {
            yield return null;
        }    

        _resetWallsBar.RefillBar();
    }
    private void RefillSprint()
    {
        //if (isSprinting)
        //{
        //    StopCoroutine(_sprintCoroutine);
        //}
        //else
        //{
        //    StopCoroutine(_stopSprintCoroutine);
        //}
        //Debug.Log("Coroutines stopped");

        //StartCoroutine(_sprintBar.RefillBar());
        _sprintBar.RefillBar();

        //if (isSprinting)
        //{
        //    isSprinting = false;
        //    StartSprinting();
        //}
        //if (isSprinting)
        //{
        //    StopSprinting();
        //    StartCoroutine(_sprintBar.RefillBar());
        //    StartSprinting();
        //}
        //else
        //{
        //    StartCoroutine(_sprintBar.RefillBar());
        //}
    }

}
