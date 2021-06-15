using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public static bool mapped;
    public static GameObject map, minimap;

    GameObject roomGenerator, mapCamera, minimapCamera;

    void Start()
    {
        roomGenerator = GameObject.FindGameObjectWithTag("Rooms");
        map = GameObject.FindGameObjectWithTag("Map");
        minimap = GameObject.FindGameObjectWithTag("Minimap");

        mapCamera = GameObject.FindGameObjectWithTag("MapCamera");
        minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera");

        map.transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        mapped = map.transform.GetChild(0).gameObject.activeInHierarchy;

        if (!inWave())
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleMinimap();
            }
        }
    }

    bool inWave()
    {
        if(roomGenerator != null)
        {
            for (int i = 0; i < roomGenerator.GetComponent<RoomTemplates>().rooms.Count; i++)
            {
                if (roomGenerator.GetComponent<RoomTemplates>().rooms[i].GetComponent<Room>().inWave)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    void ToggleMinimap()
    {
        mapCamera.transform.localPosition = minimapCamera.transform.localPosition;
        map.transform.GetChild(0).gameObject.SetActive(!map.transform.GetChild(0).gameObject.activeInHierarchy);
        minimap.SetActive(!minimap.activeInHierarchy);
    }
}
