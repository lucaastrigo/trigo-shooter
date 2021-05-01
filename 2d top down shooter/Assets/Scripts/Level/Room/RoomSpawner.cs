using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour 
{
	public int openingDirection;

	RoomTemplates templates;
	private int rand;
	public bool spawned = false;
	public bool secretRoom;

	public float waitTime = 4f;

	bool roomed;

	void Start()
    {
		Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }

    private void Update()
    {
        if(templates == null)
        {
			templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		}
	}

    void Spawn()
    {
		if(RoomTemplates.maxRooms > 0)
        {
			if (spawned == false)
			{
                if (secretRoom)
                {
					if (openingDirection == 1)
					{
						Instantiate(templates.tsecret, transform.position, templates.tsecret.transform.rotation);

					}
					else if (openingDirection == 2)
					{
						Instantiate(templates.bsecret, transform.position, templates.bsecret.transform.rotation);

					}
					else if (openingDirection == 3)
					{
						Instantiate(templates.rsecret, transform.position, templates.rsecret.transform.rotation);

					}
					else if (openingDirection == 4)
					{
						Instantiate(templates.lsecret, transform.position, templates.lsecret.transform.rotation);
					}
                }
                else
                {
					if (RoomTemplates.weaponRooms > 0)
					{
						if (Random.Range(0, 100) <= 20)
						{
							Instantiate(templates.weaponRoom, transform.position, templates.weaponRoom.transform.rotation);
							RoomTemplates.weaponRooms--;
						}
						else
						{
							if (openingDirection == 1)
							{
								rand = Random.Range(0, templates.top.Length);
								Instantiate(templates.top[rand], transform.position, templates.top[rand].transform.rotation);

							}
							else if (openingDirection == 2)
							{
								rand = Random.Range(0, templates.bottom.Length);
								Instantiate(templates.bottom[rand], transform.position, templates.bottom[rand].transform.rotation);

							}
							else if (openingDirection == 3)
							{
								rand = Random.Range(0, templates.right.Length);
								Instantiate(templates.right[rand], transform.position, templates.right[rand].transform.rotation);

							}
							else if (openingDirection == 4)
							{
								rand = Random.Range(0, templates.left.Length);
								Instantiate(templates.left[rand], transform.position, templates.left[rand].transform.rotation);
							}
						}
					}
					else if (RoomTemplates.skillRooms > 0)
					{
						if (Random.Range(0, 100) <= 15)
						{
							Instantiate(templates.skillRoom, transform.position, templates.skillRoom.transform.rotation);
							RoomTemplates.skillRooms--;
						}
						else
						{
							if (openingDirection == 1)
							{
								rand = Random.Range(0, templates.top.Length);
								Instantiate(templates.top[rand], transform.position, templates.top[rand].transform.rotation);

							}
							else if (openingDirection == 2)
							{
								rand = Random.Range(0, templates.bottom.Length);
								Instantiate(templates.bottom[rand], transform.position, templates.bottom[rand].transform.rotation);

							}
							else if (openingDirection == 3)
							{
								rand = Random.Range(0, templates.right.Length);
								Instantiate(templates.right[rand], transform.position, templates.right[rand].transform.rotation);

							}
							else if (openingDirection == 4)
							{
								rand = Random.Range(0, templates.left.Length);
								Instantiate(templates.left[rand], transform.position, templates.left[rand].transform.rotation);
							}
						}
					}
					else
					{
						if (openingDirection == 1)
						{
							rand = Random.Range(0, templates.top.Length);
							Instantiate(templates.top[rand], transform.position, templates.top[rand].transform.rotation);

						}
						else if (openingDirection == 2)
						{
							rand = Random.Range(0, templates.bottom.Length);
							Instantiate(templates.bottom[rand], transform.position, templates.bottom[rand].transform.rotation);

						}
						else if (openingDirection == 3)
						{
							rand = Random.Range(0, templates.right.Length);
							Instantiate(templates.right[rand], transform.position, templates.right[rand].transform.rotation);

						}
						else if (openingDirection == 4)
						{
							rand = Random.Range(0, templates.left.Length);
							Instantiate(templates.left[rand], transform.position, templates.left[rand].transform.rotation);
						}
					}
				}

				spawned = true;
				RoomTemplates.maxRooms--;
			}
        }
        else
        {
            if (!roomed && !spawned)
            {
				Instantiate(templates.closedRoom, transform.position, templates.closedRoom.transform.rotation);
				spawned = true;
			}
		}
	}

	void SpawnBlock()
    {
		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();

		Instantiate(templates.closedRoom, transform.position, templates.closedRoom.transform.rotation);
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
    {
		if (other.CompareTag("SpawnPoint") && !other.CompareTag("Room"))
        {
			if(other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
				SpawnBlock();
			}

			spawned = true;
		}

        if (other.CompareTag("Room"))
        {
			roomed = true;
        }
	}
}
