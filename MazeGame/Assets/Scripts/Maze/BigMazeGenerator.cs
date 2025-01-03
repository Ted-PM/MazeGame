using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// for random sorting of list ?
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.Playables;
//using UnityEditor.SceneManagement;

/*
 * Generates whole maze randomly, uses a graph (2D array of maze cells), the maze cells and holds them in array
 * each maze cell is then "visited" using a depth first search, is tagged as visited and breaks down walls
 * the player always spawns at cell arr[0,0] which is (x:0, y:0, z:0)
 * each cell is 10x10 units area, so its index in the array multiplied be 10 is the location of the center of that cell
 * i.e. a cell at arr[5,7] is located at x: 50, y: 0, z: 70 in the game world
 */
public class BigMazeGenerator : MonoBehaviour
{
    public static BigMazeGenerator Instance;

    // prefab of the maze cell & it's script
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    // enemy prefab, used to spawn them
    [SerializeField]
    private EnemyController _enemyControllerPrefab;

    [SerializeField]
    private BlindEnemy _blindEnemyPrefab;
    [SerializeField]
    private int _maxNumBlindEnemies;

    private List<BlindEnemy> _blindEnemyList;

    // how wided the maze is (x:0 to x:_mazeWidth)
    [SerializeField]
    private int _mazeWidth;

    // how "deep" the maze is (z:0 to z:_mazeDepth)
    [SerializeField]
    private int _mazeDepth;

    // a list of materials used for the walls in the maze (selected randomly for each cell at runtime)
    [SerializeField]
    private List<Material> _materials;

    // the array which holds all the maze cells
    private MazeCell[,] _mazeGrid;

    // an array of size 2, holding the x position and z position of the "end" of the maze (cell to reach to win)
    // it holds the x and z positions divided by 10 (corresponds to index of final cell in the _mazeGrid)
    private int[] _endCell;

    // when maze generated, usually first 10-20 cells are just one straight line because of the search algorithm
    // so when the maze is being searched, a few cells also have a random wall taken down
    // this arg is a way to specify how many random walls to break
    [SerializeField]
    private int _numWallsToBreak;

    // this 2D array (arr[2, _numWallsToBreak) holds the index of the cells with extra walls to break
    // the cell is held in the array as
    // _wallsToBreak[0,0] = random (0 to _mazeDepth - 1) (Z-axis)
    // _wallsToBreak[1,0] = random (0 to _mazeWidth - 1) (X-axis)
    private int[,] _wallsToBreak;

    // if it's true, the player can lower (reset) the walls, if false the player can raise the walls 
    // used alongside other bools to ensure player can only do so when walls aren't moving
    public bool _canResetWalls { get; private set; }

    // true when walls are moving up or down
    private bool _wallsMoving = false;

    // only true when walls are fully up (at normal height)
    public bool _wallsAreUp;

    // was only used when generating walls over time (to visualise maze cells being added)
    //[SerializeField]
    //private float _timeToGenerateWalls;

    // the time taken to lower the walls from their max height (used with lerp and delta time)
    [SerializeField]
    private float _timeToLowerWalls;

    // the time taken to raise walls from the bottom (also used with lerp and delta time)
    [SerializeField]
    private float _timeToRaiseWalls;

    // was previously used to change wall material after having visited half of the maze 
    //private int visitedCellCount = 0;

    // the Y-axis heigh from which the spawned enemies drop from
    [SerializeField]
    private float _enemyFallHeight;

    // a list which holds each enemy when instantiated (used to increase / decrease all enemy speed)
    private List<EnemyController> _enemyList;

    // a particle effect / trail which is used to show the player the path to the exit
    [SerializeField]
    private GameObject _pathLinePrefab;

    [SerializeField]
    private AudioSource _wallsMovingSound;

    [SerializeField]
    private GameObject _loadingScreen;

    [SerializeField]    
    private GameObject _mazeCellParent;

    [SerializeField]
    private GameObject _blindEnemyParent;
    [SerializeField]
    private GameObject _shyEnemyParent;

    [SerializeField]
    private int _totalNumInteractables;

    [SerializeField]
    private EndCell _endCellPrefab;

    [SerializeField]
    private GameObject _timer;

    [SerializeField]
    private Flashlight _playerFlash;
    //private float _gameTime;
    //private bool _gamePlaying;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //_gamePlaying = false;
        //_gameTime = 0f;
        //Camera.main.GetComponent<AudioListener>().volume = 0;
        _timer.SetActive(false);
        AudioListener.volume = 0;
        FindObjectOfType<PlayerController>().FreezePlayer();
        // all walls begin at max height
        _wallsAreUp = true;

