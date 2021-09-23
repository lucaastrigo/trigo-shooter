using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour 
{
    public GameObject[] enemies;
    public GameObject portal;
    public static int skillRooms, abilityRooms, weaponRooms;
    public static int maxRooms;

    public int _skillRooms, _abilityRooms, _weaponRooms;
    public int _maxRooms;
    public GameObject[] top;
	public GameObject[] bottom;
	public GameObject[] right;
	public GameObject[] left;
	public GameObject closedRoom;
    public GameObject skillRoom, abilityRoom, weaponRoom;
	public List<GameObject> rooms;

    float waitTime = 2;
    bool portalSpawned;

    private void Start()
    {
        skillRooms = _skillRooms;
        abilityRooms = _abilityRooms;
        weaponRooms = _weaponRooms;
        maxRooms = _maxRooms;
    }

    private void Update()
    {
        if(waitTime <= 0)
        {
            if (allWavesFinished() && !portalSpawned)
            {
                //open locked door on starter room
                Instantiate(portal, new Vector2(rooms[0].gameObject.transform.position.x, rooms[0].gameObject.transform.position.y + 3), Quaternion.identity);
                portalSpawned = true;
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }

    bool allWavesFinished()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].GetComponent<Room>().thisRoom == Room.roomType.wave && !rooms[i].GetComponent<Room>().finishedWave)
            {
                return false;
            }
        }

        return true;
    }
}
