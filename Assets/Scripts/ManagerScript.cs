using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{
    // Adjustable Number of Rockets
    [SerializeField]
    private int numberOfRockets = 3;

    // Essential components
    private GameObject enemyGroup;
    public GameObject blueEnemy;
    public GameObject redEnemy;
    public GameObject greenEnemy;

    // Enemy containers
    [SerializeField]
    private List<GameObject> blueEnemies;
    [SerializeField]
    private List<GameObject> redEnemies;
    [SerializeField]
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
    public Text scoreText;
    public int score;
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

    // Start is called before the first frame update
    void Start()
    {
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
        UpdateScore();
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
        if (!isGeneratingNewEnemies)
        {
            int randInt = UnityEngine.Random.Range(0, blueEnemies.Count);
            GameObject blue = null;
            try
            {
                blue = blueEnemies[randInt];
            }
            catch (ArgumentOutOfRangeException)
            {
                // Debug.Log("Weird index but okay");
                if (numberOfEnemies == 0)
                {
                    StartCoroutine(GenerateAllEnemy());
                }
            }
            try
            {
                BlueEnemyBehavior b = blue.GetComponentInChildren<BlueEnemyBehavior>();
                if (blue != null)
                {
                    try
                    {
                        b.ChasePlayer();
                    }
                    catch (NullReferenceException e)
                    {
                        // Debug.Log(e.ToString());
                    }
                }
            }
            catch (MissingReferenceException e)
            {
                // Debug.Log("Blue missing Reference! I'm trying to do recursion while delete the missing obj");
                blueEnemies.Remove(blue);
                BlueEnemyChasePlayer();
            }
            catch (NullReferenceException)
            {
                // Debug.Log("blue is null!");
            }
        }
    }

    private void RedEnemyChasePlayer()
    {
        if (!isGeneratingNewEnemies)
        {
            int randInt = UnityEngine.Random.Range(0, redEnemies.Count);
            GameObject red = null;
            try
            {
                red = redEnemies[randInt];
            }
            catch (ArgumentOutOfRangeException e)
            {
                /*
                if (numberOfEnemies == 0)
                {
                    StartCoroutine(GenerateAllEnemy());
                }
                */
                // Debug.Log(e.ToString());
            }
            try
            {
                RedEnemyBehavior r = red.GetComponentInChildren<RedEnemyBehavior>();
                if (red != null)
                {
                    try
                    {
                        r.ChasePlayer();
                    }
                    catch (NullReferenceException e)
                    {
                        // Debug.Log(e.ToString());
                    }
                }
            }
            catch (MissingReferenceException e)
            {
                // Debug.Log("Red missing Reference! I'm trying to do recursion while delete the missing obj");
                redEnemies.Remove(red);
                RedEnemyChasePlayer();
            }
            catch (NullReferenceException)
            {
                // Debug.Log("red is null!");
            }
        }
    }

    private void GreenEnemyChasePlayer()
    {
        if (!isGeneratingNewEnemies)
        {
            int randInt = UnityEngine.Random.Range(0, greenEnemies.Count);
            GameObject green = null;
            try
            {
                green = greenEnemies[randInt];
            }
            catch (ArgumentOutOfRangeException e)
            {
                /*
                if (numberOfEnemies == 0)
                {
                    StartCoroutine(GenerateAllEnemy());
                }
                */
                // Debug.Log(e.ToString());
            }
            try
            {
                GreenEnemyBehavior g = green.GetComponentInChildren<GreenEnemyBehavior>();
                if (green != null)
                {
                    try
                    {
                        g.ChasePlayer();
                    }
                    catch (NullReferenceException e)
                    {
                        // Debug.Log(e.ToString());
                    }
                }
            }
            catch (MissingReferenceException)
            {
                // Debug.Log("Green missing Reference! I'm trying to do recursion while delete the missing obj");
                greenEnemies.Remove(green);
                GreenEnemyChasePlayer();
            }
            catch (NullReferenceException)
            {
                // Debug.Log("green is null!");
            }
        }
    }

    private void UpdateScore()
    {
        String scoreStr = "Score: " + score.ToString();
        scoreText.text = scoreStr;
    }

    private void HandleBlueEnemyChasing()
    {
        if (currentBlueEnemyCooldown > blueEnemyCooldown)
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
        if (currentRedEnemyCooldown > redEnemyCooldown)
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
        if (currentGreenEnemyCooldown > greenEnemyCooldown)
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
        ClearAllProjectilesAndRockets();

        foreach(GameObject g in realGreenEnemyObjs)
        {
            Destroy(g);
        }

        InitializeGame();
    }

    private void ClearAllProjectilesAndRockets()
    {
        GameObject[] projs = GameObject.FindGameObjectsWithTag("Projectile");
        GameObject[] rockets = GameObject.FindGameObjectsWithTag("Rocket");
        // TO DO: Clear laser
        foreach(GameObject g in projs)
        {
            Destroy(g);
        }
        foreach (GameObject g in rockets)
        {
            Destroy(g);
        }
    }

    private void InitializeGame()
    {
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

        blueEnemies = new List<GameObject>();
        redEnemies = new List<GameObject>();
        greenEnemies = new List<GameObject>();
        
        UpdateScore();
        StartCoroutine(GenerateAllEnemy());

    }
}