        // instantiate all the arrays / lists / matrixes
        _endCell = new int[2];

        // get indexes of the end cell [ depth (x), width (z)]
        _endCell = GetEndCell();

        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        // instantiate 2d array of indexes of walls to break
        _wallsToBreak = new int[2, _numWallsToBreak];

        // gets said walls [0, n] = depth, [1,n] = width
        _wallsToBreak = GetWallsToBreak();

        _enemyList = new List<EnemyController>();
        _blindEnemyList = new List<BlindEnemy>();

        if (_totalNumInteractables < ItemSpawner.Instance.GetUniqueItemsCount() * ItemSpawner.Instance._minNumPerInteractable)
        {
            _totalNumInteractables = ItemSpawner.Instance.GetUniqueItemsCount() * ItemSpawner.Instance._minNumPerInteractable;
        }

        // instantiate all maze cells and stores them in arr at right index
        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeDepth; j++)
            {
                // i*10 = the x pos of the cell and j*10 = the z pos of the cell
                // i = grind row index and j = grid column index
                _mazeGrid[i, j] = Instantiate(_mazeCellPrefab, new Vector3((float)(i*10), 0, (float)(j * 10)), Quaternion.identity, _mazeCellParent.transform);
            }
        }

        // because GenerateMaze calls a recursive function, start at first cell [0,0]
        GenerateMaze(null, _mazeGrid[0, 0], _endCell, _wallsToBreak);
        _canResetWalls = true;
        SpawnInteractables();
        SpawnBlindEnemies();

        StartCoroutine(WaitThenStartGame());
    }

    //private void FixedUpdate()
    //{
    //    if (_gamePlaying)
    //    {
    //        _gameTime += Time.fixedDeltaTime;
    //    }
    //}

    //private void OnDestroy()
    //{
    //    _gamePlaying = false;
    //    PlayerPrefs.SetFloat("gameTime", _gameTime);
    //}

    /* GetEndCell()
     * 
     * because the player spawns at [x:0 z:0], 
     * the end cell is either on the x wall - [x = _mazeDepth - 1] and [z = random num from 0 to _mazeWidth - 1]
     * or on the z wall - [x = random num from 0 to _mazeDepth - 1] and [z = _mazeWidth - 1]
     * this function finds a random cell on one of the two walls and returns it as the end cell
     */
    private int[] GetEndCell()
    {
        int[] endCell = new int[2];

        // decide if end is on x wall (0) or z wall (1)
        int endBorder = Random.Range(0, 2);

        if (endBorder == 0)
        {
            endCell[0] = Random.Range(0, _mazeWidth);
            endCell[1] = _mazeDepth - 1;
        }
        else
        {
            endCell[0] = _mazeWidth - 1;
            endCell[1] = Random.Range(0, _mazeDepth);
        }

        return endCell;
    }

    private void StartGame()
    {
        AudioListener.volume = 1;
        _loadingScreen.SetActive(false);
        FindObjectOfType<PlayerController>().UnfreezePlayer();
        _timer.SetActive(true);
        //_gamePlaying = true;
    }

    private IEnumerator WaitThenStartGame()
    {
        DisableDuplicateWalls();
        yield return new WaitForSeconds(3);
        StartGame();
    }

    // row major

    private void DisableDuplicateWalls()//int x, int z)
    {
        //bool result = false;

        for (int i = 0; i < _mazeWidth-1; i++)
        {
            for (int j = 0; j < _mazeDepth-1; j++)
            {
                _mazeGrid[i, j].DestroyUnactiveDoors();
                if (!_mazeGrid[i, j].GetRightWallStatus())
                {
                    _mazeGrid[i, j].ClearRightWall();
                }
                if (!_mazeGrid[i, j].GetFrontWallStatus())
                {
                    _mazeGrid[i, j].ClearFrontWall();
                }
                // check if add crouch wall
                if (TrueForXFalse(10))
                {
                    Debug.Log("adding crouchwall");
                    // add to front (z-axis, depth)
                    if (TrueFalse() && _mazeGrid[i, j].GetFrontWallStatus() && !_mazeGrid[i, j]._frontDoorAdded && !_mazeGrid[i, j+1]._backDoorAdded)
                    {
                        Debug.Log("front true");
                        if (j+1 < _mazeDepth-1)
                        {
                            _mazeGrid[i, j + 1].ClearBackWall();
                            _mazeGrid[i,j].AddFrontCrouchWall();
                            Debug.Log("front added");
                        }
                    }
                    // add to right (x-axis, width)
                    else if (_mazeGrid[i, j].GetRightWallStatus() && !_mazeGrid[i, j]._rightDoorAdded && !_mazeGrid[i + 1, j]._leftDoorAdded)
                    {
                        Debug.Log("right true");
                        if (i + 1 < _mazeWidth - 1)
                        {
                            _mazeGrid[i + 1, j].ClearLeftWall();
                            _mazeGrid[i, j].AddRightCrouchWall();
                            Debug.Log("right added");
                        }
                    }
                }

                _mazeGrid[i, j].DestroyUnactiveCrouchWalls();
            }
        }
        //DestroyUnactiveDoors()

        //return result;
    }

    /* GetWallsToBreak()
     * finds random cells, stores them in 2d array (number of cells = _numWallsToBreak)
     * [0, x] = _mazeDepth and [1,z] = _mazeWidth
     * returns the array to start
     */
    private int[,] GetWallsToBreak()
    {
        int[,] wallsToBreak = new int[2, _numWallsToBreak];

        for (int i = 0; i < _numWallsToBreak; i++)
        {
            wallsToBreak[0, i] = Random.Range(4, _mazeDepth-1);
            wallsToBreak[1, i] = Random.Range(4, _mazeWidth - 1);
        }

        return wallsToBreak;
    }

    private void SpawnInteractables()
    {
        List<Vector2Int> interactableCells= new List<Vector2Int>();

        for (int i = 0; i < _totalNumInteractables; i++)
        {
            interactableCells.Add(new Vector2Int(Random.Range(3,_mazeWidth-1), Random.Range(3, _mazeDepth-1)));
        }

        for (int i = 0; i < interactableCells.Count; i++)
        {
            ItemSpawner.Instance.SpawnRandomInteractable(interactableCells[i].x, interactableCells[i].y);
        }
        //SpawnInteractables(interactableCells);
        //return interactableCells;
    }

    private void SpawnBlindEnemies()
    {
        List<Vector2Int> blindEnemyCells = new List<Vector2Int>();
        for (int i = 0; i < _maxNumBlindEnemies; i ++)
        {
            blindEnemyCells.Add(new Vector2Int(Random.Range(5, _mazeWidth - 1), Random.Range(5, _mazeDepth - 1)));
        }
        for (int i = 0; i < _maxNumBlindEnemies; i++)
        {
            Instantiate(_blindEnemyPrefab, new Vector3(blindEnemyCells[i].x * 10, 0, blindEnemyCells[i].y * 10), Quaternion.identity, _blindEnemyParent.transform);
        }
    }

    //private void SpawnInteractables(List<Vector2Int> interactableCells)
    //{
    //    for (int i = 0; i < interactableCells.Count; i++)
    //    {
    //        ItemSpawner.Instance.SpawnRandomInteractable(interactableCells[i].x, interactableCells[i].y);
    //    }
    //}


    /* GenerateMaze(cell visited before current, the currentCell, index of the end cell [x, z], indexes of all "random" cells with walls to break)
     * generates maze by marking current cell as visited, then finding a random adjacent UNVISITED cell, then breaking the walls between
     * repeats until current cell has no unvisited neighbours, then recursively backtracks until ther is an unvisited cell
     * repeats untill all cells are visited
     */
    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell, int[] endCell, int[,] wallsToBreak)
    {
        // check if the current cell is the last cell and this is the first time it's appeared
        if (currentCell == _mazeGrid[endCell[0], endCell[1]] && currentCell.isVisited == false)
        {
            // is at right of grid so x = max val
            if (endCell[0] == _mazeWidth - 1)
            {
                // calls a MazeCell member function to mark the cell as the end and destroy the "end" wall
                // pass true to the function knows to destroy the right wall
                _mazeGrid[endCell[0], endCell[1]].HasEnd(true);
                var endCellObj = Instantiate(_endCellPrefab, new Vector3(currentCell.transform.position.x + 10, 0f, currentCell.transform.position.z), Quaternion.identity);
                endCellObj.DisableLeft();
            }
            else
            {
                // if not on the right wall, but be at the top (z = max), pass false so func knows to destroy the top wall
                _mazeGrid[endCell[0], endCell[1]].HasEnd(false);
                var endCellObj = Instantiate(_endCellPrefab, new Vector3(currentCell.transform.position.x, 0f, currentCell.transform.position.z + 10), Quaternion.identity);
                endCellObj.DisableBottom();
            }

            // spawn the first enemy at the end of the maze, infront of the end
            _enemyList.Add(Instantiate(_enemyControllerPrefab, new Vector3(endCell[0]*10, _enemyFallHeight, endCell[1]*10), Quaternion.identity, _shyEnemyParent.transform));
        }

        // choose a random material from the list
        int randomMat = Random.Range(0, _materials.Count());
        //add that material to all of the cell walls before they are destroyed
        currentCell.SetCellMaterial(_materials[randomMat], _mazeWidth, _mazeDepth);


        // check if the current cell needs to break random walls (i.e. is in the wallsToBreak array)
        for (int i = 0; i < _numWallsToBreak; i++)
        {
            if (currentCell == _mazeGrid[wallsToBreak[1, i], wallsToBreak[0, i]] && currentCell.isVisited == false)
            {
                // if it should have random walls broken,
                MazeCell randomCell;
                // find neighbor which has walls up
                randomCell = GetUnvisitedCell(currentCell);
                // destory the walls with the random neightbor
                ClearWalls(randomCell, currentCell);
            }
        }

        // visit the current cell (mark it internally as visited)
        currentCell.Visit();
        // spawn a random item on it at it's current position
        if (TrueFalseFalse())
        {
            ItemSpawner.Instance.SpawnRandomItem((int)(currentCell.transform.position.x / 10), (int)(currentCell.transform.position.z / 10));
        }

        //if (TrueForXFalse(15))
        //{
        //    ItemSpawner.Instance.SpawnRandomInteractable((int)(currentCell.transform.position.x / 10), (int)(currentCell.transform.position.z / 10));
        //}

        //if (TrueForXFalse(40) && ((int)((currentCell.transform.position.x+5) / 10) * 10 > 3 && (int)((currentCell.transform.position.z+5) / 10) * 10 > 3))
        //{
        //    _blindEnemyList.Add(Instantiate(_blindEnemyPrefab, new Vector3((int)(currentCell.transform.position.x / 10) * 10, 0, (int)(currentCell.transform.position.z / 10) * 10), Quaternion.identity, _blindEnemyParent.transform));
        //}
            //ItemSpawner.Instance.SpawnWallItemLeft((int)(currentCell.transform.position.x / 10), (int)(currentCell.transform.position.z / 10));
        //ItemSpawner.Instance.SpawnWallItemRight((int)(currentCell.transform.position.x / 10), (int)(currentCell.transform.position.z / 10));
        //ItemSpawner.Instance.SpawnWallItemFront((int)(currentCell.transform.position.x / 10), (int)(currentCell.transform.position.z / 10));
        //ItemSpawner.Instance.SpawnWallItemBack((int)(currentCell.transform.position.x / 10), (int)(currentCell.transform.position.z / 10));

        // call the clear walls function, 
        ClearWalls(previousCell, currentCell);

        // create temp var to hold the next cell
        MazeCell nextCell;

        // if null, backtracks recursively
        do
        {
            // find unvisited neightbor
            nextCell = GetUnvisitedCell(currentCell);

            // if unvisited neightbor exists
            if (nextCell != null)
            {
                // call the function recursively
                GenerateMaze(currentCell, nextCell, _endCell, wallsToBreak);
            }
        } while (nextCell != null);
        // exit loop if there was no unvisited neightbor, therby backtracking recursively to prev visited cell
    }

    private bool TrueForXFalse(int x)
    {
        int temp = 0;
        temp = Random.Range(0, x);

        return temp == 0;
    }

    // returns true or false randomly, used to decide if broken wall will have door instead
    private bool TrueFalse()
    {
        int temp = 0;
        temp = Random.Range(0, 2);

        return temp == 0;
    }

    // returns true 1/3 of the time, and false 2/3 
    private bool TrueFalseFalse()
    {
        int temp = 0;
        temp = Random.Range(0, 3);

        return temp == 0;
    }

    // gets random unvisted cell (adjacent to current cell)
    private MazeCell GetUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        // orders list randomly?  (orders list randomly and returns the first one in the list)
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    /* IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
     * returns list of unvisited cells (next to current cell
     * uses IEnumerable<MazeCell> and yield return to return said list
     */
    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        // x pos of curr cell (use int so no wierd rounding)
        int x = (int)currentCell.transform.position.x;
        // divide by 10 to find the index in _mazeGrid array [x, z]
        x = x / 10;
        // z pos of curr cell
        int z = (int)currentCell.transform.position.z;
        // same as before
        z = z / 10;

        // check cell to right is in grid bounds (i.e. x + 1 < _mazeWidth)
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

        // cells start at 0, so check if not too far left (i.e. x - 1 >= 0)
        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.isVisited == false)
            {
                yield return cellToLeft;
            }
        }

        // check cell not too far at front (i.e. z + 1 < _mazeDepth)
        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.isVisited == false)
            {
                yield return cellToFront;
            }
        }

        // check cell not below grid (i.e. z - 1 >= 0)
        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.isVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    /* ClearWalls( cell visited before current,  currentCell)
     * checks where the previous cell is, relative to the current, and uses that to destroy the walls between them
     * if previous cell is to the left and current cell is to the right: previous cell destroyes right wall and current destroyes left wall
     */
    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        // check that it's not the first cell passed (i.e. there is no previous cell
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
            if (TrueFalseFalse())
            {
                currentCell.AddLeftDoor();
            }
            return;
        }

        // prev is RIGHT of current
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            // went right to left, so prev clear left & current clear right
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            if (TrueFalseFalse())
            { 
                currentCell.AddRightDoor(); 
            }
            return;
        }

        // prev is BELOW of current
        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            // went down to up, so prev clear up & current clear down
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            if (TrueFalseFalse())
            {
                currentCell.AddBackDoor();
            }
            return;
        }

        // prev is ABOVE of current
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            // went up to down, so prev clear down & current clear up
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            if (TrueFalseFalse())
            {
                currentCell.AddFrontDoor();
            }
            return;
        }
    }

    /* ClearAllWalls(float time)
     * loweres all of the walls so that they are barely visible above the floor
     * walls must not already be moving 
     */
    public void ClearAllWalls(float time)
    {
        if (!_wallsMoving)
        {
            _wallsAreUp = false;

            _wallsMoving = true;
            _wallsMovingSound.Play();
            FindObjectOfType<PlayerController>().ShakeCamera(_timeToLowerWalls);
            StartCoroutine(_playerFlash.Flicker(_timeToLowerWalls));
            StartCoroutine(FindObjectOfType<OuterWorld>().LowerCeeling(_timeToLowerWalls));
            //PlayerController

            // calls member function of MazeCell to lower all the walls
            for (int i = 0; i < _mazeWidth; i++)
            {
                for (int j = 0; j < _mazeDepth; j++)
                {
                    _mazeGrid[i, j].ClearAll(_mazeWidth, _mazeDepth, _timeToLowerWalls);
                }
            }

            // increases the speed of all the enemies
            for (int i = 0; i < _enemyList.Count(); i++)
            {
                if (_enemyList[i] != null)
                {
                    _enemyList[i].IncreaseEnemySpeed();
                    _enemyList[i].PlayGrowl();
                }
            }

            // wait for walls to be lowered before allowing the player to raise them
            StartCoroutine(CanResetWallsWait());

            // wait for a while then raise the walls automatically if the player hasent already done so
            StartCoroutine(WaitBeforeResetWalls(time));
        }
    }

    // waits until the walls are lowered, the calls a function to show the player the best path to the exit
    private IEnumerator CanResetWallsWait()
    {
        yield return new WaitForSeconds(_timeToLowerWalls);
        _wallsMovingSound.Stop();
        // once walls are at the bottom, allow player to raise walls 
        FindBestPath();
        _wallsMoving = false;
        _canResetWalls = false;
    }

    // wait for a while then automatically reset the walls
    private IEnumerator WaitBeforeResetWalls(float time)
    {
        yield return new WaitForSeconds(time);
        
        // check that the player hasen't manually reset the walls
        if (!_canResetWalls)
        {
            Debug.Log("coroutine donw");
            ResetAllWalls();
        }
    }

    // if the player reset's the walls, stop the coroutine that would do it automatically and reset them
    public void PlayerResetAll()
    {
        // check the player can actually reset the walls
        if (!_wallsMoving)
        {
            StopCoroutine("WaitBeforeResetWalls");
            ResetAllWalls();
        }
    }

    // raises all of the walls
    public void ResetAllWalls()
    {
        // check they aren't moving
        if (!_wallsMoving)
        {
            _wallsMovingSound.Play();
            FindObjectOfType<PlayerController>().ShakeCamera(_timeToRaiseWalls);
            StartCoroutine(FindObjectOfType<OuterWorld>().RaiseCeeling(_timeToRaiseWalls));
            StartCoroutine(_playerFlash.Flicker(_timeToRaiseWalls));
            // go through all cells and call a member function to raise them 
            for (int i = 0; i < _mazeWidth; i++)
            {
                for (int j = 0; j < _mazeDepth; j++)
                {
                    _mazeGrid[i, j].ResetAll(_mazeWidth, _mazeDepth, _timeToRaiseWalls);
                    StopCoroutine("DisplayBestPath");
                }
            }

            // wait until walls are back up before allowing player to lower them again
            StartCoroutine(CanRaiseWalls());
        }

    }

    // wait until all the walls are bak at the top, then allow the player to lower them again
    private IEnumerator CanRaiseWalls()
    {
        yield return new WaitForSeconds(_timeToRaiseWalls);
        _wallsMovingSound.Stop();
        _wallsMoving = false;
        _canResetWalls = true;

        // decrease the speed of all enemies
        for (int i = 0; i < _enemyList.Count(); i++)
        {
            if (_enemyList[i] != null)
            {
                _enemyList[i].DecreaseEnemySpeed();
            }
        }

        // use coroutine because need to wait for navmeshs / meshes update (or else enemy seen then unseen at time is killed)
        StartCoroutine(UpdateWallsAreUp());
    }

    private IEnumerator UpdateWallsAreUp()
    {

        yield return new WaitForSeconds(0.1f);
        // because player is frozen when walls begin lowering, unfreeze them when they are done raising
        FindObjectOfType<PlayerController>().UnfreezePlayer();
        _wallsAreUp = true;
    }

    /*
     * spawn two enemies each time one is killed at a random position relative to the player
     * the position is based on main camera (attached to player) and they are spawned in a grid around the player\
     * at least 2 cells away from player and at most 4 cells
     */
    public void Spawn2Enemies()
    {
        // get player pos (using indexes in _mazeGrid)
        int playerX = (int)(Camera.main.transform.position.x/10);
        int playerZ = (int)(Camera.main.transform.position.z/10);

        for (int i = 0; i < 2; i++)
        {
            // find random grid distance
            int newX = Random.Range(2, 5);
            int newZ = Random.Range(2, 5);
            // add distance to player position
            int xSpawn = playerX + newX;
            int zSpawn = playerZ + newZ;

            // if the new pos is outisde of the maze, subtract instead
            if (xSpawn >= _mazeWidth)
            {
                xSpawn = playerX - newX;
            }
            if (zSpawn >= _mazeDepth)
            {
                zSpawn = playerZ - newZ;
            }
            //Debug.Log("New X = " + xSpawn + ", New Z = " + zSpawn);
            // create the enemy and add to the list
            _enemyList.Add(Instantiate(_enemyControllerPrefab, new Vector3((xSpawn * 10), _enemyFallHeight, (zSpawn * 10)), Quaternion.identity));
        }
    }

    // when app quit, destroy all enemies in the list (was getting issues with prefabs remaining after quit)
    private void OnApplicationQuit()
    {
        for (int i = _enemyList.Count - 1; i >0 ;i--)
        {
            //_enemyList[i].GetComponent<PlayableGraph>().Destroy();
            Destroy(_enemyList[i]);
        }

    }

    /*
     * finds the best path to get from the player position to the end of the maze
     * calls a recursive function with the initial location, and passes refrence variables to track it
     * then displayes the best path using a coroutine (to show the icon moving from start to end)
     * uses a "cost" to make sure the path chosen is the shortest
     */
    private void FindBestPath()
    {
        Debug.Log("Find best path start");
        
        // parrallel array to the _mazeGrid, but bools to check if that node has been used in the main path 
        bool[,] nodeVisited = new bool[_mazeWidth, _mazeDepth];
        List<MazeCell> bestPath = new List<MazeCell>();
        List<int> cellCost = new List<int>();

        Transform playerPosition = Camera.main.transform;
        int playerX = (int)((playerPosition.transform.position.x + 5) / 10);
        int playerZ = (int)((playerPosition.transform.position.z+5) / 10);
        Debug.Log("PlayerX: " + playerX + ", PlayerZ: " + playerZ + "accX: " + (int)(playerPosition.transform.position.x) + ", accZ: " + (int)(playerPosition.transform.position.z));
        int minCost = 0;

        if (_mazeGrid[playerX, playerZ] != _mazeGrid[_endCell[0], _endCell[1]])
        {
            FindBestPathRec(ref nodeVisited, playerX, playerZ, ref minCost, ref bestPath, ref cellCost);

            StartCoroutine(DisplayBestPath(bestPath, cellCost));
        }


    }
    /* FindBestPathRec(bool matrix of nodes visited, x index of current cell, z index of curr cell, the cost of the current path, array of MazeCells - from current to end following best path)
     * 
     * 
     */
    //private bool FindBestPathRec(bool[,] nodeVisted, int x, int z, ref int cost, ref List<MazeCell> bestPath)
    private bool FindBestPathRec(ref bool[,] nodeVisted, int x, int z, ref int cost, ref List<MazeCell> bestPath, ref List<int> cellCost)
    {
        // set that the best path isn't found
        bool result = false;
        // increment cost
        cost++;

        // if the current cell is outside of the maze bounds, can't be best path
        if (x >= _mazeWidth || z >= _mazeDepth || x < 0 || z < 0)
        {
            result = false;
        }
        // if the current cell is equal to the end cell, a path has been found (but might not be chosen depending on the cost)
        else if (_mazeGrid[x, z] == _mazeGrid[_endCell[0], _endCell[1]])
        {
            result = true;
            nodeVisted[x, z] = true;
            bestPath.Add(_mazeGrid[x, z]);
            cellCost.Add(cost);

        }   
        else
        {
            // if the current cell hasne't already been visited
            if (!nodeVisted[x, z])
            {
                // visit it by marking the parrallel array of bool as true (at the same index as the current cell in _mazeGrid)
                nodeVisted[x, z] = true;

                // get relative cost of each other path (i.e. find cost of going to end using current cell as start)
                // also use a bool to check if that path acc lead to the end
                int leftCost = cost;
                bool leftTrue = false;
                int rightCost = cost;
                bool rightTrue = false;
                int frontCost = cost;
                bool frontTrue = false;
                int backCost = cost;
                bool backTrue = false;

                bool[,] leftNodeVisited = nodeVisted;
                bool[,] rightNodeVisited = nodeVisted;
                bool[,] frontNodeVisited = nodeVisted;
                bool[,] backNodeVisited = nodeVisted;

                // default this to 3 (will be used to select which path is best)
                // 0: left, 1: right, 2: front, 3: back
                int trueDirection = 3;

                // check functions return true if the cell in that direction is accessible (i.e. no wall blocking) 
                if (CheckLeft(x, z))
                {
                    leftTrue = FindBestPathRec(ref leftNodeVisited, x - 1, z, ref leftCost, ref bestPath, ref cellCost);
                    if (leftTrue) { result = true; }
                }
                if (CheckRight(x, z))
                {
                    rightTrue = FindBestPathRec(ref rightNodeVisited, x + 1, z, ref rightCost, ref bestPath, ref cellCost);
                    if (rightTrue) { result = true; }
                }
                if (CheckFront(x, z))
                {
                    frontTrue = FindBestPathRec(ref frontNodeVisited, x, z + 1, ref frontCost, ref bestPath, ref cellCost);
                    if (frontTrue) { result = true; }
                }
                if (CheckBack(x, z))
                {
                    backTrue = FindBestPathRec(ref backNodeVisited, x, z - 1, ref backCost, ref bestPath, ref cellCost);
                    if (backTrue) { result = true; }
                }

                if (result)
                {
                    // if one of the paths led to the end, add the current cell to the bestPath List
                    bestPath.Add(_mazeGrid[x, z]);
                    int tempCost = cost;
                    cellCost.Add(tempCost);

                    // if left lead to the exit, compare it to the cost of all the other paths which also  lead to the exit
                    if (leftTrue)
                    {
                        if (rightTrue && leftCost < rightCost)
                        {
                            trueDirection = 0;
                        }
                        else if (rightTrue)
                        {
                            trueDirection = 1;
                        }
                        if (frontTrue && leftCost < frontCost)
                        {
                            trueDirection = 0;
                        }
                        else if (frontTrue)
                        {
                            trueDirection = 2;
                        }
                        if (backTrue && leftCost < backCost)
                        {
                            trueDirection = 0;
                        }
                        else if (backTrue)
                        {
                            trueDirection = 3;
                        }
                    }
                    // re do checks if right is also true, but don't compare it to the left now (already checked)
                    if (rightTrue)
                    {
                        if (frontTrue && rightCost < frontCost)
                        {
                            trueDirection = 1;
                        }
                        else if (frontTrue)
                        {
                            trueDirection = 2;
                        }
                        if (backTrue && rightCost < backCost)
                        {
                            trueDirection = 1;
                        }
                        else if (backTrue)
                        {
                            trueDirection = 3;
                        }
                    }
                    // repeat but for front (no check left or right) - (if back was best then it wouldn't have changed from default 3)
                    if (frontTrue)
                    {
                        if (backTrue && frontCost < backCost)
                        {
                            trueDirection = 2;
                        }
                        else if (backTrue)
                        {
                            trueDirection = 3;
                        }
                    }

                    // increase the refrence cost by the best path
                    if (trueDirection == 0)
                    {
                        nodeVisted = leftNodeVisited;
                        cost += leftCost;
                    }
                    else if (trueDirection == 1)
                    {
                        nodeVisted = rightNodeVisited;
                        cost += rightCost;
                    }
                    else if (trueDirection == 2)
                    {
                        nodeVisted = frontNodeVisited;
                        cost += frontCost;
                    }
                    else if (trueDirection == 3)
                    {
                        nodeVisted = backNodeVisited;
                        cost += backCost;
                    }
                    //int tempCost = cost;
                    //cellCost.Add(tempCost);
                }
            }
        }
        // return's true only if the path has the lowest cost and it lead to the end
        return result;

    }

    /*
     * displays a particle orb thing that moves from the players location to the end of the maze (leaving trail behind)
     * 
     */
    private IEnumerator DisplayBestPath(List<MazeCell> bestPath, List<int> cellCost)
    {
        // create the prefab
        var _pathLine = Instantiate(_pathLinePrefab, bestPath[bestPath.Count() - 1].transform.position + new Vector3(0,5,0), Quaternion.identity);

        for (int i = bestPath.Count() - 1; i >= 0; i--)
        {
            // wait for .1 seconds then display next
            yield return new WaitForSeconds(0.1f);
            // check that it's still in the array
            if (i + 1 < bestPath.Count())
            {

                // make the prefab look at the next cell in the path
                _pathLine.transform.LookAt(bestPath[i].transform.position);
                float time = 0f;
                float t = 0;
                // move the prefab from it's current location to the next cell in a linear manner (using lerp)
                while (t < 1)
                {
                    yield return null;
                    time += Time.deltaTime;
                    t = time / 0.2f;

                    // pass the prev cell as A, the next cell loc as B and the time percentage
                    _pathLine.transform.localPosition = Vector3.Lerp(bestPath[i+1].transform.position + new Vector3(0, 5, 0), bestPath[i].transform.position + new Vector3(0, 5, 0), t);
                }
                // add icon over cell 
                //Debug.Log("CurrentCell (X,Z): " + bestPath[i].transform.position.x + ", " + bestPath[i].transform.position.z);
                bestPath[i].EnablePathToEnd();
                //Debug.Log("cell: x(" + (int)(bestPath[i].transform.position.x + 5) / 10 + "), z(" + (int)(bestPath[i].transform.position.z + 5) / 10 + "), COST = " + cellCost[i]);
            }
            // ----
            if (bestPath[i].isFrontEnd || bestPath[i].isRightEnd || _wallsAreUp)
            {
                break;
                //Debug.Log("^END CELL");
            }
            // ----
        }

        // when done destroy path prefab and disbale all the icons over the path cells
        Destroy(_pathLine);

        for (int i = 0; i < bestPath.Count(); i++)
        {
            bestPath[i].DisablePathToEnd();
        }

    }

    

    private bool CheckLeft(int x, int z)
    {
        bool result = false;

        // cells start at 0, so check if not too far left
        if (x - 1 >= 0 && _mazeGrid[x,z].GetLeftWallStatus() && _mazeGrid[x-1, z].GetRightWallStatus())
        {
            result = true;
            //Debug.Log("left true, x: " + (x-1) + ",z " + z);
        }

        return result;
    }

    private bool CheckRight(int x, int z)
    {
        bool result = false;

        //right
        if (x + 1 < _mazeWidth && _mazeGrid[x, z].GetRightWallStatus() && _mazeGrid[x + 1, z].GetLeftWallStatus())
        {
            result = true;
            //Debug.Log("Right true, x: " + (x + 1) + ",z " + z);
        }

        return result;
    }

    private bool CheckFront(int x, int z)
    {
        bool result = false;

        //right
        if (z + 1 < _mazeDepth && _mazeGrid[x, z].GetFrontWallStatus() && _mazeGrid[x, z+1].GetBackWallStatus())
        {
            //Debug.Log("front true, x: " + (x) + ",z " + (z+1));
            result = true;
        }

        return result;
    }
    private bool CheckBack(int x, int z)
    {
        bool result = false;

        //right
        if (z - 1 >= 0 && _mazeGrid[x, z].GetBackWallStatus() && _mazeGrid[x, z - 1].GetFrontWallStatus())
        {
            result = true;
            //Debug.Log("back true, x: " + (x) + ",z " + (z-1));
        }

        return result;
    }

    private void DisableBestPath()
    {
        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeDepth; j++)
            {
                //Debug.Log("Disable Node x: " + i + ", z: " + j);
                _mazeGrid[i,j].DisablePathToEnd();
            }
        }
    }

    public int GetMazeWidth()
    {
        return _mazeWidth;
    }
    public int GetMazeDepth()
    {
        return _mazeDepth;
    }
}
