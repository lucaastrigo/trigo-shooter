using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour 
{
    public static int skillRooms, weaponRooms;
    public static int maxRooms;

    public int _skillRooms, _weaponRooms;
    public int _maxRooms;
    public GameObject[] top;
	public GameObject[] bottom;
	public GameObject[] right;
	public GameObject[] left;
    public GameObject tsecret, bsecret, rsecret, lsecret;
	public GameObject closedRoom;
    public GameObject skillRoom, weaponRoom;
	public List<GameObject> rooms;

    float waitTime = 2;
    bool setBossRoom;

    private void Start()
    {
        skillRooms = _skillRooms;
        weaponRooms = _weaponRooms;
        maxRooms = _maxRooms;
    }

    private void Update()
    {
        if(waitTime <= 0 && !setBossRoom)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if(i == rooms.Count - 1)
                {
                    //Instantiate(boss, rooms[i].transform.position, Quaternion.identity);
                    setBossRoom = true;
                }
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }
}
