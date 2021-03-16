﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    private float timerSpeed = 2f; //Este valor define los segundos del temporizador

    public float attackDelay = 1;

    private float lastAttackTime;

    private float elapsed;

    private float elapsedCD;

    public Transform player;

    public int damage;

    Rigidbody2D rb;

    [SerializeField] float speed;

    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    int directionIndex = 0;

    //Ver la direccion actual
    [SerializeField] Vector2 currentDir;

    //La distancia a la que va a mirar el raycast
    [SerializeField] float rayDistance;

    [SerializeField] LayerMask rayLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        currentDir = directions[directionIndex];
        elapsed += Time.deltaTime;
    }


    private void Update()
    {
        //Raycast
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, currentDir,rayDistance, rayLayer);
        Vector3 endpoint = currentDir * rayDistance;
        Debug.DrawLine(transform.position, transform.position + endpoint, Color.green);

        if(hit2D.collider != null)
        {
            if(hit2D.collider.gameObject.CompareTag("Wall"))
            {
                ChangeDirection();
            }

            if (hit2D.collider.gameObject.CompareTag("Breakable"))
            {
                ChangeDirection();
            }

            if (hit2D.collider.gameObject.CompareTag("Susana"))
            {
                attackPlayer();
               
            }
        }


        elapsed += Time.deltaTime;
        if(elapsed>=timerSpeed)
        {
            elapsed = 0f;
            ChangeDirection();
        }

    }


    void ChangeDirection()
    {
        //Decide una dirección de forma aleatoria (En el array de direcciones)
        directionIndex += Random.Range(0, 3) * 3 - 1;

        //Hace que el índice no supere 3
        int clampedIndex = directionIndex % directions.Length;

        //Hace que el índice sea positivo
        if (clampedIndex < 0)
        {
            clampedIndex = directions.Length + clampedIndex;
        }
        rb.velocity = Vector2.zero;

        currentDir = directions[clampedIndex];
    }
    private void FixedUpdate()
    {
        rb.AddForce(currentDir * speed); 
    }

    void attackPlayer()
    {
        if (Time.time > lastAttackTime + attackDelay)
        {
            player.SendMessage("TakeDamage", damage);
            lastAttackTime = Time.time;
        }
    }

}