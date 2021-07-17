using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour
{
    public float speed;
    public GameObject sideCart, frontCart;
    public Transform[] waypoint;


    [Header("Side Cart")]
    public Transform sideCartSeat;
    public GameObject sideMinecartFront, rightWheel, leftWheel;

    [Header("Front Cart")]
    public Transform frontCartSeat;
    public GameObject frontMinecartFront;


    int wayNo;
    float xMove, yMove;
    bool onCartTrigger, onCart, sided, rightFaced, moving;
    Transform nextWaypoint;
    GameObject player;
    Rigidbody2D rb;
    Vector3 startingPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        sided = true;
        nextWaypoint = waypoint[0];
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, nextWaypoint.position, speed * Time.deltaTime);

        if(Vector2.Distance(transform.position, nextWaypoint.position) <= 0f)
        {
            if(wayNo < waypoint.Length - 1)
            {
                wayNo++;
            }
            else
            {
                wayNo = 0;
            }

            nextWaypoint = waypoint[wayNo];
        }

        rightWheel.transform.Rotate(0, 0, 1);
        leftWheel.transform.Rotate(0, 0, 1);

        if (onCartTrigger && !onCart)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EnterCart();
            }
        }
        else if (onCart)
        {
            if (sided)
            {
                player.transform.position = sideCartSeat.position;
            }
            else
            {
                player.transform.position = frontCartSeat.position;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitCart();
            }

            if (sided)
            {
                sideMinecartFront.GetComponent<SpriteRenderer>().sortingOrder = 7;
                rightWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
                leftWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
            }
            else
            {
                frontMinecartFront.GetComponent<SpriteRenderer>().sortingOrder = 7;
                rightWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
                leftWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
            }
        }

        if (Mathf.Abs(transform.position.x - nextWaypoint.position.x) < 0.2f)
        {
            FrontCart();
        }

        if (Mathf.Abs(transform.position.y - nextWaypoint.position.y) < 0.2f)
        {
            SideCart();
        }
    }

    void EnterCart()
    {
        if (sided)
        {
            player.transform.position = sideCartSeat.position;
            sideMinecartFront.GetComponent<SpriteRenderer>().sortingOrder = 7;
            rightWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
            leftWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
        }
        else
        {
            player.transform.position = frontCartSeat.position;
            frontMinecartFront.GetComponent<SpriteRenderer>().sortingOrder = 7;
            rightWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
            leftWheel.GetComponent<SpriteRenderer>().sortingOrder = 8;
        }

        onCart = true;
        player.transform.parent = transform;
        player.GetComponent<PlayerMovement>().friction = 0;
    }

    void ExitCart()
    {
        if (sided)
        {
            sideMinecartFront.GetComponent<SpriteRenderer>().sortingOrder = 2;
            rightWheel.GetComponent<SpriteRenderer>().sortingOrder = 3;
            leftWheel.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
        else
        {
            frontMinecartFront.GetComponent<SpriteRenderer>().sortingOrder = 2;
            rightWheel.GetComponent<SpriteRenderer>().sortingOrder = 3;
            leftWheel.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }

        onCart = false;
        player.transform.parent = null;
        player.GetComponent<PlayerMovement>().friction = 1;
    }

    void SideCart()
    {
        sided = true;
        sideCart.SetActive(true);
        frontCart.SetActive(false);
    }

    void FrontCart()
    {
        sided = false;
        sideCart.SetActive(false);
        frontCart.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onCartTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onCartTrigger = false;
        }
    }
}
