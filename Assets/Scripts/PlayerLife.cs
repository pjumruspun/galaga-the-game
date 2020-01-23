﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    // Essential components
    public GameObject player;

    // Lives Implementation
    public int lives = 3;
    public Text livesText;

    // Respawn Location
    private Vector3 respawnLocation = new Vector3(-3.0f, -4.0f, 0.0f);

    // Invulnerable Variables
    private bool isInvulnerable;
    private const float invulnerableTime = 3.0f;
    private float currentInvulTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        isInvulnerable = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLives();
        if (isInvulnerable)
        {
            Physics.IgnoreLayerCollision(8, 9, true);
            currentInvulTime += Time.deltaTime;
            if (currentInvulTime > invulnerableTime)
            {
                Physics.IgnoreLayerCollision(8, 9, false);
                isInvulnerable = false;
                currentInvulTime = 0.0f;
            }
        }
    }

    public void Respawn()
    {
        SetInvulnerable();
        --lives;
        UpdateLives();
        if (lives > 0)
        {
            GameObject newLife = Instantiate(player, respawnLocation, Quaternion.identity);
            SpriteRenderer s = newLife.GetComponent<SpriteRenderer>();
            s.sortingOrder = 2;
        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!!");
    }

    public void SetInvulnerable()
    {
        isInvulnerable = true;
    }

    public bool GetInvulnerableStatus()
    {
        return isInvulnerable;
    }

    private void UpdateLives()
    {
        string livesStr = "Lives: " + lives.ToString();
        livesText.text = livesStr;
    }
}