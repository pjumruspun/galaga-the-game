using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueEnemyBehavior : MonoBehaviour
{
    // Controllable speed
    public float blueEnemySpeedMultiplier = 1.0f;

    // Essential Components
    private CharacterController controller;
    public GameObject player;
    public GameObject originalPos;
    public GameObject spawnLocationLeft;
    public GameObject spawnLocationRight;

    // Constants
    private const float repositionSpeed = 5.0f;
    private const float verticalSpeed = 3.0f;
    private const float horizontalSpeed = 1.2f;
    private const float leftBound = -3.7f;
    private const float rightBound = 3.7f;
    private const float upperBound = 5.2f;
    private const float lowerBound = -6f;
    private const float repositionDistance = 0.05f;

    // Controlling Variables
    Vector3 move;

    // State Variable
    enum State
    {
        Idle, Chase, Destroy, Relocation, Reposition
    }
    State currentState;

    // Checking Variable
    private const int ensureRelocation = 60;
    private int relocationCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    private void HandleState()
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

    private void FlyToPlayer()
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
        /*
        try
        {
            playerX = player.transform.position.x;
        }
        catch (MissingReferenceException)
        {
            player = GameObject.FindGameObjectWithTag("Player");  
        }
        catch (NullReferenceException)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerX = 0.0f;
        }*/
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

    private void Reposition()
    {
        Vector2 diff = originalPos.transform.position - transform.position;
        transform.up = diff;
        move = Vector3.zero;
        move = Vector3.Normalize(diff);
        Debug.DrawLine(gameObject.transform.position, originalPos.transform.position);
        controller.Move(move * Time.deltaTime * repositionSpeed * blueEnemySpeedMultiplier);
    }

    private bool IsRepositioned()
    {
        float dist = Vector3.Distance(originalPos.transform.position, gameObject.transform.position);
        return dist < repositionDistance;
    }

    private void HandleBounds()
    {
        Vector3 currentPosition = gameObject.transform.position;
        if (currentPosition.x < leftBound || currentPosition.x > rightBound || currentPosition.y > upperBound || currentPosition.y < lowerBound)
        {
            currentState = State.Relocation;
        }
    }

    public void ChasePlayer()
    {
        currentState = State.Chase;
    }

    public void LookAtPlayer()
    {
        transform.up = player.transform.position - transform.position;
    }

    public void ResetRotation()
    {
        transform.up = Vector3.down;
    }
}
