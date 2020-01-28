using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueEnemyBehavior : EnemyBehavior
{
    // Overall Speed
    private float blueEnemySpeedMultiplier = 1.0f;

    // Constants
    private const float verticalSpeed = 3.0f;

    // State Variable
    private enum State
    {
        Idle, Chase, Destroy, Relocation, Reposition
    }
    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManagerScript>();
        this.gameObject.tag = "Enemy";
        controller = GetComponent<CharacterController>();
        currentState = State.Relocation;
        player = GameObject.FindGameObjectWithTag("Player");
        spawnLocationLeft = GameObject.Find("SpawnLocationLeft");
        spawnLocationRight = GameObject.Find("SpawnLocationRight");
        originalPos = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();
        blueEnemySpeedMultiplier = managerScript.GetEnemySpeedMult();
        if (blueEnemySpeedMultiplier < 0) blueEnemySpeedMultiplier = 0;
    }

    override protected void HandleState()
    {
        if (currentState == State.Idle)
        {

        }
        else if (currentState == State.Chase)
        {
            FlyToPlayer();
            HandleBounds();
        }
        else if (currentState == State.Reposition)
        {
            if (!IsRepositioned())
            {
                Reposition();
            }
            else
            {
                ResetRotation();
                currentState = State.Idle;
            }
        }
        else if (currentState == State.Relocation)
        {
            ResetRotation();
            int randDir = UnityEngine.Random.Range(0, 2);
            float dist = 999.0f;
            if(randDir < 1)
            {
                transform.position = spawnLocationLeft.transform.position;
                dist = Vector3.Distance(spawnLocationLeft.transform.position, transform.position);
            }
            else
            {
                transform.position = spawnLocationRight.transform.position;
                dist = Vector3.Distance(spawnLocationRight.transform.position, transform.position);
            }
            
            if (dist < 0.05f)
                ++relocationCounter;
            if (relocationCounter == ensureRelocation)
            {
                currentState = State.Reposition;
                relocationCounter = 0;
            }
        }
    }

    override protected void FlyToPlayer()
    {
        
        float curX = gameObject.transform.position.x;
        float playerX = 0.0f;

        if(player != null)
        {
            playerX = player.transform.position.x;
            LookAtPlayer();
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerX = 0.0f;
            ResetRotation();
        }

        move = Vector3.zero;
        float randomSpeedFactor = UnityEngine.Random.Range(0.9f, 1.1f);
        move += Vector3.down * verticalSpeed * blueEnemySpeedMultiplier * randomSpeedFactor;
        if (playerX < curX) // Player is on left side of the enemy (from player's perspective)
        {
            move += Vector3.left * horizontalSpeed * blueEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on LHS
        }
        else if (playerX > curX) // Player is on right side of the enemy (from player's perspective)
        {
            move += Vector3.right * horizontalSpeed * blueEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on RHS
        }
        controller.Move(move * Time.deltaTime);
    }

    override protected void Reposition()
    {
        Vector2 diff = originalPos.transform.position - transform.position;
        transform.up = diff;
        move = Vector3.zero;
        move = Vector3.Normalize(diff);
        Debug.DrawLine(gameObject.transform.position, originalPos.transform.position);
        controller.Move(move * Time.deltaTime * repositionSpeed * blueEnemySpeedMultiplier);
    }

    override protected void HandleBounds()
    {
        Vector3 currentPosition = gameObject.transform.position;
        if (currentPosition.x < leftBound || currentPosition.x > rightBound || currentPosition.y > upperBound || currentPosition.y < lowerBound)
        {
            currentState = State.Relocation;
        }
    }

    override public void ChasePlayer()
    {
        currentState = State.Chase;
    }
}
