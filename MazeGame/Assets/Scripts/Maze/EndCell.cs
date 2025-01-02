using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _bottomWall;
    [SerializeField]
    private GameObject _leftWall;

    public void DisableLeft()
    {
        _leftWall.SetActive(false);
    }

    public void DisableBottom()
    {
        _bottomWall.SetActive(false);
    }
}
