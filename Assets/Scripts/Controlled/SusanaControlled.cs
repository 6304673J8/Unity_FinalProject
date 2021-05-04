﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class SusanaControlled : MonoBehaviour
{
    //testing 
    public bool saver;

    PlayerInputs controls;

    //Movement
    Vector2 move;
    public bool canMove = false;
    bool facingRight = true;

    //hp
    public int health;
    public int level;

    //inventory
    public int nPotions;
    public int nKeys;
    public int nBombs;

    [SerializeField] private float lungeDistance;

    //Animation
    Animator anim;
    Rigidbody2D rb;

    bool isMoving;


    //Collision Check
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private Tilemap collisionTilemap;
    [SerializeField]
    private Tilemap damagingTilemap;
    [SerializeField]
    private Tilemap healingTilemap;
    [SerializeField] float speed;

    [Header("Abilities")]

    public GameObject earthquakePrefab;
    public GameObject lungePrefab;
    public GameObject potionPrefab;
    public AbilitiesControlled abilities;
    public bool defending;
    public bool lunging;
    public bool quaking;
    //new
    // shows rounded position = tile position

    private void Awake()
    {
        //testing 
        saver = false;

        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerInputs();

        //items 
        nPotions = 0;
        nKeys = 0;
        nBombs = 0;

        #region INPUCTACTIONS
        controls.Susana.Move.performed += ctx => SendMessage(ctx.ReadValue<Vector2>());

        controls.Susana.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Susana.Move.canceled += ctx => move = Vector2.zero;

        //controls.Susana.Action.performed += ctx => Interact();

        //Test Abilities
        controls.Susana.Lunge.performed += ctx => Lunge();
        controls.Susana.Shield.started += ctx => Shield();
        controls.Susana.Shield.canceled += ctx => Shield();
        controls.Susana.Earthquake.started += ctx => Earthquake();
        #endregion
    }

    private void OnEnable()
    {
        controls.Susana.Enable();
    }
    private void OnDisable()
    {
        controls.Susana.Disable();
    }

    void SendMessage(Vector2 coordinates)
    {
        Debug.Log("Thumb-stick coordinates" + coordinates);
    }
    private void Update()
    {
        if (saver == true)
        {
            Debug.Log("CULAZO CRIS");
            LoadPlayer();
        }
    }
    void FixedUpdate()
    {
        Vector3Int gridPos = floorTilemap.WorldToCell(transform.position + (Vector3)move);

        if (move.x != 0 || move.y != 0)
        {
            if (facingRight == false && move.x > 0)
            {

                Flip();

            }
            else if (facingRight == true && move.x < 0)
            {
                Flip();
            }
            if (floorTilemap.HasTile(gridPos) || !collisionTilemap.HasTile(gridPos))
            {
                Debug.Log("Moving");
                //move = new Vector2Int(Mathf.FloorToInt(move.x), Mathf.FloorToInt(move.y));
                //rb.velocity = new Vector2(move.x * speed * Time.fixedDeltaTime, rb.velocity.y);
                transform.position += (Vector3)move * speed * Time.fixedDeltaTime;
            }
        }
        else
        {
            //Debug.Log("PROTECT");
            isMoving = false;
        }
    }

    #region SKILLS
    public void Lunge()
    {
        Debug.Log("Topetazo!");
        move = controls.Susana.Move.ReadValue<Vector2>();
        if (CanLunge(move) && abilities.lungeCooldown == false)
        {
            LungeLogic();
            transform.position += (Vector3)move * lungeDistance;
            //rb.velocity = new Vector2(lungeDistance, rb.velocity.y);
        }
    }

    private bool CanLunge(Vector2 lungeToNext)
    {
        Vector3Int gridPos = floorTilemap.WorldToCell(transform.position + (Vector3)move);
        Vector3Int lungeGridPos = floorTilemap.WorldToCell(transform.position + (Vector3)move * 2);
        if (!floorTilemap.HasTile(gridPos) || collisionTilemap.HasTile(gridPos) ||
            !floorTilemap.HasTile(lungeGridPos) || collisionTilemap.HasTile(lungeGridPos))
        {
            return false;
        }
        else if (move.x == 0 && move.y == 0)
        {
            return false;
        }
        lunging = true;
        return true;
    }

    public void LungeLogic()
    {
        Vector2 pos = transform.position;

        GameObject lungeFX = Instantiate(lungePrefab, pos, transform.rotation);
    }

    public void Earthquake()
    {
        quaking = true;
        if (abilities.earthquakeCooldown == false)
        {
            quaking = true;
            //sprite.color = new Color(0, 0, 1, 1);
            EarthquakeLogic();
            Camera.main.GetComponent<CameraFollow>().shakeDuration = 0.2f;
        }
        //sprite.color = new Color(1, 1, 1, 1);
        //Camera.main.GetComponent<CameraShake>().shakeDuration = 0.2f;
        //gameHandler.GetComponent<CameraShake>().shakeDuration = 0.2f;
    }

    public void EarthquakeLogic()
    {
        Vector2 pos = transform.position;

        GameObject earthquakeFX = Instantiate(earthquakePrefab, pos, transform.rotation);
    }


    private void Shield()
    {
        Debug.Log("Topetazo!");
    }

    #endregion

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    public void SavePlayer()
    {
        SaveSystem.SaveSusana(this);
    }

    public void LoadPlayer()
    {
        SusanaData data = SaveSystem.LoadSusana();

        //level = susana.level;
        health = data.health;
        nPotions = data.potions;
        nKeys = data.keys;
        nBombs = data.bombs;
        controls = new PlayerInputs();
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "SavePoint")
        {
            SavePlayer();
        }

        if(collision.tag == "Fireball")
        {
            health -= 20;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "SavePoint")
        {
            if (saver == true)
            {
                LoadPlayer();
            }
        }
    }

    /*public void Earthquake()
    {
        Debug.Log("BROOOM");
    }

    public void Lunge()
    {
        Debug.Log("Lunging");
        //transform.position += (Vector3)axis * 2;
    }

    private void CheckFlip()
    {
        if (facingRight == false && axis.x > 0)
        {

            Flip();

        }
        else if (facingRight == true && axis.x < 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    public void SetAxis(Vector2 _axis)
    {
        print(axis);
        axis = _axis;
    }
    public bool IsMoving()
    {
        return isMoving;
    }*/
    /*
     *
     *https://www.youtube.com/watch?v=oBvWOHHbzJ0&list=PLzDRvYVwl53vXmpctKrMQTxA3cQwGcAk2&index=5
     private void Move(Vector2 dir)
    {
        bool isIdle = dir.x == 0 && dir.y == 0;
        if (isIdle)
        {
            Debug.Log("Animación IDLE");
        }
        else
        {
            //si falla quitar normalized
            Vector3 movementDir = new Vector3(dir.x, dir.y);

            bool canMove = CanMove(movementDir, speed * Time.deltaTime);
            if (TestMove(movementDir,speed * Time.deltaTime))
            {
                Debug.Log("SIIIIIII" + dir);
                //transform.position += movementDir;
                //dir = new Vector2Int(Mathf.FloorToInt(dir.x), Mathf.FloorToInt(dir.y));
                //transform.position += (Vector3)dir;
            }
            else
            {
                Debug.Log("NOOOOOOOO" + dir);
                //transform.position += lastMoveDir;
            }
        }
    }
    private bool CanMove(Vector3 dir, float distance)
    {
        return Physics2D.Raycast(transform.position, dir, distance).collider == null;
    }

    private bool TestMove(Vector3 baseMoveDir, float distance)
    {
        Vector3 moveDir = baseMoveDir;
        bool canMove = CanMove(moveDir, distance);
        if (!canMove)
        {
            //cannot move diagonal
            moveDir = new Vector3(baseMoveDir.x, 0f);
            canMove = moveDir.x != 0f && CanMove(moveDir, distance);
            if (!canMove)
            {
                //cannot move horizontal
                moveDir = new Vector3(0, baseMoveDir.y);
                canMove = moveDir.y != 0f && CanMove(moveDir, distance);
            }
        }
        if (canMove)
        {
            lastMoveDir = moveDir;
            transform.position += moveDir * distance;
            return true;
        }
        else
        {
            return false;
        }
    }
     */
}
