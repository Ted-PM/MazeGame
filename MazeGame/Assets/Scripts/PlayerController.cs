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

    Collider _collider;

    bool canJump;

    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        canJump = true;
        //rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        lookAtMouse();
        Mover();
    }

    void Mover()
    {
        if (Input.GetKey("w") && !rb.isKinematic)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed); //move forward
        }
        if (Input.GetKey("s") && !rb.isKinematic)
        {
            transform.Translate(Vector3.back * Time.deltaTime * movementSpeed); //move backwards
        }
        if (Input.GetKey("a") && !rb.isKinematic)
        {
            transform.Translate(Vector3.left * Time.deltaTime * movementSpeed); //move left
        }
        if (Input.GetKey("d") && !rb.isKinematic)
        {
            transform.Translate(Vector3.right * Time.deltaTime * movementSpeed); //move right
        }
        if (Input.GetKey(KeyCode.Space) && canJump && !rb.isKinematic)
        {
            canJump = false;
            rb.AddForce(transform.up * jumpForce);
        }

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
