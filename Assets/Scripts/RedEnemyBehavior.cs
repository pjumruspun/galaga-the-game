using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedEnemyBehavior : MonoBehaviour
{
    // Adjustable speed
    public float redEnemySpeedMultiplier = 1.0f;

    // Essential Components
    private CharacterController controller;
    private ManagerScript managerScript;
    private GameObject player;
    private GameObject originalPos;
    private GameObject spawnLocationLeft;
    private GameObject spawnLocationRight;

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
    private Vector3 move;

    // State Variable
    private enum State
    {
        Idle, Chase, Destroy, Relocation, Reposition
    }
    private State currentState;

    // Checking Variables
    private const int ensureRelocation = 60;
    private int relocationCounter = 0;

    // Shooting Variables
    [SerializeField]
    private GameObject rocket;
    private const float maxFanShootingDegree = 220.0f;
    private const float projectileRotationDegree = 30;
    private const float shootingDistanceY = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<ManagerScript>();
        this.gameObject.tag = "RedEnemy";
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
        redEnemySpeedMultiplier = managerScript.GetEnemySpeedMult();
        if (redEnemySpeedMultiplier < 0) redEnemySpeedMultiplier = 0;
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
    }

    private void FlyToPlayer()
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

        if(diffY <= shootingDistanceY)
        {
            Shoot(managerScript.GetNumberOfRockets());
            currentState = State.Reposition;
        }
        move = Vector3.zero;
        float randomSpeedFactor = UnityEngine.Random.Range(0.9f, 1.1f);
        move += Vector3.down * verticalSpeed * redEnemySpeedMultiplier * randomSpeedFactor;
        if (playerX < curX) // Player is on left side of the enemy (from player's perspective)
        {
            move += Vector3.left * horizontalSpeed * redEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on LHS
        }
        else if (playerX > curX) // Player is on right side of the enemy (from player's perspective)
        {
            move += Vector3.right * horizontalSpeed * redEnemySpeedMultiplier * randomSpeedFactor; // Chase the player on RHS
        }
        controller.Move(move * Time.deltaTime);
    }

    private void Reposition()
    {
        Vector2 diff = originalPos.transform.position - transform.position;
        transform.up = diff;
        move = Vector3.zero;
        move = Vector3.Normalize(diff);
        Debug.DrawLine(transform.position, originalPos.transform.position);
        controller.Move(move * Time.deltaTime * repositionSpeed * redEnemySpeedMultiplier);
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

    private void Shoot(int n)
    {
        if (n < 1) n = 1;
        if (n == 1)
        {
            Vector2 toPlayer = player.transform.position - transform.position; // Get Vec2 direction to player
            GameObject newRocket = Instantiate(rocket, transform.position, Quaternion.identity);
            EnemyProjectileBehavior e = newRocket.GetComponent<EnemyProjectileBehavior>();
            e.ChangeDirection(toPlayer);
        }
        else
        {
            float realProjectileRotationDegree;
            if(n > 7)
            {
                realProjectileRotationDegree = maxFanShootingDegree / (n - 1);
            }
            else
            {
                realProjectileRotationDegree = projectileRotationDegree;
            }


            if(n % 2 == 0)
            {
                for(int i = 0; i < n/2; ++i)
                {
                    
                    Vector2 toPlayer = player.transform.position - transform.position; // Get Vec2 direction to player
                    GameObject newRocket1 = Instantiate(rocket, transform.position, Quaternion.identity);
                    EnemyProjectileBehavior e1 = newRocket1.GetComponent<EnemyProjectileBehavior>();
                    GameObject newRocket2 = Instantiate(rocket, transform.position, Quaternion.identity);
                    EnemyProjectileBehavior e2 = newRocket2.GetComponent<EnemyProjectileBehavior>();
                    float rotationDeg = ((float)i + 0.5f) * realProjectileRotationDegree;

                    Vector2 left = Quaternion.AngleAxis(rotationDeg, Vector3.forward) * toPlayer;
                    Vector2 right = Quaternion.AngleAxis(-rotationDeg, Vector3.forward) * toPlayer;

                    Debug.DrawLine(transform.position, new Vector3(transform.position.x + left.x, transform.position.y + left.y, 0.0f));

                    e1.ChangeDirection(left);
                    e2.ChangeDirection(right);
                }
            }
            else
            {
                Vector2 toPlayer = player.transform.position - transform.position; // Get Vec2 direction to player
                for (int i = 0; i < n / 2; ++i)
                {
                    GameObject newRocket1 = Instantiate(rocket, transform.position, Quaternion.identity);
                    EnemyProjectileBehavior e1 = newRocket1.GetComponent<EnemyProjectileBehavior>();
                    GameObject newRocket2 = Instantiate(rocket, transform.position, Quaternion.identity);
                    EnemyProjectileBehavior e2 = newRocket2.GetComponent<EnemyProjectileBehavior>();
                    float rotationDeg = ((float)i + 1.0f) * realProjectileRotationDegree;

                    Vector2 left = Quaternion.AngleAxis(rotationDeg, Vector3.forward) * toPlayer;
                    Vector2 right = Quaternion.AngleAxis(-rotationDeg, Vector3.forward) * toPlayer;

                    Debug.DrawLine(transform.position, new Vector3(transform.position.x + left.x, transform.position.y + left.y, 0.0f));

                    e1.ChangeDirection(left);
                    e2.ChangeDirection(right);
                }
                GameObject newRocket3 = Instantiate(rocket, transform.position, Quaternion.identity);
                EnemyProjectileBehavior e3 = newRocket3.GetComponent<EnemyProjectileBehavior>();
                e3.ChangeDirection(toPlayer);
            }
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
