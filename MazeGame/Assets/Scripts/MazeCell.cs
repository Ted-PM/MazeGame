using System.Collections;
using System.Collections.Generic;
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

    public bool isVisited {  get; private set; }

    public bool isRightEnd { get; private set; }
    public bool isFrontEnd { get; private set; }

    public void Visit()
    {
        isVisited = true;
        _unvisitedBlock.SetActive(false);
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

    public void ClearAll(int mazeWidth, int mazeDepth)
    {
        if (_leftVisited == false && transform.position.x > 0)
        {
            _leftWall.SetActive(false);
        }
        if (_rightVisited == false && transform.position.x < mazeWidth - 1)
        {
            _rightWall.SetActive(false);
        }
        if (_frontVisited == false && transform.position.z < mazeDepth - 1)
        {
            _frontWall.SetActive(false);
        }
        if(_backsVisited == false && transform.position.z > 0)
        {
            _backWall.SetActive(false);
        }

        //if (isRightEnd)
        //{
        //    _rightWall.SetActive(true);
        //}
        //if (isFrontEnd)
        //{
        //    _frontWall.SetActive(true);
        //}
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

    //public void AddRightEnd()
    //{
    //    _rightWall.SetActive(true);
    //}

    //public void AddFrontEnd()
    //{
    //    _frontWall.SetActive(true);
    //}

    //public void ResetRightEnd()
    //{
    //    _rightWall.SetActive(true);
    //}

    //public void ResetFrontEnd()
    //{
    //    _frontWall.SetActive(true);
    //}

    public void ResetAll(int mazeWidth, int mazeDepth)
    {
        if (_leftVisited == false && transform.position.x > 0)
        {
            _leftWall.SetActive(true);
        }
        if (_rightVisited == false && transform.position.x < mazeWidth - 1)
        {
            _rightWall.SetActive(true);
        }
        if (_frontVisited == false && transform.position.z < mazeDepth - 1)
        {
            _frontWall.SetActive(true);
        }
        if (_backsVisited == false && transform.position.z > 0)
        {
            _backWall.SetActive(true);
        }

        //if (isRightEnd)
        //{
        //    _rightWall.SetActive(false);
        //}
        //if (isFrontEnd)
        //{
        //    _frontWall.SetActive(false);
        //}
    }
}
