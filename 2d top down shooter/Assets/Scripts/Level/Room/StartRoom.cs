using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : MonoBehaviour
{
    // IF THE ROOM IS ACTIVE, CLOSE DOORS AND CREATE A BLACK PANEL AROUND 
    // PUBLIC BOOL TO SEE IF A ROOM WAS ALREADY ENTERED BY THE PLAYER

    public Vector2 roomSizeWorldUnits;

    [Header("Player")]
    public GameObject player;

    [Header("Map")]
    public GameObject startFloor;
    public GameObject floor;

    int roomWidth, roomHeight;
    bool foundPlayerSpace = false;
    enum gridSpace { empty, floor, player, startFloor };
    gridSpace[,] grid;

    private void Start()
    {
        Setup();
        CreateFloors();
        CreatePlayer();
        SpawnRoom();
    }
    void Setup()
    {
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.x / 2);
        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.y / 2);

        grid = new gridSpace[roomWidth, roomHeight];

        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                grid[x, y] = gridSpace.empty;
            }
        }
    }
    void CreateFloors()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                grid[x, y] = gridSpace.floor;
            }
        }
    }
    void CreatePlayer()
    {
        print("criando");
        for (int x = 0; x < roomWidth; x++)
        {
            print("1");
            for (int y = 0; y < roomHeight; y++)
            {
                print("2");
                if (x == roomWidth / 2 && y == roomHeight / 2)
                {
                    foundPlayerSpace = true;
                    print("found player space");
                    grid[x, y] = gridSpace.player;
                }

                if (x >= roomWidth / 2 && y >= roomHeight / 2 && x <= roomWidth / 2 + 1 && y <= roomHeight / 2 + 1)
                {
                    grid[x, y] = gridSpace.startFloor;
                }
                //break;
            }

            if (foundPlayerSpace)
            {
                break;
            }
        }
    }
    void SpawnRoom()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.empty:
                        Spawn(x, y, floor);
                        break;
                    case gridSpace.floor:
                        Spawn(x, y, floor);
                        break;
                    case gridSpace.player:
                        Spawn(x, y, startFloor);
                        Spawn(x, y, player);
                        break;
                    case gridSpace.startFloor:
                        Spawn(x, y, startFloor);
                        break;
                }
            }
        }
    }
    void Spawn(float x, float y, GameObject toSpawn)
    {
        Vector2 offset = (roomSizeWorldUnits + new Vector2(2, 2)) / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * 2 - offset;
        Instantiate(toSpawn, spawnPos, Quaternion.identity);
    }
    void Update()
    {
        //
    }
}
