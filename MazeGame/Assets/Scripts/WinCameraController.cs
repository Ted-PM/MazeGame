using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _centerPoint;
    [SerializeField]
    private Transform _start;
    [SerializeField]
    private Transform _end;

    bool side;
    // Start is called before the first frame update
    void Start()
    {
        //const int initialSeed = 1234;

        //Random.InitState(initialSeed); // cannot be retrieved

        //_start = _end = transform;
        ////_end = transform;
        //_start.position = new Vector3(0, 0, 0);
        //_end.position = new Vector3(0, 0, 0) ;
        //_end = transform;
        side = true;
        GetStartAndEndPosition();
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    private void FixedUpdate()
    {
        transform.LookAt(_centerPoint);
    }

    private void GetStartAndEndPosition()
    {

        //Random.State newState = Random.state;

        //_start.position = Random.insideUnitSphere * 5;

        float startX = Random.Range(-4.5f, 4.9f);
        float endX = Random.Range(-4.5f, 4.9f);

        float startY = Random.Range(1, 8);
        float endY = Random.Range(1, 8);

        if (side)
        {
            _start.position = new Vector3(startX, startY, -4.5f);
            _end.position = new Vector3(endX, endY, -4.5f);
            side = false;
        }
        else
        {
            _start.position = new Vector3(-4.5f, startY, startX);
            _end.position = new Vector3(-4.5f, endY, endX);
            side = true;
        }
        //while (_start.position.x > -1f && _start.position.x < 1f)
        //{
        //    _start.position = new Vector3(Random.Range(-4, 4), _start.position.y, -5f);
        //}


        //while (_start.position == _end.position)
        //{
        //    _end.position = new Vector3(Random.Range(-4, 4), Random.Range(1, 4), Random.Range(-4, 4));
        //}

        //while (_end.position.x > -1f && _end.position.x < 1f)
        //{
        //    _end.position = new Vector3(Random.Range(-4, 4), _end.position.y, -5f);
        //}
        //_end.position = Random.insideUnitSphere * 5;
        //_end.position = Random.insideUnitSphere * 5;
        Debug.Log("start: " + _start.position + ", end: " + _end.position);


        StartCoroutine(CameraPan());
    }

    private IEnumerator CameraPan()
    {
        transform.position = _start.position;
        transform.LookAt(_centerPoint);
        float time = 0f;
        float t = 0f;

        while (t < 1)
        {
            yield return null;
            time += Time.deltaTime;
            t = time / 4f;
            transform.position = Vector3.Lerp(_start.position, _end.position, t);
        }
        yield return null;
        GetStartAndEndPosition();
    }
}
