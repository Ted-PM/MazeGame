using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeWall : MonoBehaviour
{
    private NavMeshObstacle _navMeshObstacle;
    public bool canRaise = false;
    public bool canLower = false;

    private Vector3 _endPosition;
    private Vector3 _startPosition;


    private void Start()
    {
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _startPosition = transform.localPosition;
        _endPosition = _startPosition + new Vector3(0, -9.9f, 0);
        canLower = true;
        canRaise = false;
    }

    public IEnumerator LowerWall(float lowerTime) //Vector3 endPosition, 
    {
        //Debug.Log("cuurent pos: " + transform.localPosition.y);
        //Debug.Log("goal pos: " + _endPosition.y);

        if (canLower)
        {
            //StopCoroutine("RaiseWall");
            yield return 0;
            Vector3 startPosition = transform.localPosition;
            canLower = false;
            canRaise = true;
            float time = 0f;
            float t = 0;
            //_endPosition = _startPosition + endPosition;

            while (t < 1)
            {
                yield return null;
                time += Time.deltaTime;
                t = time / lowerTime;

                transform.localPosition = Vector3.Lerp(startPosition, _endPosition, t);
            }

            _navMeshObstacle.carving = false;
        }
        else
        {
            yield return null;
        }
    }

    public IEnumerator RaiseWall(float raiseTime)
    {
        //Debug.Log("cuurent pos: " + transform.localPosition.y);
        //Debug.Log("goal pos: " + _endPosition.y);

        if (canRaise)
        {
            //StopCoroutine("LowerWall");
            yield return 0;

            canLower = true;
            canRaise = false;
            float time = 0f;
            float t = 0;

            Vector3 currentPosition = transform.localPosition;

            while (t < 1)
            {
                yield return null;
                time += Time.deltaTime;
                t = time / raiseTime;

                transform.localPosition = Vector3.Lerp(currentPosition, _startPosition, t);
            }

            _navMeshObstacle.carving = true;
        }
        else
        {
            yield return null;
        }
    }

    public void SetMaterial(Material newMaterial)
    {
        GetComponent<MeshRenderer>().material = newMaterial;
    }
}
