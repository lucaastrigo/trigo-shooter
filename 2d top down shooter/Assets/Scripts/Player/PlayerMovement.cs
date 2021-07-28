using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public GameObject sprintFX;
    public Transform holster;
    public Transform secondHolster;
    public Transform rightHolster;
    public Transform leftHolster;
    [HideInInspector] public float _speed;
    [HideInInspector] public float xMove;
    [HideInInspector] public float yMove;
    [HideInInspector] public float friction = 1;
    bool rightFaced;
    [HideInInspector] public bool moving;
    Vector2 mousePos;
    [HideInInspector] public Vector2 dashDirection;
    Camera cam;
    Animator anim;
    Rigidbody2D rb;
    AudioSource audioSource;
    GameObject skillStorage;
    SpriteRenderer sprite;

    void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        _speed = speed;
    }

    void FixedUpdate()
    {
        xMove = Input.GetAxis("Horizontal");
        yMove = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(xMove * speed * friction, yMove * speed * friction);
    }

    void Update()
    {
        skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");

        if (!PauseMenu.paused)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (xMove != 0 || yMove != 0)
            {
                moving = true;
                anim.SetBool("walk", true);
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                moving = false;
                anim.SetBool("walk", false);
                audioSource.Stop();
            }

            if (mousePos.x < transform.position.x)
            {
                if (!rightFaced)
                {
                    Flip();
                }
            }

            if (mousePos.x > transform.position.x)
            {
                if (rightFaced)
                {
                    Flip();
                }
            }

            if (xMove > 0)
            {
                dashDirection = new Vector2(transform.position.x + 5, transform.position.y);
            }

            if (xMove < 0)
            {
                dashDirection = new Vector2(transform.position.x - 5, transform.position.y);
            }

            if (yMove > 0)
            {
                dashDirection = new Vector2(transform.position.x, transform.position.y + 5);
            }

            if (yMove < 0)
            {
                dashDirection = new Vector2(transform.position.x, transform.position.y - 5);
            }

            if (xMove > 0 && yMove > 0)
            {
                dashDirection = new Vector2(transform.position.x + 5, transform.position.y + 5);
            }

            if (xMove < 0 && yMove > 0)
            {
                dashDirection = new Vector2(transform.position.x - 5, transform.position.y + 5);
            }

            if (xMove > 0 && yMove < 0)
            {
                dashDirection = new Vector2(transform.position.x + 5, transform.position.y - 5);
            }

            if (xMove < 0 && yMove < 0)
            {
                dashDirection = new Vector2(transform.position.x - 5, transform.position.y - 5);
            }
        }
    }

    void Flip()
    {
        rightFaced = !rightFaced;

        transform.GetComponent<SpriteRenderer>().flipX = !transform.GetComponent<SpriteRenderer>().flipX;

        if (rightFaced)
        {
            holster.transform.position = leftHolster.transform.position;
            secondHolster.transform.position = rightHolster.transform.position;
        }
        else
        {
            holster.transform.position = rightHolster.transform.position;
            secondHolster.transform.position = leftHolster.transform.position;
        }
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Friction Floor"))
        {
            if(collision.gameObject.GetComponent<FrictionFloor>() != null)
            {
                if (skillStorage.GetComponentInChildren<LongLegsSkill>().skill.GetComponent<Skill>().skillOn)
                {
                    friction = 1;
                }
                else
                {
                    friction = collision.gameObject.GetComponent<FrictionFloor>().friction;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Friction Floor"))
        {
            if (collision.gameObject.GetComponent<FrictionFloor>() != null)
            {
                friction = 1;
            }
        }
    }
    */
}