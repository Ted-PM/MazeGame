using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;
    [HideInInspector]
    public bool _leftVisited { get; private set; }

    [SerializeField]
    private GameObject _leftDoor;
    [HideInInspector]
    public bool _leftDoorAdded { get; private set; }

    [SerializeField]
    private GameObject _leftCrouchWall;
    [HideInInspector]
    public bool _leftCrouchWallAdded { get; private set; }

    [SerializeField]
    private GameObject _rightWall;
    [HideInInspector]
    public bool _rightVisited { get; private set; }

    [SerializeField]
    private GameObject _rightDoor;
    [HideInInspector]
    public bool _rightDoorAdded { get; private set; }
    [SerializeField]
    private GameObject _rightCrouchWall;
    [HideInInspector]
    public bool _rightCrouchWallAdded { get; private set; }


    [SerializeField]
    private GameObject _frontWall;
    [HideInInspector]
    public bool _frontVisited { get; private set; }

    [SerializeField]
    private GameObject _frontDoor;
    [HideInInspector]
    public bool _frontDoorAdded { get; private set; }
    [SerializeField]
    private GameObject _frontCrouchWall;
    [HideInInspector]
    public bool _frontCrouchWallAdded { get; private set; }


    [SerializeField]
    private GameObject _backWall;
    [HideInInspector]
    public bool _backsVisited { get; private set; }

    [SerializeField]
    private GameObject _backDoor;
    [HideInInspector]
    public bool _backDoorAdded { get; private set; }
    [SerializeField]
    private GameObject _backCrouchWall;
    [HideInInspector]
    public bool _backCrouchWallAdded { get; private set; }


    //[SerializeField]
    //private GameObject _unvisitedBlock;

    // corner balls for corners?
    [SerializeField]
    private GameObject _frontLeftCorner;

    [SerializeField]
    private GameObject _frontRightCorner;

    [SerializeField]
    private GameObject _backLeftCorner;

    [SerializeField]
    private GameObject _backRightCorner;


    [SerializeField]
    private Material _edgeMaterial;

    [SerializeField]
    private float _ceelingHeight;

    [SerializeField]
    private GameObject _pathToEndIndicator;

    public bool isVisited {  get; private set; }

    public bool isRightEnd { get; private set; }

    public bool isFrontEnd { get; private set; }

    private Vector3 endPosition = new Vector3(0, -0.5f, 0);

    private void Start()
    {
        _pathToEndIndicator.SetActive(false);
    }

    private void DisableDoors()
    {
        _leftDoor.SetActive(false);
        _rightDoor.SetActive(false);
        _frontDoor.SetActive(false);
        _backDoor.SetActive(false);
    }

    private void DisableAllCrouchWalls()
    {
        _leftCrouchWall.SetActive(false);
        _rightCrouchWall.SetActive(false);
        _frontCrouchWall.SetActive(false);
        _backCrouchWall.SetActive(false);
    }

    public void EnablePathToEnd()
    {
        //yield return new WaitForSeconds(0.05f);
        //Debug.Log("Path enabled: " + transform.position);
        //WaitTheDisplayEnd();
        _pathToEndIndicator.SetActive(true);
        //yield return null;
    }

    //private IEnumerator WaitTheDisplayEnd()
    //{
    //    yield return new WaitForSeconds(0.05f);
    //    _pathToEndIndicator.SetActive(true);
    //}


    public void DisablePathToEnd()
    {
        _pathToEndIndicator.SetActive(false);
    }
    //private void Awake()
    //{
    //    _leftWall = GameObject.Find("LeftWall").GetComponentInChildren<MazeWall>();
    //    _rightWall = GameObject.Find("RightWall").GetComponentInChildren<MazeWall>();
    //    _frontWall = GameObject.Find("FrontWall").GetComponentInChildren<MazeWall>();
    //    _backWall = GameObject.Find("BackWall").GetComponentInChildren<MazeWall>();
    //}
    public void Visit()
    {
        isVisited = true;
        DisableDoors();
        DisableAllCrouchWalls();
        //_unvisitedBlock.SetActive(false);
    }

    public void SetCellMaterial(Material material, int mazeWidth, int mazeDepth)
    {
        _leftWall.GetComponentInChildren<MazeWall>().SetMaterial(material);
        _rightWall.GetComponentInChildren<MazeWall>().SetMaterial(material);
        _backWall.GetComponentInChildren<MazeWall>().SetMaterial(material);
        _frontWall.GetComponentInChildren<MazeWall>().SetMaterial(material);

        ChangeEdgeMaterial(mazeWidth, mazeDepth);
    }

    private void ChangeEdgeMaterial(int mazeWidth, int mazeDepth)
    {
        // is left wall
        if (transform.position.x == 0)
        {
            _leftWall.GetComponentInChildren<MazeWall>().SetMaterial(_edgeMaterial);
            //_leftWall.transform.localScale = _leftWall.transform.localScale + new Vector3(0f, _ceelingHeight, 0f);
            _leftWall.GetComponentInChildren<MazeWall>().transform.localScale = _leftWall.GetComponentInChildren<MazeWall>().transform.localScale + new Vector3(0f,5*_ceelingHeight, 0f);
            _leftWall.GetComponentInChildren<MazeWall>().transform.localPosition = _leftWall.GetComponentInChildren<MazeWall>().transform.localPosition + new Vector3(0f,2.5f*_ceelingHeight, 0f);
            //_leftWall.transform.localPosition = _leftWall.transform.localPosition + new Vector3(0f, 5f*_ceelingHeight, 0f);

            // balls to left
            _backLeftCorner.SetActive(false);
            _frontLeftCorner.SetActive(false);
        }
        //if (transform.position.x == mazeWidth - 1)
        // is right wall
        if ((transform.position.x/10) == mazeWidth - 1)
        {
            _rightWall.GetComponentInChildren<MazeWall>().SetMaterial(_edgeMaterial);
            //_rightWall.transform.localScale = _rightWall.transform.localScale + new Vector3(0f, _ceelingHeight, 0f);
            _rightWall.GetComponentInChildren<MazeWall>().transform.localScale = _rightWall.GetComponentInChildren<MazeWall>().transform.localScale + new Vector3(0f, 5 * _ceelingHeight, 0f);
            _rightWall.GetComponentInChildren<MazeWall>().transform.localPosition = _rightWall.GetComponentInChildren<MazeWall>().transform.localPosition + new Vector3(0f,2.5f*_ceelingHeight, 0f);
            //_rightWall.transform.localPosition = _rightWall.transform.localPosition + new Vector3(0f, 5f * _ceelingHeight, 0f);

            // balls to right
            _backRightCorner.SetActive(false);
            _frontRightCorner.SetActive(false);
        }
        // is back wall
        if (transform.position.z == 0)
        {
            _backWall.GetComponentInChildren<MazeWall>().SetMaterial(_edgeMaterial);
            _backWall.GetComponentInChildren<MazeWall>().transform.localScale = _backWall.GetComponentInChildren<MazeWall>().transform.localScale + new Vector3(0f, 5 * _ceelingHeight, 0f);
            _backWall.GetComponentInChildren<MazeWall>().transform.localPosition = _backWall.GetComponentInChildren<MazeWall>().transform.localPosition + new Vector3(0f, 2.5f * _ceelingHeight, 0f);

            //_backWall.transform.localScale = _backWall.transform.localScale + new Vector3(0f, _ceelingHeight, 0f);
            //_backWall.transform.localPosition = _backWall.transform.localPosition + new Vector3(0f, 5f * _ceelingHeight, 0f);

            // balls at back
            _backRightCorner.SetActive(false);
            _backLeftCorner.SetActive(false);
        }
        // balls at front
        //if (transform.position.z == mazeDepth - 1)
        if ((transform.position.z/10) == mazeDepth - 1)
        {
            _frontWall.GetComponentInChildren<MazeWall>().SetMaterial(_edgeMaterial);
            _frontWall.GetComponentInChildren<MazeWall>().transform.localScale = _frontWall.GetComponentInChildren<MazeWall>().transform.localScale + new Vector3(0f, 5 * _ceelingHeight, 0f);
            _frontWall.GetComponentInChildren<MazeWall>().transform.localPosition = _frontWall.GetComponentInChildren<MazeWall>().transform.localPosition + new Vector3(0f, 2.5f * _ceelingHeight, 0f);

            //_frontWall.transform.localScale = _frontWall.transform.localScale + new Vector3(0f, _ceelingHeight, 0f);
            //_frontWall.transform.localPosition = _frontWall.transform.localPosition + new Vector3(0f, 5f * _ceelingHeight, 0f);

            // disable front corner balls
            _frontLeftCorner.SetActive(false);
            _frontRightCorner.SetActive(false);
        }
    }

    public void ClearLeftWall()
    {
        _leftVisited = true;
        //_leftWall.SetActive(false);
        Destroy(_leftWall);
    }

    public bool GetLeftWallStatus() { return _leftVisited; }

    public void AddLeftDoor()
    {
        _leftDoorAdded = true;
        _leftDoor.SetActive(true);
    }

    public void AddLeftCrouchWall()
    {
        _leftCrouchWallAdded = true;
        _leftCrouchWall.SetActive(true);
    }

    public void ClearRightWall()
    {
        _rightVisited = true;
        //_rightWall.SetActive(false);
        Destroy( _rightWall );
    }

    public bool GetRightWallStatus() { return _rightVisited; }

    public void AddRightDoor()
    {
        _rightDoorAdded = true;
        _rightDoor.SetActive(true);
    }

    public void AddRightCrouchWall()
    {
        _rightCrouchWallAdded = true;
        _rightCrouchWall.SetActive(true);
    }

    public void ClearFrontWall()
    {
        _frontVisited = true;
        //_frontWall.SetActive(false);
        Destroy (_frontWall );
    }

    public bool GetFrontWallStatus() { return _frontVisited; }

    public void AddFrontDoor()
    {
        _frontDoorAdded = true;
        _frontDoor.SetActive(true);
    }

    public void AddFrontCrouchWall()
    {
        _frontCrouchWallAdded = true;
        _frontCrouchWall.SetActive(true);
    }

    public void ClearBackWall()
    {
        _backsVisited = true;
        //_backWall.SetActive(false);
        Destroy ( _backWall );
    }

    public bool GetBackWallStatus() { return _backsVisited; }

    public void AddBackDoor()
    {
        _backDoorAdded = true;
        _backDoor.SetActive(true);
    }

    public void AddBackCrouchWall()
    {
        _backCrouchWallAdded = true;
        _backCrouchWall.SetActive(true);
    }

    public void DestroyUnactiveCrouchWalls()
    {
        if (!_leftCrouchWallAdded)
        {
            Destroy(_leftCrouchWall);
        }
        if (!_rightCrouchWallAdded)
        {
            Destroy(_rightCrouchWall);
        }
        if (!_frontCrouchWallAdded)
        {
            Destroy(_frontCrouchWall);
        }
        if (!_backCrouchWallAdded)
        {
            Destroy(_backCrouchWall);
        }
    }
    public void DestroyUnactiveDoors()
    {
        if (!_leftDoorAdded)
        {
            Destroy(_leftDoor);
        }
        if (!_rightDoorAdded)
        {
            Destroy(_rightDoor);
        }
        if (!_frontDoorAdded)
        {
            Destroy(_frontDoor);
        }
        if (!_backDoorAdded)
        {
            Destroy(_backDoor);
        }
    }

    public void ClearAll(int mazeWidth, int mazeDepth, float lowerTime)
    {
        if (_leftVisited == false)// && transform.position.x > 0)
        {
            _leftWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_leftWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _leftWall.GetComponent<BoxCollider>()));
            //_leftWall.LowerWall(lowerTime);
            //_leftWall.SetActive(false);
        }
        else if (_leftDoorAdded == true)// && transform.position.x > 0)
        {
            _leftDoor.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_leftDoor.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _leftDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_leftCrouchWallAdded == true)
        {
            _leftCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_leftCrouchWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _leftCrouchWall.GetComponentInChildren<MeshCollider>()));
        }
        //if (_rightVisited == false && transform.position.x < mazeWidth - 1)
        if (_rightVisited == false)// && transform.position.x < (mazeWidth*10) - 10)
        {
            _rightWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_rightWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _rightWall.GetComponent<BoxCollider>()));
            //_rightWall.LowerWall(lowerTime);
            //_rightWall.gameObject.SetActive(false);
        }
        else if (_rightDoorAdded == true)// && transform.position.x < (mazeWidth * 10) - 10)
        {
            _rightDoor.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_rightDoor.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _rightDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_rightCrouchWallAdded == true)
        {
            _rightCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_rightCrouchWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _rightCrouchWall.GetComponentInChildren<MeshCollider>()));
        }
        //if (_frontVisited == false && transform.position.z < mazeDepth - 1)
        if (_frontVisited == false)// && transform.position.z < (mazeDepth*10) - 10)
        {
            _frontWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_frontWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _frontWall.GetComponent<BoxCollider>()));
            //_frontWall.LowerWall(lowerTime);
            //_frontWall.gameObject.SetActive(false);
        }
        else if (_frontDoorAdded == true)// && transform.position.z < (mazeDepth * 10) - 10)
        {
            _frontDoor.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_frontDoor.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _frontDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_frontCrouchWallAdded == true)
        {
            _frontCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_frontCrouchWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _frontCrouchWall.GetComponentInChildren<MeshCollider>()));
        }
        if (_backsVisited == false)// && transform.position.z > 0)
        {
            _backWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_backWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _backWall.GetComponent<BoxCollider>()));
            //_backWall.LowerWall(lowerTime);
            //_backWall.gameObject.SetActive(false);
        }
        else if (_backDoorAdded == true)// && transform.position.z > 0)
        {
            _backDoor.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_backDoor.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _backDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_backCrouchWallAdded == true)
        {
            _backCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_backCrouchWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime, _backCrouchWall.GetComponentInChildren<MeshCollider>()));
        }

        if (_backLeftCorner.activeSelf)
        {
            _backLeftCorner.GetComponent<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_backLeftCorner.GetComponent<MazeWall>().LowerWall(lowerTime, _backLeftCorner.GetComponent<CapsuleCollider>()));
        }
        if (_backRightCorner.activeSelf)
        {
            _backRightCorner.GetComponent<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_backRightCorner.GetComponent<MazeWall>().LowerWall(lowerTime, _backRightCorner.GetComponent<CapsuleCollider>()));
        }
        if (_frontLeftCorner.activeSelf)
        {
            _frontLeftCorner.GetComponent<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_frontLeftCorner.GetComponent<MazeWall>().LowerWall(lowerTime, _frontLeftCorner.GetComponent<CapsuleCollider>()));
        }
        if (_frontRightCorner.activeSelf)
        {
            _frontRightCorner.GetComponent<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_frontRightCorner.GetComponent<MazeWall>().LowerWall(lowerTime, _frontRightCorner.GetComponent<CapsuleCollider>()));
        }
    }

    public void HasEnd(bool right)
    {
        if (right)
        {
            _rightWall.SetActive(false);
            //_rightWall.GetComponentInChildren<MeshRenderer>().enabled = false;
            //_rightWall.tag = "End";
            //---
            //_rightWall.GetComponent<BoxCollider>().enabled = false;
            isRightEnd = true;
        }
        else
        {
            _frontWall.SetActive(false);
            //_frontWall.GetComponentInChildren<MeshRenderer>().enabled = false;
            //_frontWall.tag = "End";
            //---
            //_frontWall.GetComponent<BoxCollider>().enabled = false;
            isFrontEnd = true;
        }
    }

    public void ResetAll(int mazeWidth, int mazeDepth, float raiseTime)
    {
        if (_leftVisited == false)// && transform.position.x > 0)
        {
            _leftWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_leftWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _leftWall.GetComponent<BoxCollider>()));
            //_leftWall.RaiseWall(raiseTime);
            //_leftWall.gameObject.SetActive(true);
        }
        else if (_leftDoorAdded == true)// && transform.position.x > 0)
        {
            _leftDoor.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_leftDoor.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _leftDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_leftCrouchWallAdded == true)
        {
            _leftCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_leftCrouchWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _leftCrouchWall.GetComponentInChildren<MeshCollider>()));
        }
        //if (_rightVisited == false && transform.position.x < mazeWidth - 1)
        if (_rightVisited == false)// && transform.position.x < mazeWidth * 10 - 1)
        {
            _rightWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_rightWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _rightWall.GetComponent<BoxCollider>()));
            //_rightWall.RaiseWall(raiseTime);
            //_rightWall.gameObject.SetActive(true);
        }
        else if (_rightDoorAdded == true)// && transform.position.x < (mazeWidth * 10) - 10)
        {
            _rightDoor.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_rightDoor.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _rightDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_rightCrouchWallAdded == true)
        {
            _rightCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_rightCrouchWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _rightCrouchWall.GetComponentInChildren<MeshCollider>()));
        }
        //if (_frontVisited == false && transform.position.z < mazeDepth - 1)
        if (_frontVisited == false)// && transform.position.z < mazeDepth* 10 - 1)
        {
            _frontWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_frontWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _frontWall.GetComponent<BoxCollider>()));
            //_frontWall.RaiseWall(raiseTime);
            //_frontWall.gameObject.SetActive(true);
        }
        else if (_frontDoorAdded == true)// && transform.position.z < (mazeDepth * 10) - 10)
        {
            _frontDoor.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_frontDoor.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _frontDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_frontCrouchWallAdded == true)
        {
            _frontCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_frontCrouchWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _frontCrouchWall.GetComponentInChildren<MeshCollider>()));
        }
        if (_backsVisited == false)// && transform.position.z > 0)
        {
            _backWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_backWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _backWall.GetComponent<BoxCollider>()));
            //_backWall.RaiseWall(raiseTime);
            //_backWall.gameObject.SetActive(true);
        }
        else if (_backDoorAdded == true)// && transform.position.z > 0)
        {
            _backDoor.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_backDoor.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _backDoor.GetComponentInChildren<MeshCollider>()));
        }
        else if (_backCrouchWallAdded == true)
        {
            _backCrouchWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_backCrouchWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime, _backCrouchWall.GetComponentInChildren<MeshCollider>()));
        }

        if (_backLeftCorner.activeSelf)
        {
            _backLeftCorner.GetComponent<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_backLeftCorner.GetComponent<MazeWall>().RaiseWall(raiseTime, _backLeftCorner.GetComponent<CapsuleCollider>()));
        }
        if (_backRightCorner.activeSelf)
        {
            _backRightCorner.GetComponent<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_backRightCorner.GetComponent<MazeWall>().RaiseWall(raiseTime, _backRightCorner.GetComponent<CapsuleCollider>()));
        }
        if (_frontLeftCorner.activeSelf)
        {
            _frontLeftCorner.GetComponent<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_frontLeftCorner.GetComponent<MazeWall>().RaiseWall(raiseTime, _frontLeftCorner.GetComponent<CapsuleCollider>()));
        }
        if (_frontRightCorner.activeSelf)
        {
            _frontRightCorner.GetComponent<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_frontRightCorner.GetComponent<MazeWall>().RaiseWall(raiseTime, _frontRightCorner.GetComponent<CapsuleCollider>()));
        }
    }
}
