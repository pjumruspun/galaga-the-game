﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{
    // Adjustable Number of Rockets
    [SerializeField]
    private int numberOfRockets = 3;

    // Adjustable Enemy Speed
    [SerializeField]
    private float enemySpeedMult = 1.0f;
    private const float maxNaturalEnemySpeedMult = 2.5f;

    // Essential components
    private GameObject enemyGroup;
    [SerializeField]
    private GameObject blueEnemy;
    [SerializeField]
    private GameObject redEnemy;
    [SerializeField]
    private GameObject greenEnemy;
    /*
    // Audio background
    [SerializeField]
    private AudioSource music;
    */
    // Enemy containers
    private List<GameObject> blueEnemies;
    private List<GameObject> redEnemies;
    private List<GameObject> greenEnemies;
    private GameObject[] realGreenEnemyObjs;
    private int numberOfEnemies;
    private int numberOfBlueEnemies;
    private int numberOfRedEnemies;
    private int numberOfGreenEnemies;

    // Enemy attack rate
    private const float blueEnemyCooldown = 2.78f;
    private float currentBlueEnemyCooldown;
    private const float redEnemyCooldown = 5.95f;
    private float currentRedEnemyCooldown;
    private const float greenEnemyCooldown = 7.32f;
    private float currentGreenEnemyCooldown;

    // Score implementation
    [SerializeField]
    private Text scoreText;
    private int score;
    private const int blueEnemyScoreGain = 100;
    private const int redEnemyScoreGain = 200;
    private const int greenEnemyScoreGain = 300;

    // New wave of enemy
    private bool isGeneratingNewEnemies;

    // Layer numbers
    private const int enemyLayer = 8;
    private const int playerLayer = 9;
    private const int rocketLayer = 10;
    private const int playerProjectileLayer = 11;
    private const int powerUpLayer = 12;

    // Level implementation
    [SerializeField]
    private Text levelText;
    private int level = 0;

    // Shooting Speed Power up implementation
    [SerializeField]
    private GameObject speedPowerUp;
    [SerializeField]
    private Text speedPowerUpText;
    private int shootingSpeedLevel;  // MAX LEVEL = 10
    private float[] shootingSpeedMultiplier = { 1.0f, 1.2f, 1.4f, 1.6f, 1.8f, 1.9f, 2.0f, 2.1f, 2.2f, 2.3f };

    // Start is called before the first frame update
    void Start()
    {
        // music = GetComponent<AudioSource>();
        InitializeGame();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateScore();
        HandleBlueEnemyChasing();
        HandleRedEnemyChasing();
        HandleGreenEnemyChasing();
        HandleEnemyRegenerating();
        ClearMissingEnemies();
    }

    private void ClearMissingEnemies()
    {
        for (int i = blueEnemies.Count - 1; i > -1; --i)
        {
            if (blueEnemies[i] == null)
            {
                blueEnemies.RemoveAt(i);
            }
        }

        for (int i = redEnemies.Count - 1; i > -1; --i)
        {
            if (redEnemies[i] == null)
            {
                redEnemies.RemoveAt(i);
            }
        }

        for (int i = greenEnemies.Count - 1; i > -1; --i)
        {
            if (greenEnemies[i] == null)
            {
                greenEnemies.RemoveAt(i);
            }
        }
    }

    public void EnemyCountDecrement()
    {
        --numberOfEnemies;
    }

    public void BlueEnemyDestroyed(bool scoreGain)
    {
        --numberOfBlueEnemies;
        if(scoreGain) score += blueEnemyScoreGain;
        UpdateScore();
    }

    public void RedEnemyDestroyed(bool scoreGain)
    {
        --numberOfRedEnemies;
        if (scoreGain) score += redEnemyScoreGain;
        UpdateScore();
    }

    public void GreenEnemyDestroyed(bool scoreGain)
    {
        --numberOfGreenEnemies;
        if (scoreGain) score += greenEnemyScoreGain;
        UpdateScore();
    }

    IEnumerator GenerateAllEnemy()
    {
        ++level;
        UpdateLevel();
        UpdateScore();
        if(level != 1) enemySpeedMult += 0.1f;
        if (enemySpeedMult > maxNaturalEnemySpeedMult) enemySpeedMult = maxNaturalEnemySpeedMult;
        isGeneratingNewEnemies = true;
        yield return new WaitForSeconds(1.5f);
        EnemyGroupMovement e = enemyGroup.GetComponent<EnemyGroupMovement>();
        e.ResetState();
        numberOfEnemies += 25;
        numberOfBlueEnemies += 10;
        numberOfRedEnemies += 10;
        numberOfGreenEnemies += 5;
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                GenerateBlueEnemy(i, j);
                GenerateRedEnemy(i, j);
                if (j == 0) GenerateGreenEnemy(i, j);
            }
        }
        yield return new WaitForSeconds(0.05f);

        SortBlueEnemyRenderer();
        SortRedEnemyRenderer();
        SortGreenEnemyRenderer();

        realGreenEnemyObjs = GameObject.FindGameObjectsWithTag("GreenEnemy");

        isGeneratingNewEnemies = false;
        // enemyGroup.transform.position = Vector3.zero;
    }

    private void GenerateBlueEnemy(int posi, int posj)
    {
        GameObject newBlueEnemy = Instantiate(blueEnemy);
        newBlueEnemy.transform.parent = enemyGroup.transform;
        newBlueEnemy.transform.position = new Vector3(posi - 2.0f, posj + 0.0f, 0.0f);
        blueEnemies.Add(newBlueEnemy);
        SpriteRenderer s = newBlueEnemy.GetComponentInChildren<SpriteRenderer>();
    }

    private void GenerateRedEnemy(int posi, int posj)
    {
        GameObject newRedEnemy = Instantiate(redEnemy);
        newRedEnemy.transform.parent = enemyGroup.transform;
        newRedEnemy.transform.position = new Vector3(posi - 2.0f, posj + 2.0f, 0.0f); // two rows above blue enemies
        redEnemies.Add(newRedEnemy);
        SpriteRenderer s = newRedEnemy.GetComponentInChildren<SpriteRenderer>();
    }

    private void GenerateGreenEnemy(int posi, int posj)
    {
        GameObject newGreenEnemy = Instantiate(greenEnemy);
        newGreenEnemy.transform.parent = enemyGroup.transform;
        newGreenEnemy.transform.position = new Vector3(posi - 2.0f, posj + 4.0f, 0.0f); // two rows above blue enemies
        greenEnemies.Add(newGreenEnemy);
        SpriteRenderer s = newGreenEnemy.GetComponentInChildren<SpriteRenderer>();
    }

    private void SortBlueEnemyRenderer()
    {
        foreach (GameObject b in blueEnemies)
        {
            if (b != null)
            {
                SpriteRenderer s = b.GetComponentInChildren<SpriteRenderer>();
                s.sortingOrder = 0;
            }
        }
    }

    private void SortRedEnemyRenderer()
    {
        foreach (GameObject r in redEnemies)
        {
            if (r != null)
            {
                SpriteRenderer s = r.GetComponentInChildren<SpriteRenderer>();
                s.sortingOrder = 0;
            }
        }
    }

    private void SortGreenEnemyRenderer()
    {
        foreach (GameObject g in greenEnemies)
        {
            if (g != null)
            {
                SpriteRenderer s = g.GetComponentInChildren<SpriteRenderer>();
                s.sortingOrder = 0;
            }
        }
    }

    private void BlueEnemyChasePlayer()
    {
        if (!isGeneratingNewEnemies && blueEnemies.Count > 0)
        {
            int randInt = UnityEngine.Random.Range(0, blueEnemies.Count);
            if (blueEnemies[randInt] != null)
            {
                blueEnemies[randInt].GetComponentInChildren<BlueEnemyBehavior>().ChasePlayer();
            }
            else if (numberOfEnemies == 0)
            {
                // Debug.Log("Weird index but okay");
                StartCoroutine(GenerateAllEnemy());
            }
        }
    }

    private void RedEnemyChasePlayer()
    {
        if (!isGeneratingNewEnemies && redEnemies.Count > 0)
        {
            int randInt = UnityEngine.Random.Range(0, redEnemies.Count);
            if(redEnemies[randInt] != null)
            {
                redEnemies[randInt].GetComponentInChildren<RedEnemyBehavior>().ChasePlayer();
            }
        }
    }

    private void GreenEnemyChasePlayer()
    {
        if (!isGeneratingNewEnemies && greenEnemies.Count > 0)
        {
            int randInt = UnityEngine.Random.Range(0, greenEnemies.Count);
            if(greenEnemies[randInt] != null)
            {
                greenEnemies[randInt].GetComponentInChildren<GreenEnemyBehavior>().ChasePlayer();
            }
        }
    }

    private void UpdateScore()
    {
        String scoreStr = "Score: " + score.ToString();
        scoreText.text = scoreStr;
    }

    private void UpdateLevel()
    {
        String levelStr = "Level: " + level.ToString();
        levelText.text = levelStr;
    }

    private void HandleBlueEnemyChasing()
    {
        float e = enemySpeedMult;
        if (e < 0.5f) e = 0.5f;
        if (currentBlueEnemyCooldown > blueEnemyCooldown / e)
        {
            BlueEnemyChasePlayer();
            int doubleChase = UnityEngine.Random.Range(0, 2);
            if (doubleChase >= 1)
                BlueEnemyChasePlayer();
            currentBlueEnemyCooldown = 0.0f;
        }
        currentBlueEnemyCooldown += Time.deltaTime;
    }

    private void HandleRedEnemyChasing()
    {
        float e = enemySpeedMult;
        if (e < 0.5f) e = 0.5f;
        if (currentRedEnemyCooldown > redEnemyCooldown / e)
        {
            RedEnemyChasePlayer();
            int doubleChase = UnityEngine.Random.Range(0, 2);
            if (doubleChase >= 1)
                RedEnemyChasePlayer();
            currentRedEnemyCooldown = 0.0f;
        }
        currentRedEnemyCooldown += Time.deltaTime;
    }

    private void HandleGreenEnemyChasing()
    {
        float e = enemySpeedMult;
        if (e < 0.5f) e = 0.5f;
        if (currentGreenEnemyCooldown > greenEnemyCooldown / e)
        {
            GreenEnemyChasePlayer();
            currentGreenEnemyCooldown = 0.0f;
        }
        currentGreenEnemyCooldown += Time.deltaTime;
    }

    private void HandleEnemyRegenerating()
    {
        if (numberOfEnemies <= 0 && !isGeneratingNewEnemies)
        {
            DestroyAll(blueEnemies);
            DestroyAll(redEnemies);
            DestroyAll(greenEnemies);
            StartCoroutine(GenerateAllEnemy());
        }
    }

    private void DestroyAll(List<GameObject> aList)
    {
        foreach (GameObject r in aList)
        {
            Destroy(r);
        }
       aList.Clear();
    }

    public int GetNumberOfRockets()
    {
        return numberOfRockets;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        DestroyAll(blueEnemies);
        DestroyAll(redEnemies);
        DestroyAll(greenEnemies);
        ClearAllNonEnemyGameObjects();

        foreach(GameObject g in realGreenEnemyObjs)
        {
            Destroy(g);
        }

        InitializeGame();
    }

    private void ClearAllNonEnemyGameObjects()
    {
        GameObject[] projs = GameObject.FindGameObjectsWithTag("Projectile");
        GameObject[] rockets = GameObject.FindGameObjectsWithTag("Rocket");
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("SpeedPU");
        // TO DO: Clear laser
        foreach(GameObject g in projs)
        {
            Destroy(g);
        }
        foreach (GameObject g in rockets)
        {
            Destroy(g);
        }
        foreach (GameObject g in powerUps)
        {
            Destroy(g);
        }
    }

    private void InitializeGame()
    {
        //music.Play();

        enemySpeedMult = 1.0f;
        level = 0;
        shootingSpeedLevel = 1;
        enemyGroup = GameObject.FindGameObjectWithTag("EnemyGroup");

        Time.timeScale = 1f;
        isGeneratingNewEnemies = true;
        currentBlueEnemyCooldown = 0.0f;
        currentRedEnemyCooldown = 0.0f;
        currentGreenEnemyCooldown = 0.0f;
        score = 0;
        numberOfEnemies = 0;
        numberOfBlueEnemies = 0;
        numberOfRedEnemies = 0;
        numberOfGreenEnemies = 0;

        Physics.IgnoreLayerCollision(enemyLayer, enemyLayer); // Ignore collision between enemies
        Physics.IgnoreLayerCollision(playerProjectileLayer, rocketLayer); // Ignore collision between projectiles and rockets
        Physics.IgnoreLayerCollision(rocketLayer, rocketLayer); // Ignore collision between rockets and rockets
        Physics.IgnoreLayerCollision(rocketLayer, enemyLayer); // Ignore collision between rockets and enemies

        
        Physics.IgnoreLayerCollision(8, 12); // Ignore collision between rockets and enemies
        Physics.IgnoreLayerCollision(10, 12); // Ignore collision between projectiles and rockets
        Physics.IgnoreLayerCollision(11, 12); // Ignore collision between enemies
        
        blueEnemies = new List<GameObject>();
        redEnemies = new List<GameObject>();
        greenEnemies = new List<GameObject>();
        
        UpdateScore();
        UpdateShootingSpeed();
        StartCoroutine(GenerateAllEnemy());

    }

    public float GetEnemySpeedMult()
    {
        return enemySpeedMult;
    }

    public float GetShootingSpeedLevel()
    {
        return shootingSpeedLevel;
    }

    public void ShootingSpeedLevelUp()
    {
        //Debug.Log(shootingSpeedLevel);
        ++shootingSpeedLevel;
        if (shootingSpeedLevel > 10) shootingSpeedLevel = 10;
        UpdateShootingSpeed();
    }

    private void UpdateShootingSpeed()
    {
        speedPowerUpText.text = "Atk Speed:\nLevel " + shootingSpeedLevel + "/10";
    }

    public float GetShootingSpeedMult()
    {
        return shootingSpeedMultiplier[shootingSpeedLevel - 1];
    }

    public GameObject GetSpeedPowerUp()
    {
        return speedPowerUp;
    }
}
