using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    enum gridSpace { empty, floor, wall, boss, border, player, emptyFloor };
    gridSpace[,] grid;
    int roomHeight, roomWidth;
    Vector2 roomSizeWorldUnits = new Vector2(60, 70);
    float worldUnitsInOneGridCell = 2;
    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }
    List<walker> walkers;
    float chanceWalkerChangeDir = 0.5f, chanceWalkerSpawn = 0.05f;
    float chanceWalkerDestoy = 0.05f;
    int maxWalkers = 10;
    float percentToFill = 0.2f;

    int randomicEnemy;
    int enemiesLeft;
    int bossLeft;
    int chests;
    int ammoKits;
    //int sacrifices;
    bool bossSpawned = false;
    bool foundPlayerSpace = false;
    bool foundBossPlace = false;
    GameObject skillStorage;

    [Header("LEVEL")]
    public GameObject startFloor;
    public GameObject normalFloor;
    public GameObject darkFloor;
    public GameObject normalWall;
    public GameObject darkWall;
    public LayerMask wallLayer;
    //public float sacrificeSpawnChance;

    [Header("PLAYER")]
    public int spawnRange;
    public GameObject player;

    [Header("ENEMIES")]
    public float enemyRate;
    public GameObject[] enemies;
    public LayerMask enemyLayer;

    [Header("BOSS")]
    public int explosionRange;
    public GameObject boss;

    [Header("PROPS")]
    public GameObject chest;   
    public int chestsUnits;
    public GameObject ammoKit;
    public int ammoKitUnits;
    //public GameObject sacrifice;

    void Start()
    {
        Setup();
        CreateFloors();
        CreateWalls();
        CreatePlayer();
        CreateIndestructableWalls();
        RemoveSingleWalls();
        SpawnLevel();
    }
    void Setup()
    {
        //find grid size
        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
        //create grid
        grid = new gridSpace[roomWidth, roomHeight];
        //set grid's default state
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //make every cell "empty"
                grid[x, y] = gridSpace.empty;
            }
        }
        //set first walker
        //init list
        walkers = new List<walker>();
        //create a walker 
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();
        //find center of grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / worldUnitsInOneGridCell),
                                        Mathf.RoundToInt(roomHeight / worldUnitsInOneGridCell));
        newWalker.pos = spawnPos;
        //add walker to list
        walkers.Add(newWalker);
    }
    void CreateFloors()
    {
        int iterations = 0;//loop will not run forever
        do
        {
            //create floor at position of every walker
            foreach (walker myWalker in walkers)
            {
                grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = gridSpace.floor;
            }
            //chance: destroy walker
            int numberChecks = walkers.Count; //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if its not the only one, and at a low chance
                if (Random.value < chanceWalkerDestoy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break; //only destroy one per iteration
                }
            }
            //chance: walker pick new direction
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < chanceWalkerChangeDir)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.dir = RandomDirection();
                    walkers[i] = thisWalker;
                }
            }
            //chance: spawn new walker
            numberChecks = walkers.Count; //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if # of walkers < max, and at a low chance
                if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
                {
                    //create a walker 
                    walker newWalker = new walker();
                    newWalker.dir = RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }
            //move walkers
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir;
                walkers[i] = thisWalker;
            }
            //avoid boarder of grid
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                //clamp x,y to leave a 1 space boarder: leave room for walls
                thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                walkers[i] = thisWalker;
            }
            //check to exit loop
            if ((float)NumberOfFloors() / (float)grid.Length > percentToFill)
            {
                break;
            }
            iterations++;
        } while (iterations < 100000);
    }
    void CreateWalls()
    {
        //loop though every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //if theres a floor, check the spaces around it
                if (grid[x, y] == gridSpace.floor)
                {
                    //if any surrounding spaces are empty, place a wall
                    if (grid[x, y + 1] == gridSpace.empty)
                    {
                        grid[x, y + 1] = gridSpace.wall;
                    }
                    if (grid[x, y - 1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }
                    if (grid[x - 1, y] == gridSpace.empty)
                    {
                        grid[x - 1, y] = gridSpace.wall;
                    }
                }
            }
        }
    }
    void CreatePlayer()
    {
        for (int i = 1; i < roomWidth; i++)
        {
            for (int j = 1; j < roomHeight; j++)
            {
                if (grid[i, j] == gridSpace.floor)
                {
                    foundPlayerSpace = true;
                    grid[i, j] = gridSpace.player;

                    for (int k = i - 1; k <= i + spawnRange && k <= roomWidth; k++)
                    {
                        for (int l = j - 1; l <= j + spawnRange && l <= roomHeight; l++)
                        {
                            if (grid[k, l] == gridSpace.floor)
                            {
                                grid[k, l] = gridSpace.emptyFloor;
                            }
                        }
                    }
                    break;
                }
            }

            if (foundPlayerSpace)
            {
                break;
            }
        }
    }
    void CreateBoss()
    {
        for (int i = roomWidth / 2; i < roomWidth; i++)
        {
            for (int j = roomHeight / 2; j < roomHeight; j++)
            {
                if (grid[i, j] == gridSpace.floor)
                {
                    foundBossPlace = true;
                    grid[i, j] = gridSpace.boss;

                    break;
                }
            }

            if (foundBossPlace)
            {
                break;
            }
        }
    }
    void SpawnBoss()
    {
        for(int x = 0; x < roomWidth; x++)
        {
            for(int y = 0; y < roomHeight; y++)
            {
                switch(grid[x, y])
                {
                    case gridSpace.boss:
                        SpawnExplosive(x, y, boss);
                        break;
                }
            }
        }
    }
    void CreateIndestructableWalls()
    {
        grid[roomWidth - 1, roomHeight - 1] = gridSpace.border;

        for (int x = 0; x < roomWidth - 1; x++)
        {
            grid[x, 0] = gridSpace.border;
            grid[x, roomHeight - 1] = gridSpace.border;
        }

        for (int y = 0; y < roomHeight - 1; y++)
        {
            grid[0, y] = gridSpace.border;
            grid[roomWidth - 1, y] = gridSpace.border;
        }
    }
    void RemoveSingleWalls()
    {
        //loop though every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //if theres a wall, check the spaces around it
                if (grid[x, y] == gridSpace.wall)
                {
                    //assume all space around wall are floors
                    bool allFloors = true;
                    //check each side to see if they are all floors
                    for (int checkX = -1; checkX <= 1; checkX++)
                    {
                        for (int checkY = -1; checkY <= 1; checkY++)
                        {
                            if (x + checkX < 0 || x + checkX > roomWidth - 1 ||
                                y + checkY < 0 || y + checkY > roomHeight - 1)
                            {
                                //skip checks that are out of range
                                continue;
                            }
                            if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
                            {
                                //skip corners and center
                                continue;
                            }
                            if (grid[x + checkX, y + checkY] != gridSpace.floor)
                            {
                                allFloors = false;
                            }
                        }
                    }
                    if (allFloors)
                    {
                        grid[x, y] = gridSpace.floor;
                    }
                }
            }
        }
    }
    void SpawnLevel()
    {
        chests = chestsUnits;
        ammoKits = ammoKitUnits;
        //sacrifices = 1

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.player:
                        Spawn(x, y, startFloor);
                        Spawn(x, y, player);
                        break;
                    case gridSpace.emptyFloor:
                        Spawn(x, y, startFloor);
                        break;
                    case gridSpace.empty:
                        Spawn(x, y, darkFloor);
                        Spawn(x, y, normalWall);
                        break;
                    case gridSpace.floor:
                        Spawn(x, y, normalFloor);

                        if(Random.Range(0, 101) <= enemyRate)
                        {
                            randomicEnemy = Random.Range(0, enemies.Length);
                            Spawn(x, y, enemies[randomicEnemy]);

                        }
                        else if (Random.Range(0, 101) <= 2)
                        {
                            if (chests > 0)
                            {
                                Spawn(x, y, chest);
                                chests--;
                            }
                        }
                        else if (Random.Range(0, 101) <= 1.25f)
                        {
                            if (ammoKits > 0)
                            {
                                Spawn(x, y, ammoKit);
                                ammoKits--;
                            }
                        }
                        break;
                    case gridSpace.border:
                        Spawn(x, y, darkWall);
                        break;
                    case gridSpace.wall:
                        Spawn(x, y, darkFloor);
                        Spawn(x, y, normalWall);
                        break;
                }
            }
        }
    }
    int NumberOfFloors()
    {
        int count = 0;
        foreach (gridSpace space in grid)
        {
            if (space == gridSpace.floor)
            {
                count++;
            }
        }
        return count;
    }
    Vector2 RandomDirection()
    {
        //pick random int between 0 and 3
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        //use that int to chose a direction
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }
    void Spawn(float x, float y, GameObject toSpawn)
    {
        //find the position to spawn
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset;
        //spawn object
        Instantiate(toSpawn, spawnPos, Quaternion.identity);
    }
    void SpawnExplosive(float x, float y, GameObject toSpawn)
    {
        //find the position to spawn
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset;

        //destroy walls around
        Collider2D[] hitWall = Physics2D.OverlapCircleAll(new Vector2(x, y), explosionRange, wallLayer);

        foreach (Collider2D wall in hitWall)
        {
            Destroy(wall.gameObject);
        }

        //spawn object
        Instantiate(toSpawn, spawnPos, Quaternion.identity);
    }
    void Update()
    {
        GameObject[] enemiess = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesLeft = enemiess.Length;

        if (enemiesLeft <= 0 && !bossSpawned)
        {
            CreateBoss();
            SpawnBoss();
            bossSpawned = true;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //spawn portal
        }
    }
}
