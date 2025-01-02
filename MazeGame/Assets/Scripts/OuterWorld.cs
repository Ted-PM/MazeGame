using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OuterWorld : MonoBehaviour
{
    //public List<Texture> m_Textures;

    [SerializeField]
    private GameObject _eyePrefab;

    [SerializeField]
    private GameObject _eyeParent;
    //private List<GameObject> _eyeList;

    public bool canRaise = false;
    public bool canLower = false;

    private Vector3 _endPosition;
    private Vector3 _startPosition;


    private void Start()
    {

        _startPosition = transform.localPosition;
        _endPosition = _startPosition + new Vector3(0, -9.9f, 0);
        canLower = true;
        canRaise = false;

        SpawnEyes();
    }

    private void SpawnEyes()
    {
        int width = BigMazeGenerator.Instance.GetMazeWidth();
        int depth = BigMazeGenerator.Instance.GetMazeDepth();

        for (int i = 0;  i < width; i++)
        {
            i+=5;
            var tempEye = Instantiate(_eyePrefab, new Vector3 (i*10, 40, -30), Quaternion.identity, _eyeParent.transform);
            tempEye.transform.localScale = new Vector3(500, 500, 500);
            var tempEye2 = Instantiate(_eyePrefab, new Vector3(i * 10, 40, depth*10+30), Quaternion.identity, _eyeParent.transform);
            tempEye2.transform.localScale = new Vector3(500, 500, 500);
        }
        for (int i = 0; i < depth; i++)
        {
            i+=5;
            var tempEye = Instantiate(_eyePrefab, new Vector3(-30, 40, i*10), Quaternion.identity, _eyeParent.transform);
            tempEye.transform.localScale = new Vector3(500, 500, 500);
            var tempEye2 = Instantiate(_eyePrefab, new Vector3(width*10 + 30, 40, i*10), Quaternion.identity, _eyeParent.transform);
            tempEye2.transform.localScale = new Vector3(500, 500, 500);
        }
    }

    public IEnumerator LowerCeeling(float lowerTime) //Vector3 endPosition, 
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
        }
        else
        {
            yield return null;
        }
    }

    public IEnumerator RaiseCeeling(float raiseTime)
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
        }
        else
        {
            yield return null;
        }
    }
}
