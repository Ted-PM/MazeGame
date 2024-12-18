using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;
    public bool _leftVisited { get; private set; }

    [SerializeField]
    private GameObject _rightWall;
    public bool _rightVisited { get; private set; }


    [SerializeField]
    private GameObject _frontWall;
    public bool _frontVisited { get; private set; }


    [SerializeField]
    private GameObject _backWall;
    public bool _backsVisited { get; private set; }


    [SerializeField]
    private GameObject _unvisitedBlock;

    [SerializeField]
    private Material _edgeMaterial;

    public bool isVisited {  get; private set; }

    public bool isRightEnd { get; private set; }
    public bool isFrontEnd { get; private set; }

    private Vector3 endPosition = new Vector3(0, -0.5f, 0);

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
        _unvisitedBlock.SetActive(false);
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
        }
        if (transform.position.x == mazeWidth - 1)
        {
            _rightWall.GetComponentInChildren<MazeWall>().SetMaterial(_edgeMaterial);
        }
        if (transform.position.z == 0)
        {
            _backWall.GetComponentInChildren<MazeWall>().SetMaterial(_edgeMaterial);
        }
        if (transform.position.z == mazeDepth - 1)
        {
            _frontWall.GetComponentInChildren<MazeWall>().SetMaterial(_edgeMaterial);
        }
    }

    public void ClearLeftWall()
    {
        _leftVisited = true;
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightVisited = true;
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontVisited = true;
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backsVisited = true;
        _backWall.SetActive(false);
    }

    public void ClearAll(int mazeWidth, int mazeDepth, float lowerTime)
    {
        if (_leftVisited == false && transform.position.x > 0)
        {
            _leftWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_leftWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime));
            //_leftWall.LowerWall(lowerTime);
            //_leftWall.SetActive(false);
        }
        if (_rightVisited == false && transform.position.x < mazeWidth - 1)
        {
            _rightWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_rightWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime));
            //_rightWall.LowerWall(lowerTime);
            //_rightWall.gameObject.SetActive(false);
        }
        if (_frontVisited == false && transform.position.z < mazeDepth - 1)
        {
            _frontWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_frontWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime));
            //_frontWall.LowerWall(lowerTime);
            //_frontWall.gameObject.SetActive(false);
        }
        if(_backsVisited == false && transform.position.z > 0)
        {
            _backWall.GetComponentInChildren<MazeWall>().StopCoroutine("RaiseWall");
            StartCoroutine(_backWall.GetComponentInChildren<MazeWall>().LowerWall(lowerTime));
            //_backWall.LowerWall(lowerTime);
            //_backWall.gameObject.SetActive(false);
        }
    }

    public void HasEnd(bool right)
    {
        if (right)
        {
            isRightEnd = true;
        }
        else
        {
            isFrontEnd = true;
        }
    }

    public void ResetAll(int mazeWidth, int mazeDepth, float raiseTime)
    {
        if (_leftVisited == false && transform.position.x > 0)
        {
            _leftWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_leftWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime));
            //_leftWall.RaiseWall(raiseTime);
            //_leftWall.gameObject.SetActive(true);
        }
        if (_rightVisited == false && transform.position.x < mazeWidth - 1)
        {
            _rightWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_rightWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime));
            //_rightWall.RaiseWall(raiseTime);
            //_rightWall.gameObject.SetActive(true);
        }
        if (_frontVisited == false && transform.position.z < mazeDepth - 1)
        {
            _frontWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_frontWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime));
            //_frontWall.RaiseWall(raiseTime);
            //_frontWall.gameObject.SetActive(true);
        }
        if (_backsVisited == false && transform.position.z > 0)
        {
            _backWall.GetComponentInChildren<MazeWall>().StopCoroutine("LowerWall");
            StartCoroutine(_backWall.GetComponentInChildren<MazeWall>().RaiseWall(raiseTime));
            //_backWall.RaiseWall(raiseTime);
            //_backWall.gameObject.SetActive(true);
        }
    }
}
