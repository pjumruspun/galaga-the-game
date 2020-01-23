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
        controller = GetComponent<CharacterController>();
        currentState = State.Relocation;
        player = GameObject.Find("Player");
        spawnLocationLeft = GameObject.Find("SpawnLocationLeft");
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
                currentState = State.Idle;
            }
        }
        else if (currentState == State.Relocation)
        {
            transform.position = spawnLocationLeft.transform.position;
            float dist = Vector3.Distance(spawnLocationLeft.transform.position, transform.position);
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
            playerX = 0.0f;
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

    private void Reposition()
    {
        move = Vector3.zero;
        move = Vector3.Normalize(originalPos.transform.position - gameObject.transform.position);
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
}
