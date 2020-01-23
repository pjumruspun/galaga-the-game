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

    // Enemy containers
    public List<GameObject> blueEnemies;
    public int numberOfEnemies;
    public int numberOfBlueEnemies;

    // Enemy attack rate
    private const float blueEnemyCooldown = 5.0f;
    private float currentBlueEnemyCooldown;

    // Score implementation
    public Text scoreText;
    public int score;
    private const int BlueEnemyScoreGain = 100;

    // New wave of enemy
    private bool isGeneratingNewEnemies;

    // Start is called before the first frame update
    void Start()
    {
        isGeneratingNewEnemies = true;
        currentBlueEnemyCooldown = 0.0f;
        score = 0;
        numberOfEnemies = 0;
        numberOfBlueEnemies = 0;
        Physics.IgnoreLayerCollision(8, 8);
        blueEnemies = new List<GameObject>();
        UpdateScore();
        StartCoroutine(GenerateAllEnemy());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateScore();
        HandleBlueEnemyChasing();
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

    IEnumerator GenerateAllEnemy()
    {
        UpdateScore();
        isGeneratingNewEnemies = true;
        yield return new WaitForSeconds(2.0f);
        EnemyGroupMovement e = enemyGroup.GetComponent<EnemyGroupMovement>();
        e.ResetState();
        numberOfEnemies += 10;
        numberOfBlueEnemies += 10;
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                GameObject newBlueEnemy = Instantiate(blueEnemy);
                newBlueEnemy.transform.parent = enemyGroup.transform;
                newBlueEnemy.transform.position = new Vector3(i - 2.0f, j + 0.0f, 0.0f);
                blueEnemies.Add(newBlueEnemy);
                SpriteRenderer s = newBlueEnemy.GetComponentInChildren<SpriteRenderer>();
                s.sortingOrder = 0;
            }
        }
        isGeneratingNewEnemies = false;
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
                if(numberOfEnemies == 0)
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
                    catch (NullReferenceException)
                    {

                    }
                }
            }
            catch (MissingReferenceException)
            {
                blueEnemies.Remove(blue);
                BlueEnemyChasePlayer();
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
