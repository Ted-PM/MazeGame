using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// for random sorting of list ?
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.Playables;

public class BigMazeGenerator : MonoBehaviour
{
    public static BigMazeGenerator Instance;

    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private EnemyController _enemyControllerPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private List<Material> _materials;

    private MazeCell[,] _mazeGrid;

    // index of end cell in maze
    private int[] _endCell;

    private int[,] _wallsToBreak;

    [SerializeField]
    private int _numWallsToBreak;
    public bool _canResetWalls { get; private set; }

    private bool _wallsMoving = false;

    public bool _wallsAreUp;

    [SerializeField]
    private float _timeToGenerateWalls;

    [SerializeField]
    private float _timeToLowerWalls;

    [SerializeField]
    private float _timeToRaiseWalls;

    private int visitedCellCount = 0;

    [SerializeField]
    private float _enemyFallHeight;

    private List<EnemyController> _enemyList;

    private void Awake()
    {
        Instance = this;
    }
    //IEnumerator Start()
    void Start()
    {
        _wallsAreUp = true;

        _endCell = new int[2];

        _endCell = GetEndCell();

        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        _wallsToBreak = new int[2, _numWallsToBreak];//(int)(_mazeWidth + _mazeDepth) / 10];

        _wallsToBreak = GetWallsToBreak();

        _enemyList = new List<EnemyController>();

        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeDepth; j++)
            {
                _mazeGrid[i, j] = Instantiate(_mazeCellPrefab, new Vector3((float)(i*10), 0, (float)(j * 10)), Quaternion.identity);
            }
        }

        // start at first cell
        //yield return GenerateMaze(null, _mazeGrid[0, 0], _endCell, _wallsToBreak);
        GenerateMaze(null, _mazeGrid[0, 0], _endCell, _wallsToBreak);
        _canResetWalls = true;
        
    }

    private int[] GetEndCell()
    {
        // 
        int[] endCell = new int[2];

        // 0 if end on right wall (x = _mazeDepth - 1)
        // 1 if end of front wall (z = _mazeWidth - 1)
        int endBorder = Random.Range(0, 2);

        if (endBorder == 0)
        {
            endCell[0] = _mazeWidth - 1;
            endCell[1] = Random.Range(0, _mazeDepth);
        }
        else
        {
            endCell[0] = Random.Range(0, _mazeWidth);
            endCell[1] = _mazeDepth - 1;
        }

        //Debug.Log("EndCell: " + endCell[0] + ", " + endCell[1]);
        return endCell;
    }

    private int[,] GetWallsToBreak()
    {
        // 
        int[,] wallsToBreak = new int[2, _numWallsToBreak];

        // 0 if end on right wall (x = _mazeDepth - 1)
        // 1 if end of front wall (z = _mazeWidth - 1)
        for (int i = 0; i < _numWallsToBreak; i++)
        {
            wallsToBreak[0, i] = Random.Range(1, _mazeDepth-1);
            wallsToBreak[1, i] = Random.Range(1, _mazeWidth - 1);

            //for (int j = 0; j < i; j++)
            //{
            //    if (wallsToBreak[0, j] == wallsToBreak[0, i] && wallsToBreak[1, j] == wallsToBreak[1, i])

            //}
        }

        //Debug.Log("EndCell: " + endCell[0] + ", " + endCell[1]);
        return wallsToBreak;
    }

    // generats maze
    //private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell, int[] endCell, int[,] wallsToBreak)
    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell, int[] endCell, int[,] wallsToBreak)
    {
        visitedCellCount++;
        if (currentCell == _mazeGrid[endCell[0], endCell[1]] && currentCell.isVisited == false)
        {
            // is at right of grid
            if (endCell[0] == _mazeWidth - 1)
            {
                //_mazeGrid[endCell[0], endCell[1]].AddComponent<NavMeshObstacle>();
                //_mazeGrid[endCell[0], endCell[1]].GetComponent<NavMeshObstacle>().transform.localPosition = new Vector3(5f, 5f, 0f);
                //_mazeGrid[endCell[0], endCell[1]].GetComponent<NavMeshObstacle>().transform.localScale = new Vector3(1f, 10f, 10f);
                //_mazeGrid[endCell[0], endCell[1]].ClearRightWall();
                _mazeGrid[endCell[0], endCell[1]].HasEnd(true);
                
            }
            else
            {
                //_mazeGrid[endCell[0], endCell[1]].AddComponent<NavMeshObstacle>();
                //_mazeGrid[endCell[0], endCell[1]].GetComponent<NavMeshObstacle>().transform.localPosition = new Vector3(0f, 5f, 5f);
                //_mazeGrid[endCell[0], endCell[1]].GetComponent<NavMeshObstacle>().transform.localScale = new Vector3(1f, 10f, 10f);
                //_mazeGrid[endCell[0], endCell[1]].ClearFrontWall();
                _mazeGrid[endCell[0], endCell[1]].HasEnd(false);
            }

            _enemyList.Add(Instantiate(_enemyControllerPrefab, new Vector3(endCell[0]*10, _enemyFallHeight, endCell[1]*10), Quaternion.identity));
            //Debug.Log("EndCell Cleared: " + endCell[0] + ", " + endCell[1]);
        }

        //if (visitedCellCount <= (_mazeWidth * _mazeDepth / 2))
        //{
        //    currentCell.SetCellMaterial(_materials[0], _mazeWidth, _mazeDepth);
        //}
        //else
        //{
        //    currentCell.SetCellMaterial(_materials[1], _mazeWidth, _mazeDepth);
        //}

        int randomMat = Random.Range(0, _materials.Count());
        currentCell.SetCellMaterial(_materials[randomMat], _mazeWidth, _mazeDepth);



        for (int i = 0; i < _numWallsToBreak; i++)
        {
            if (currentCell == _mazeGrid[wallsToBreak[0, i], wallsToBreak[1, i]] && currentCell.isVisited == false)
            {
                MazeCell randomCell;
                randomCell = GetUnvisitedCell(currentCell);
                ClearWalls(randomCell, currentCell);
                Debug.Log("Breaking wall between " + wallsToBreak[0, i] + ", " + wallsToBreak[1, i]);
            }
        }

        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        // waits before do next
        //yield return new WaitForSeconds(_timeToGenerateWalls);

        MazeCell nextCell;

        // if null, backtracks recursively
        do
        {
            nextCell = GetUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                // use yield return for coroutine
                //yield return GenerateMaze(currentCell, nextCell, _endCell, wallsToBreak);
                GenerateMaze(currentCell, nextCell, _endCell, wallsToBreak);
            }
        } while (nextCell != null);
    }

    // gets random unvisted cell (adjacent to current cell)
    private MazeCell GetUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        // orders list randomly? (return first or default return first if only 1)
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    // returns list of unvisited cells (next to current cell)
    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        // x pos of curr cell
        int x = (int)currentCell.transform.position.x;
        x = x / 10;
        // z pos of curr cell
        int z = (int)currentCell.transform.position.z;
        z = z / 10;
        // (vals correspond to index of cell in arr)

        // check cell to right is in grid
        if (x + 1 < _mazeWidth)
        {
            // if is in grid, get cell 
            var cellToRight = _mazeGrid[x + 1, z];

            // check if visited
            if (cellToRight.isVisited == false)
            {
                // if no, add cell to return collection
                yield return cellToRight;
            }
        }

        // cells start at 0, so check if not too far left
        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.isVisited == false)
            {
                yield return cellToLeft;
            }
        }

        // check cell not too far at front
        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.isVisited == false)
            {
                yield return cellToFront;
            }
        }

        // check cell not below 0
        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.isVisited == false)
            {
                yield return cellToBack;
            }
        }
    }


    // checks where prev is relative to current and break walls between
    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        // is first cell
        if (previousCell == null)
        {
            return;
        }

        // prev is LEFT of current
        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            // went left to right, so prev clear right & current clear left
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        // prev is RIGHT of current
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            // went right to left, so prev clear left & current clear right
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        // prev is BELOW of current
        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            // went down to up, so prev clear up & current clear down
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        // prev is ABOVE of current
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            // went up to down, so prev clear down & current clear up
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    public void ClearAllWalls(float time)
    {
        if (!_wallsMoving)
        {
            _wallsAreUp = false;

            _wallsMoving = true;
            for (int i = 0; i < _mazeWidth; i++)
            {
                for (int j = 0; j < _mazeDepth; j++)
                {
                    _mazeGrid[i, j].ClearAll(_mazeWidth, _mazeDepth, _timeToLowerWalls);
                }
            }

            StartCoroutine(CanResetWallsWait());

            StartCoroutine(WaitBeforeResetWalls(time));
        }
    }

    private IEnumerator CanResetWallsWait()
    {
        yield return new WaitForSeconds(_timeToLowerWalls);

        _wallsMoving = false;
        _canResetWalls = false;
    }

    private IEnumerator WaitBeforeResetWalls(float time)
    {
        yield return new WaitForSeconds(time);

        if (!_canResetWalls)
        {
            Debug.Log("coroutine donw");
            ResetAllWalls();
        }
    }

    public void PlayerResetAll()
    {
        if (!_wallsMoving)
        {
            StopCoroutine("WaitBeforeResetWalls");
            ResetAllWalls();
        }
    }
    public void ResetAllWalls()
    {
        if (!_wallsMoving)
        {
            Debug.Log("start reset");
            for (int i = 0; i < _mazeWidth; i++)
            {
                for (int j = 0; j < _mazeDepth; j++)
                {
                    _mazeGrid[i, j].ResetAll(_mazeWidth, _mazeDepth, _timeToRaiseWalls);
                }
            }

            StartCoroutine(CanRaiseWalls());
        }

    }

    private IEnumerator CanRaiseWalls()
    {
        yield return new WaitForSeconds(_timeToRaiseWalls);

        _wallsMoving = false;
        _canResetWalls = true;
        FindObjectOfType<PlayerController>().UnfreezePlayer();
        _wallsAreUp = true;
        //PlayerController.UnfreezePlayer();
    }

    public void Spawn2Enemies()//Transform transform)
    {
        //int playerX = (int)(transform.position.x/10);
        int playerX = (int)(Camera.main.transform.position.x/10);
        //int playerZ = (int)(transform.position.z/10);
        int playerZ = (int)(Camera.main.transform.position.z/10);

        for (int i = 0; i < 2; i++)
        {
            int newX = Random.Range(2, 5);
            int newZ = Random.Range(2, 5);

            int xSpawn = playerX + newX;
            int zSpawn = playerZ + newZ;

            if (xSpawn >= _mazeWidth)
            {
                xSpawn = playerX - newX;
            }
            if (zSpawn >= _mazeDepth)
            {
                zSpawn = playerZ - newZ;
            }
            Debug.Log("New X = " + xSpawn + ", New Z = " + zSpawn);
            _enemyList.Add(Instantiate(_enemyControllerPrefab, new Vector3((xSpawn * 10), _enemyFallHeight, (zSpawn * 10)), Quaternion.identity));
        }
    }

    private void OnApplicationQuit()
    {
        for (int i = _enemyList.Count - 1; i >0 ;i--)
        {
            //_enemyList[i].GetComponent<PlayableGraph>().Destroy();
            Destroy(_enemyList[i]);
        }

    }
}
