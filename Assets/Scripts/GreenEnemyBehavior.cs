using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemyBehavior : EnemyBehavior
{
    // Adjustable speed
    [SerializeField]
    private float greenEnemySpeedMultiplier = 1.0f;

    // Essential Components
    private PlayerLife playerLife;
    [SerializeField]
    private GameObject speedPowerUp;

    // Constants
    private const float laserPosAdjustFactor = 3.1f;
    private const float verticalSpeed = 5.0f;

    // State Variable
    private enum State
    {
        Idle, Chase, Destroy, Relocation, Reposition, Shooting
    }
    private State currentState;

    // Shooting Variables
    [SerializeField]
    private GameObject rocket;
    [SerializeField]
    private GameObject laser;
    private GameObject currentLaserObj;
    private const float shootingDistanceY = 2.0f;
    private const float timeBeforeShooting = 0.5f;
    private const float timeAfterShooting = 0.5f;
    private const float laserTime = 3.0f;
    private float currentLaserTime;
    private bool isShootingLaser;
    private const float shootingHorizontalSpeed = 0.4f;
    private bool isLaserRendererEnabled;
    private int currentLaserFrameCount;
    private const int numberOfFramesToToggle = 5;

    // Start is called before the first frame update
    void Start()
    {
        currentLaserFrameCount = 0;
        isLaserRendererEnabled = true;
        isShootingLaser = false;
        currentLaserTime = 0.0f;
        managerScript = GameObject.Find("Game Manager").GetComponent<ManagerScript>();
        playerLife = GameObject.Find("Game Manager").GetComponent<PlayerLife>();
        this.gameObject.tag = "GreenEnemy";
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
        greenEnemySpeedMultiplier = managerScript.GetEnemySpeedMult();
        if (greenEnemySpeedMultiplier < 0) greenEnemySpeedMultiplier = 0;
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
            Relocation();
        }
        else if (currentState == State.Shooting)
        {
            ResetRotation();
            if(currentLaserTime < laserTime + timeAfterShooting && !playerLife.IsGameOver())
            {
                transform.parent = null;
                float curX = gameObject.transform.position.x;
                float playerX = 0.0f;

                if(player != null)
                {
                    playerX = player.transform.position.x;
                }
                else
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                    playerX = 0.0f;
                }
                move = Vector3.zero;
                float randomSpeedFactor = UnityEngine.Random.Range(0.9f, 1.1f);
                if (playerX < curX) // Player is on left side of the enemy (from player's perspective)
                {
                    move += Vector3.left * shootingHorizontalSpeed * greenEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on LHS
                }
                else if (playerX > curX) // Player is on right side of the enemy (from player's perspective)
                {
                    move += Vector3.right * shootingHorizontalSpeed * greenEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on RHS
                }

                if(currentLaserTime > timeBeforeShooting && !isShootingLaser && currentLaserTime < laserTime)
                {
                    // Debug.Log("Gonna SHOOT");
                    isShootingLaser = true;
                    Shoot(1);
                    isLaserRendererEnabled = true;
                    currentLaserObj = Instantiate(laser, transform.position + Vector3.down * laserPosAdjustFactor, Quaternion.identity);
                    currentLaserObj.transform.parent = transform;
                }
                if(currentLaserTime > laserTime && isShootingLaser)
                {
                    // Debug.Log("STOP");
                    isShootingLaser = false;
                    Destroy(currentLaserObj);
                    currentLaserFrameCount = 0;
                }

                if (currentLaserObj != null)
                {
                    // Debug.Log("Shooting");
                    CharacterController laserController = currentLaserObj.GetComponent<CharacterController>();
                    isLaserRendererEnabled = !isLaserRendererEnabled;
                    SpriteRenderer s = currentLaserObj.GetComponentInChildren<SpriteRenderer>();
                    if(currentLaserFrameCount % numberOfFramesToToggle == 0)
                    {
                        Shoot(1);
                        s.enabled = isLaserRendererEnabled;
                    }
                        
                    ++currentLaserFrameCount;
                    laserController.Move(move * Time.deltaTime);
                }

                controller.Move(move * Time.deltaTime);
            }
            else
            {
                transform.parent = originalPos.transform;
                if (transform.parent == null) Debug.Log("THROW ERROR");
                currentLaserTime = 0.0f;
                currentState = State.Reposition;
            }
            currentLaserTime += Time.deltaTime;
        }
    }

    override protected void FlyToPlayer()
    {

        float curX = gameObject.transform.position.x;
        float playerX = 0.0f;
        float diffY = 999.0f;

        if(player != null)
        {
            playerX = player.transform.position.x;
            diffY = Mathf.Abs(player.transform.position.y - transform.position.y);
            LookAtPlayer();
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerX = 0.0f;
            ResetRotation();
        }
        

        if (diffY <= shootingDistanceY)
        {
            currentState = State.Shooting;
        }

        move = Vector3.zero;
        float randomSpeedFactor = UnityEngine.Random.Range(0.9f, 1.1f);
        move += Vector3.down * verticalSpeed * greenEnemySpeedMultiplier * randomSpeedFactor;
        if (playerX < curX) // Player is on left side of the enemy (from player's perspective)
        {
            move += Vector3.left * horizontalSpeed * greenEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on LHS
        }
        else if (playerX > curX) // Player is on right side of the enemy (from player's perspective)
        {
            move += Vector3.right * horizontalSpeed * greenEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on RHS
        }

        controller.Move(move * Time.deltaTime);
    }

    private void Relocation()
    {
        ResetRotation();
        int randDir = UnityEngine.Random.Range(0, 2);
        float dist = 999.0f;
        if (randDir < 1)
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

    override protected void Reposition()
    {
        Vector2 diff = originalPos.transform.position - transform.position;
        transform.up = diff;
        move = Vector3.zero;
        move = Vector3.Normalize(diff);
        Debug.DrawLine(transform.position, originalPos.transform.position);
        controller.Move(move * Time.deltaTime * repositionSpeed * greenEnemySpeedMultiplier);
    }

    override protected void HandleBounds()
    {
        Vector3 currentPosition = gameObject.transform.position;
        if (currentPosition.x < leftBound || currentPosition.x > rightBound || currentPosition.y > upperBound || currentPosition.y < lowerBound)
        {
            currentState = State.Relocation;
        }
    }

    private void Shoot(int n)
    {
        if (n < 1) n = 1;
        if (n == 1)
        {
            GameObject newRocket = Instantiate(rocket, transform.position + Vector3.down * 1.2f, Quaternion.identity);
            EnemyProjectileBehavior e = newRocket.GetComponent<EnemyProjectileBehavior>();
            foreach (Transform child in newRocket.transform)
            {
                SpriteRenderer s = child.GetComponent<SpriteRenderer>();
                s.enabled = false;
            }
            e.ChangeDirection(Vector3.down);
        }
    }

    override public void ChasePlayer()
    {
        currentState = State.Chase;
    }

    public GameObject GetOriginalPosGameObject()
    {
        return originalPos;
    }
}
