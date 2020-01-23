using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{
    // Essential components
    public GameObject enemyGroup;
    public GameObject blueEnemy;
    public GameObject redEnemy;

    // Enemy containers
    public List<GameObject> blueEnemies;
    public List<GameObject> redEnemies;
    public int numberOfEnemies;
    public int numberOfBlueEnemies;
    public int numberOfRedEnemies;

    // Enemy attack rate
    private const float blueEnemyCooldown = 5.0f;
    private float currentBlueEnemyCooldown;
    private const float redEnemyCooldown = 8.0f;
    private float currentRedEnemyCooldown;

    // Score implementation
    public Text scoreText;
    public int score;
    private const int BlueEnemyScoreGain = 100;
    private const int RedEnemyScoreGain = 200;

    // New wave of enemy
    private bool isGeneratingNewEnemies;

    // Start is called before the first frame update
    void Start()
    {
        isGeneratingNewEnemies = true;
        currentBlueEnemyCooldown = 0.0f;
        currentRedEnemyCooldown = 0.0f;
        score = 0;
        numberOfEnemies = 0;
        numberOfBlueEnemies = 0;
        Physics.IgnoreLayerCollision(8, 8); // Ignore collision between enemies
        Physics.IgnoreLayerCollision(11, 10); // Ignore collision between projectiles and rockets
        blueEnemies = new List<GameObject>();
        UpdateScore();
        StartCoroutine(GenerateAllEnemy());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateScore();
        HandleBlueEnemyChasing();
        HandleRedEnemyChasing();
        HandleEnemyRegenerating();
    }

    public void EnemyCountDecrement()
    {
        --numberOfEnemies;
    }

    public void BlueEnemyDestroyed(bool scoreGain)
    {
        --numberOfBlueEnemies;
        if(scoreGain) score += BlueEnemyScoreGain;
        UpdateScore();
    }

    public void RedEnemyDestroyed(bool scoreGain)
    {
        --numberOfRedEnemies;
        if (scoreGain) score += RedEnemyScoreGain;
        UpdateScore();
    }

    IEnumerator GenerateAllEnemy()
    {
        UpdateScore();
        isGeneratingNewEnemies = true;
        yield return new WaitForSeconds(1.0f);
        EnemyGroupMovement e = enemyGroup.GetComponent<EnemyGroupMovement>();
        e.ResetState();
        numberOfEnemies += 20;
        numberOfBlueEnemies += 10;
        numberOfRedEnemies += 10;
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                GenerateBlueEnemy(i, j);
                GenerateRedEnemy(i, j);
            }
        }
        yield return new WaitForSeconds(1.0f);

        SortBlueEnemyRenderer();
        SortRedEnemyRenderer();
        
        isGeneratingNewEnemies = false;
    }

    private void GenerateBlueEnemy(int posi, int posj)
    {
        GameObject newBlueEnemy = Instantiate(blueEnemy);
        newBlueEnemy.transform.parent = enemyGroup.transform;
        newBlueEnemy.transform.position = new Vector3(posi - 2.0f, posj + 0.0f, 0.0f);
        blueEnemies.Add(newBlueEnemy);
        SpriteRenderer s = newBlueEnemy.GetComponentInChildren<SpriteRenderer>();
        s.sortingOrder = -2;
    }

    private void GenerateRedEnemy(int posi, int posj)
    {
        GameObject newRedEnemy = Instantiate(redEnemy);
        newRedEnemy.transform.parent = enemyGroup.transform;
        newRedEnemy.transform.position = new Vector3(posi - 2.0f, posj + 2.0f, 0.0f); // two rows above blue enemies
        redEnemies.Add(newRedEnemy);
        SpriteRenderer s = newRedEnemy.GetComponentInChildren<SpriteRenderer>();
        s.sortingOrder = -2;
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
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e.ToString());
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
                        Debug.Log(e.ToString());
                    }
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e.ToString());
                blueEnemies.Remove(blue);
                BlueEnemyChasePlayer();
            }
            catch (NullReferenceException)
            {
                Debug.Log("blue is null!");
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
                Debug.Log(e.ToString());
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
                        Debug.Log(e.ToString());
                    }
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.Log(e.ToString());
                redEnemies.Remove(red);
                RedEnemyChasePlayer();
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

    private void HandleEnemyRegenerating()
    {
        if (numberOfEnemies <= 0 && !isGeneratingNewEnemies)
        {
            foreach (GameObject b in blueEnemies)
            {
                Destroy(b);
            }
            blueEnemies.Clear();
            StartCoroutine(GenerateAllEnemy());
        }
    }
}
